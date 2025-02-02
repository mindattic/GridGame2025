using UnityEngine;

public class InputManager : MonoBehaviour
{
    //External properties
    protected PauseManager pauseManager => GameManager.instance.pauseManager;
    protected ActorInstance focusedActor => GameManager.instance.focusedActor;
    protected ActorInstance previousSelectedPlayer => GameManager.instance.previousSelectedPlayer;
    protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
    protected bool hasFocusedActor => focusedActor != null;
    protected bool hasSelectedPlayer => selectedPlayer != null;
    protected SelectedPlayerManager selectedPlayerManager => GameManager.instance.selectedPlayerManager;
    protected StageManager stageManager => GameManager.instance.stageManager;
    protected float dragThreshold;
    protected float tileSize => GameManager.instance.tileSize;

    //Fields
    private bool isDragging;
    //public bool IsDragging => dragStart != null;
    //public Vector3? dragStart = null;
    //[SerializeField] public float dragThreshold = 5f;

    //Method which is automatically called before the first frame update  
    void Start()
    {
        dragThreshold = tileSize / 24;
    }

    void Update()
    {
        //Check abort conditions
        if (pauseManager.IsPaused)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            selectedPlayerManager.Focus();
            isDragging = hasFocusedActor;

        }
        else if (Input.GetMouseButtonUp(0))
        {
            selectedPlayerManager.Unfocus();
            selectedPlayerManager.Unselect();
            isDragging = false;
        }

        CheckDragging();
    }

    private void CheckDragging()
    {
        //Check abort conditions
        if (!isDragging || !hasFocusedActor)
            return;

        var dragDistance = Vector3.Distance(focusedActor.position, focusedActor.currentTile.position);
        if (dragDistance > dragThreshold)
        {
            selectedPlayerManager.Select();
        }

    }


}
