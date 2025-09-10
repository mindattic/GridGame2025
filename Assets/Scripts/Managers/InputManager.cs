// --- File: Assets/Scripts/Managers/InputManager.cs ---
using Assets.Helpers;
using Assets.Scripts.Sequences;
using System;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

/// <summary>
/// Handles touch input and delegates to the correct systems.
/// Modes:
/// - PlayerTurn: focus, drag, drop for the selected hero.
/// - AbilityTarget: tap to select a target and execute ability.
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

    private void Awake()
    {
        dragThreshold = GameManager.instance.tileSize * 0.01f;
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

                case InputMode.PlayerTurn:
                    UpdatePlayerTurn(touch);
                    break;

                case InputMode.EnemyTurn:
                    UpdateEnemyTurn(touch);
                    break;
            }
        }
    }
}
