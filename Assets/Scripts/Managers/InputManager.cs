using UnityEngine;

public class InputManager : MonoBehaviour
{
    //External properties
    protected PauseManager pauseManager => GameManager.instance.pauseManager;
    protected ActorInstance focusedActor => GameManager.instance.focusedActor;
    protected ActorInstance previousSelectedPlayer => GameManager.instance.previousSelectedPlayer;
    protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
    protected bool hasSelectedActor => focusedActor != null;
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
        //TriggerEnqueueAttacks abort conditions
        if (pauseManager.IsPaused)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            selectedPlayerManager.Select();
            isDragging = hasSelectedActor;

        }
        else if (Input.GetMouseButtonUp(0))
        {
            selectedPlayerManager.Deselect();
            selectedPlayerManager.Drop();
            isDragging = false;
        }

        CheckDragging();
    }

    private void CheckDragging()
    {
        //TriggerEnqueueAttacks abort conditions
        if (!isDragging || !hasSelectedActor)
            return;

        var dragDistance = Vector3.Distance(focusedActor.position, focusedActor.currentTile.position);
        if (dragDistance > dragThreshold)
        {
            selectedPlayerManager.Drag();
        }

    }


}
