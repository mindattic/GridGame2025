using UnityEngine;

// InputManager handles hero touch input and delegates focus, drag, and drop actions
// to the SelectedPlayerManager, while also considering the game's paused state.
public class InputManager : MonoBehaviour
{
    // Quick reference properties that retrieve core game systems and actor information from GameManager.
    protected PauseManager pauseManager => GameManager.instance.pauseManager;
    protected ActorInstance focusedActor => GameManager.instance.focusedActor;
    protected ActorInstance selectedPlayer => GameManager.instance.selectedHero;
    protected bool hasSelectedActor => focusedActor != null;
    protected bool hasSelectedPlayer => selectedPlayer != null;
    protected SelectedHeroManager selectedHeroManager => GameManager.instance.selectedHeroManager;
    protected StageManager stageManager => GameManager.instance.stageManager;
    // Assuming GameManager handles conversion from input (mouse or touch) to a 3D world position.
    protected Vector3 touchPosition3D => GameManager.instance.touchPosition3D;
    protected float tileSize => GameManager.instance.tileSize;
    protected float dragThreshold => GameManager.instance.dragThreshold;


    // Fields
    private bool isTouching = false;
    private Vector3 initialTouchPosition;

    //Properties
    public bool IsDragging => isTouching && Vector3.Distance(initialTouchPosition, touchPosition3D) > dragThreshold;


    // Save is called once per frame to process hero input.
    void Update()
    {
        // Do not process input if the game is paused.
        if (pauseManager.IsPaused)
            return;

        // Check if there's at least one touch on the screen.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:

                    // Attempt to focus on an actor under the touch.
                    selectedHeroManager.Focus();

                    // Begin tracking the touch
                    isTouching = true;
                    initialTouchPosition = touchPosition3D;

                    break;

                case TouchPhase.Moved:
                    if (IsDragging)
                    {
                        selectedHeroManager.Drag();
                        isTouching = false;  // Prevent duplicate drag calls.
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    // Touch has ended, trigger drop logic.
                    selectedHeroManager.Drop();
                    isTouching = false;
                    break;
            }
        }
    }



}
