using System;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;



// InputManager handles hero touch input and delegates focus, drag, and drop actions
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
                            var actor = collider.gameObject.GetComponent<ActorInstance>();

                            if (actor == null || !actor.isPlaying) return;

                            if (g.Actors.TargetActor == actor)
                            {
                                //This is a double click...

                                ConfirmationDialog.Show(canvas2D, "Are you sure?", onSubmit: (value) =>
                                {
                                    if (value)
                                    {
                                        Debug.Log("You targetted: " + g.Actors.TargetActor.characterName);
                                        //TODO: Add sequence and exectue....
                                        inputMode = InputMode.HeroTurn;
                                    }
                                    else
                                    {

                                    }
                                });



                                return;
                            }


                            g.Actors.TargetActor = actor;
                            g.TargetIndicator.Assign();
                            break;

                        case TouchPhase.Moved:

                            break;

                        case TouchPhase.Ended:
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
                            // Attempt to focus on an actor under the touch.
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
