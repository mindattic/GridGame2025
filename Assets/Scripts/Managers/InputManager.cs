// --- File: Assets/Scripts/Managers/InputManager.cs ---
using Assets.Helpers;
using Assets.Scripts.Sequences;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Handles touch input and delegates to the correct systems.
/// Modes:
/// - PlayerTurn: focus, drag, drop for the selected hero.
/// - AbilityTarget: tap to select a target and execute ability.
/// - LinearTarget: show straight-line paths and select valid target in row/column.
/// - EnemyTurn: tap a hero to trigger a dodge window; at enemy impact, evaluate Parry or Dodge timing.
/// </summary>
public class InputManager : MonoBehaviour
{
    private Vector3 initialTouchPosition;
    public float dragThreshold;

    /// <summary>
    /// Raised whenever the input mode changes.
    /// </summary>
    public event Action<InputMode> OnInputModeChanged;

    private InputMode inputMode = InputMode.PlayerTurn;

    // Cancel button (optional)
    private GameObject cancelButton;

    // Ability user cache for targeting flows (do not reuse SelectedHero to avoid side-effects)
    private ActorInstance pendingAbilityUser;

    /// <summary>
    /// Current input mode for the screen.
    /// </summary>
    public InputMode InputMode
    {
        get => inputMode;
        set
        {
            if (inputMode == value) return;
            inputMode = value;
            OnInputModeChanged?.Invoke(inputMode);
        }
    }

    /// <summary>
    /// Fired when a successful dodge timing occurs during EnemyTurn.
    /// </summary>
    public event Action OnDodge;

    /// <summary>
    /// Fired when a successful parry timing occurs during EnemyTurn.
    /// </summary>
    public event Action OnParry;

    private const float DodgeWindowSeconds = 0.20f;
    private const float ParryWindowSeconds = 0.10f;

    private float lastEnemyTurnTapTime = -999f;
    private ActorInstance lastEnemyTurnTappedHero = null;

    /// <summary>
    /// True if the selected hero is currently being dragged.
    /// </summary>
    public bool isDragging => g.Actors.HasSelectedHero && g.Actors.SelectedHero.Flags.IsMoving;

    // --------------------------------------------------------------------------------------------
    // Gate input until current press is released (used when timer forces a Drop)
    // --------------------------------------------------------------------------------------------
    private bool requireTouchRelease;

    /// <summary>
    /// Prevent any input from being processed until all touches/mouse buttons are released.
    /// </summary>
    public void RequireTouchRelease()
    {
        requireTouchRelease = true;
    }

    private static bool AnyPointerDown()
    {
        return Input.touchCount > 0 || Input.GetMouseButton(0);
    }

    private void Awake()
    {
        dragThreshold = GameManager.instance.tileSize * 0.01f;

        // Cache CancelButton if present and start hidden
        var cancelObj = GameObject.Find("CancelButton");
        if (cancelObj != null)
        {
            cancelButton = cancelObj;
            cancelButton.SetActive(false);
        }
    }

    /// <summary>
    /// Prepare a user for a targeting flow.
    /// </summary>
    public void BeginAbilityTargeting(ActorInstance user)
    {
        pendingAbilityUser = user;
    }

    /// <summary>
    /// Clear any cached ability user.
    /// </summary>
    private void ClearPendingUser()
    {
        pendingAbilityUser = null;
    }

    /// <summary>
    /// Show the global Cancel button (if found).
    /// </summary>
    public void ShowCancelButton()
    {
        if (cancelButton != null) cancelButton.SetActive(true);
    }

    /// <summary>
    /// Hide the global Cancel button (if found).
    /// </summary>
    public void HideCancelButton()
    {
        if (cancelButton != null) cancelButton.SetActive(false);
    }

    /// <summary>
    /// Bind this to the Canvas/CancelButton OnClick. Cancels targeting and returns to PlayerTurn.
    /// </summary>
    public void OnCancelButtonClickedEvent()
    {
        // Clear any highlights/indicators
        g.TileManager.Reset();
        if (g.Actors.HasTargetActor)
        {
            g.Actors.TargetActor.Render.SetTargetIndicatorEnabled(false);
            g.Actors.TargetActor = null;
        }

        // Stop any drag motion on selected hero and snap to tile
        if (g.Actors.HasSelectedHero)
        {
            g.Actors.SelectedHero.Move.ToLocation();
            g.Actors.SelectedHero.Flags.IsMoving = false;
            g.Actors.SelectedHero.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        // If we were targeting with a specific user (e.g., Paladin Shield Bash), ensure they are snapped and idle
        if (pendingAbilityUser != null)
        {
            pendingAbilityUser.Move.ToLocation();
            pendingAbilityUser.Flags.IsMoving = false;
            pendingAbilityUser.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        // Clear any cached data and offsets
        ClearPendingUser();
        g.TouchOffset = Vector3.zero;

        // Restore normal input
        HideCancelButton();
        InputMode = InputMode.PlayerTurn;
        RequireTouchRelease();
    }

    /// <summary>
    /// Ability targeting flow. First tap selects a target, second tap confirms and executes.
    /// </summary>
    private void UpdateAbilityTarget(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                var target = TouchHelper.GetActorAtTouchPosition();
                if (target == null || !target.IsPlaying) return;

                if (g.Actors.TargetActor == target)
                {
                    HideCancelButton();
                    var startPosition = g.Card.PortraitWorldPosition();
                    g.SequenceManager.Add(new HealAbilitySequence(startPosition, g.Actors.TargetActor));
                    g.SequenceManager.Add(new HideTargetIndicatorSequence());
                    g.SequenceManager.Execute();
                    return;
                }

                g.Actors.TargetActor = target;
                g.Actors.TargetActor.Render.SetTargetIndicatorEnabled(true);
                break;

            case TouchPhase.Moved:
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:

                break;
        }
    }

    // Centralized execution for Paladin Shield Bash once a valid hero/target pair is chosen.
    private void TryExecuteShieldBash(ActorInstance hero, ActorInstance target)
    {
        if (hero == null || target == null) return;
        if (!hero.IsHero || !target.IsEnemy) return;
        if (!target.IsPlaying) return;

        // Must be aligned strictly in row or column
        bool aligned = hero.location.x == target.location.x || hero.location.y == target.location.y;
        if (!aligned) return;

        // Ensure clear path: no intervening actors
        var between = g.TileMap.EnumerateBetween(hero.location, target.location);
        if (between.Any(t => t != null && t.IsOccupied)) return;

        // Build and run sequence atomically
        HideCancelButton();
        g.TileManager.Reset();
        InputMode = InputMode.None; // lock input for duration
        g.SequenceManager.Add(new ShieldBashSequence(hero, target));
        g.SequenceManager.Add(new SequenceCallback(() =>
        {
            ClearPendingUser();
            InputMode = InputMode.PlayerTurn;
        }));
        g.SequenceManager.Execute();
    }

    // Linear target: select an enemy in same row/column with clear line; move hero and bump
    private void UpdateLinearTarget(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                var hero = pendingAbilityUser; // acting paladin for Shield Bash
                var target = TouchHelper.GetActorAtTouchPosition();
                TryExecuteShieldBash(hero, target);
                break;
        }
    }

    /// <summary>
    /// Player turn flow. Focus on touch, drag past threshold, drop on release.
    /// </summary>
    private void UpdatePlayerTurn(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                g.SelectedHeroManager.Focus();
                initialTouchPosition = g.TouchPosition3D;
                break;

            case TouchPhase.Moved:
                if (Vector3.Distance(initialTouchPosition, g.TouchPosition3D) > dragThreshold)
                    g.SelectedHeroManager.Drag();
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                g.SelectedHeroManager.Drop();
                break;
        }
    }

    /// <summary>
    /// Enemy turn flow. Tapping a hero triggers a brief dodge animation and opens timing windows.
    /// Parry window is the first half; Dodge window is the full duration.
    /// </summary>
    private void UpdateEnemyTurn(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                {
                    var actor = TouchHelper.GetActorAtTouchPosition();
                    if (actor != null && actor.IsPlaying && actor.IsHero)
                    {
                        // Start dodge animation. If you have a duration overload, prefer Dodge(DodgeWindowSeconds).
                        actor.Animation.Dodge();

                        // Begin timing window for this tap.
                        lastEnemyTurnTapTime = Time.time;
                        lastEnemyTurnTappedHero = actor;
                    }
                    break;
                }

            case TouchPhase.Moved:
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                break;
        }
    }

    /// <summary>
    /// Called by combat at the exact enemy impact frame.
    /// Evaluates timing against Parry first, then Dodge, and raises the corresponding event.
    /// </summary>
    public void OnEnemyAttackOccurred()
    {
        if (inputMode != InputMode.EnemyTurn) return;

        var dt = Time.time - lastEnemyTurnTapTime;

        if (dt >= 0f && dt <= ParryWindowSeconds)
        {
            OnParry?.Invoke();
            lastEnemyTurnTapTime = -999f;
            return;
        }

        if (dt >= 0f && dt <= DodgeWindowSeconds)
        {
            OnDodge?.Invoke();
            lastEnemyTurnTapTime = -999f;
            return;
        }

        // No timing success.
    }

    private void Update()
    {
        if (GameManager.instance.inputManager == null) return;
        if (g.PauseManager.IsPaused) return;

        // If we require a touch/mouse release (e.g., timer-forced Drop just happened),
        // block all input until nothing is pressed.
        if (requireTouchRelease)
        {
            if (!AnyPointerDown())
            {
                requireTouchRelease = false;
            }
            return;
        }

        // Primary touch handling
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            switch (InputMode)
            {
                case InputMode.None:
                    break;

                case InputMode.AbilityTarget:
                    UpdateAbilityTarget(touch);
                    break;

                case InputMode.LinearTarget:
                    UpdateLinearTarget(touch);
                    break;

                case InputMode.PlayerTurn:
                    UpdatePlayerTurn(touch);
                    break;

                case InputMode.EnemyTurn:
                    UpdateEnemyTurn(touch);
                    break;
            }
        }
        else
        {
            // Mouse fallback for Editor/PC: mirror the Began/Moved/Ended flows
            switch (InputMode)
            {
                case InputMode.None:
                    break;

                case InputMode.AbilityTarget:
                    if (Input.GetMouseButtonDown(0))
                    {
                        var target = TouchHelper.GetActorAtTouchPosition();
                        if (target == null || !target.IsPlaying) break;

                        if (g.Actors.TargetActor == target)
                        {
                            HideCancelButton();
                            var startPosition = g.Card.PortraitWorldPosition();
                            g.SequenceManager.Add(new HealAbilitySequence(startPosition, g.Actors.TargetActor));
                            g.SequenceManager.Add(new HideTargetIndicatorSequence());
                            g.SequenceManager.Execute();
                        }
                        else
                        {
                            g.Actors.TargetActor = target;
                            g.Actors.TargetActor.Render.SetTargetIndicatorEnabled(true);
                        }
                    }
                    break;

                case InputMode.LinearTarget:
                    if (Input.GetMouseButtonDown(0))
                    {
                        var hero = pendingAbilityUser;
                        var target = TouchHelper.GetActorAtTouchPosition();
                        TryExecuteShieldBash(hero, target);
                    }
                    break;

                case InputMode.PlayerTurn:
                    if (Input.GetMouseButtonDown(0))
                    {
                        g.SelectedHeroManager.Focus();
                        initialTouchPosition = g.TouchPosition3D;
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        if (Vector3.Distance(initialTouchPosition, g.TouchPosition3D) > dragThreshold)
                            g.SelectedHeroManager.Drag();
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        g.SelectedHeroManager.Drop();
                    }
                    break;

                case InputMode.EnemyTurn:
                    if (Input.GetMouseButtonDown(0))
                    {
                        var actor = TouchHelper.GetActorAtTouchPosition();
                        if (actor != null && actor.IsPlaying && actor.IsHero)
                        {
                            actor.Animation.Dodge();
                            lastEnemyTurnTapTime = Time.time;
                            lastEnemyTurnTappedHero = actor;
                        }
                    }
                    break;
            }
        }
    }
}
