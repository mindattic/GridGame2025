using Assets.Helpers;
using Assets.Scripts.Events;
using System;
using UnityEngine;
using g = Assets.Helpers.GameHelper;



// InputManager handles hero touch input and delegates focus, drag, and drop action
// to the SelectedHeroManager, while also considering the game's paused state.
public class InputManager : MonoBehaviour
{
    private Vector3 initialTouchPosition;
    public float dragThreshold;

    // Fired whenever inputMode changes
    public event Action<InputMode> OnInputModeChanged;
    private InputMode _inputMode = InputMode.Player;
    public InputMode inputMode
    {
        get => _inputMode;
        set
        {
            if (_inputMode == value)
                return;

            _inputMode = value;
            OnInputModeChanged?.Invoke(_inputMode);
        }
    }


    public bool isDragging => g.Actors.HasSelectedHero && g.Actors.SelectedHero.flags.IsMoving;

    private void Awake()
    {
        dragThreshold = GameManager.instance.tileSize * 0.125f;
    }

    void Update()
    {
        if (GameManager.instance.inputManager == null)
            return;

        if (g.PauseManager.IsPaused)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (inputMode)
            {
                case InputMode.None:
                    //Screen receieves no input
                    break;

                case InputMode.AbilityTarget:
                    #region AbilityTarget

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:

                            var target = TouchHelper.GetActorAtTouchPosition();
                            if (target == null || !target.isPlaying) return;


                            if (g.Actors.TargetActor == target)
                            {
                                //This is a double click...
                                //ConfirmationDialog.Show(canvas, "Are you sure?", onSubmit: (value) =>
                                //{
                                //    var btn = g.AbilityButtonManager.buttons.First();
                                //    var startPosition = g.AbilityButtonManager.buttons.First().transform.localPosition;

                                //    if (value)
                                //        g.SequenceManager.Add(new HealAbilitySequence(startPosition, g.Actors.TargetActor));

                                //    g.SequenceManager.Add(new HideTargetIndicatorSequence());
                                //    g.SequenceManager.ProcessRoutine();
                                //});

                                //YieldSpawn from avility button


                                //YieldSpawn at Actor
                                //var startPosition = g.Actors.FocusedActor.position;

                                //YieldSpawn at button
                                //var startPosition = g.AbilityButtonManager.buttons.First().WorldPosition();

                                //YieldSpawn at card portrait
                                var startPosition = g.Card.PortraitWorldPosition();

                                g.SequenceManager.Add(new HealAbilitySequence(startPosition, g.Actors.TargetActor));
                                g.SequenceManager.Add(new HideTargetIndicatorSequence());
                                g.SequenceManager.Execute();

                                return;
                            }

                            g.Actors.TargetActor = target;
                            g.TargetIndicator.Show();
                            break;

                        case TouchPhase.Moved:

                            break;

                        case TouchPhase.Ended:




                            break;
                        case TouchPhase.Canceled:

                            break;
                    }
                    #endregion
                    break;

                case InputMode.Player:
                    #region Hero Turn
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            // Attempt to focus on an target under the touch.
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
                    #endregion
                    break;
            }
        }
    }
}
