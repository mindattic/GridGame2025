using System;
using System.Linq;
using UnityEngine;

public enum InputMode
{
    Gameplay,
    AbilityTarget
}


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


    private bool isTouching = false;
    private Vector3 initialTouchPosition;
    public float dragThreshold;



    // Fired whenever inputMode changes
    public event Action<InputMode> OnInputModeChanged;
    private InputMode _inputMode = InputMode.Gameplay;
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


    private void Awake()
    {
        dragThreshold = tileSize * 0.125f;
    }

    void Update()
    {
        if (pauseManager.IsPaused)
            return;

        switch (inputMode)
        {
            case InputMode.AbilityTarget:
                #region AbilityTarget

                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    var touch = Input.GetTouch(0);
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
                    worldPos.z = 0f;

                    // Find the hero under the touch
                    var tappedHero = GameManager.instance.actors
                        .Where(a => Vector3.Distance(a.position, worldPos) < dragThreshold)
                        .OrderBy(a => Vector3.Distance(a.position, worldPos))
                        .FirstOrDefault();

                    targetLineManager.OnTargetTouch(tappedHero);
                }
                break;
            #endregion


            #region GamePlay

            case InputMode.Gameplay:
            default:
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            // Attempt to focus on an actor under the touch.
                            selectedHeroManager.Focus();
                            isTouching = true;
                            initialTouchPosition = touchPosition3D;
                            break;

                        case TouchPhase.Moved:
                            if (isTouching && Vector3.Distance(initialTouchPosition, touchPosition3D) > dragThreshold)
                            {
                                selectedHeroManager.Drag();
                                isTouching = false;  // Prevent duplicate drag calls.
                            }
                            break;

                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            selectedHeroManager.Drop();
                            isTouching = false;
                            break;
                    }
                }
                break;
                #endregion
        }



    }
}
