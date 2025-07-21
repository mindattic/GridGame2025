using Assets.Scripts.Repositories;
using System;
using System.Linq;
using UnityEngine;
using game = GameManagerHelper;



// InputManager handles hero touch input and delegates focus, drag, and drop actions
// to the SelectedHeroManager, while also considering the game's paused state.
public class InputManager : MonoBehaviour
{
    protected PauseManager pauseManager => GameManager.instance.pauseManager;
    protected ActorInstance focusedActor => GameManager.instance.focusedActor;
    protected ActorInstance selectedPlayer => GameManager.instance.selectedHero;
    protected bool hasSelectedActor => focusedActor != null;
    protected bool hasSelectedPlayer => selectedPlayer != null;
    protected SelectedHeroManager selectedHeroManager => GameManager.instance.selectedHeroManager;
    protected StageManager stageManager => GameManager.instance.stageManager;
    protected Vector3 touchPosition3D => GameManager.instance.touchPosition3D;
    protected float tileSize => GameManager.instance.tileSize;
    protected TargetLineManager targetLineManager => GameManager.instance.targetLineManager;
    protected ActorInstance targetActor
    {
        get => GameManager.instance.targetActor;
        set => GameManager.instance.targetActor = value;
    }
    protected bool hasTargetActor => GameManager.instance.hasTargetActor;
    protected TargetIndicator targetIndicator => GameManager.instance.targetIndicator;
    

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
        dragThreshold = tileSize * 0.125f;
    }

    void Update()
    {
        if (pauseManager.IsPaused)
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

                            var collisions = Physics2D.OverlapPointAll(touchPosition3D);
                            if (collisions == null) return;
                            var collider = collisions.FirstOrDefault(x => x.CompareTag(Tag.Actor));
                            if (collider == null) return;
                            var actor = collider.gameObject.GetComponent<ActorInstance>();

                            if (actor == null || !actor.isPlaying) return;

                            if (targetActor == actor)
                            {
                                //This is a double click...

                                ConfirmationDialog.Show(canvas2D, "Are you sure?", onSubmit: (value) =>
                                {
                                    if (value)
                                    {
                                        Debug.Log("You targetted: " + targetActor.characterName);
                                        //TODO: Add sequence and exectue....
                                        inputMode = InputMode.HeroTurn; 
                                    } else
                                    {

                                    }
                                });



                                return;
                            }
                               

                            targetActor = actor;
                            targetIndicator.Assign();
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
                            selectedHeroManager.Focus();

                            initialTouchPosition = touchPosition3D;
                            break;

                        case TouchPhase.Moved:
                            if (Vector3.Distance(initialTouchPosition, touchPosition3D) > dragThreshold)
                                selectedHeroManager.Drag();
                            break;

                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            selectedHeroManager.Drop();
                            break;
                    }
                    #endregion
                    break;
            }
        }
    }
}
