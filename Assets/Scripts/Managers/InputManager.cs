using Assets.Scripts.Events;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;
using static GameObjectHelper;
using g = Assets.Helpers.GameManagerHelper;



// InputManager handles hero touch input and delegates focus, drag, and drop action
// to the SelectedHeroManager, while also considering the game's paused state.
public class InputManager : MonoBehaviour
{
    private Vector3 initialTouchPosition;
    public float dragThreshold;

    // Fired whenever inputMode changes
    public event Action<InputMode> OnInputModeChanged;
    private InputMode _inputMode = InputMode.HeroTurn;
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

    private RectTransform canvas2D;

    private void Awake()
    {
        canvas2D = GameObject.Find("Canvas2D").GetComponent<RectTransform>();
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
                case InputMode.AbilityTarget:
                    #region AbilityTarget

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:

                            var collisions = Physics2D.OverlapPointAll(g.TouchPosition3D);
                            if (collisions == null) return;
                            var collider = collisions.FirstOrDefault(x => x.CompareTag(Tag.Actor));
                            if (collider == null) return;
                            var target = collider.gameObject.GetComponent<ActorInstance>();
                            if (target == null || !target.isPlaying) return;
                           
                            if (g.Actors.TargetActor == target)
                            {
                                //This is a double click...
                                //ConfirmationDialog.Show(canvas2D, "Are you sure?", onSubmit: (value) =>
                                //{
                                //    var btn = g.AbilityButtonManager.buttons.First();
                                //    var startPosition = g.AbilityButtonManager.buttons.First().transform.localPosition;

                                //    if (value)
                                //        g.SequenceManager.Add(new HealAbilitySequence(startPosition, g.Actors.TargetActor));

                                //    g.SequenceManager.Add(new HideTargetIndicatorSequence());
                                //    g.SequenceManager.TriggerExecute();
                                //});

                                //Spawn from avility button


                                //Spawn at Actor
                                //var startPosition = g.Actors.FocusedActor.position;

                                //Spawn at button
                                //var startPosition = g.AbilityButtonManager.buttons.First().WorldPosition();

                                //Spawn at card portrait
                                var startPosition = g.Card.PortraitWorldPosition();

                                g.SequenceManager.Add(new HealAbilitySequence(startPosition, g.Actors.TargetActor));
                                g.SequenceManager.Add(new HideTargetIndicatorSequence());
                                g.SequenceManager.TriggerExecute();

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

                case InputMode.EnemyTurn:
                    #region Enemy Turn
                    #endregion
                    break;

                case InputMode.HeroTurn:
                    #region Hero Turn
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            // Attempt to focus on an targetActor under the touch.
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
