using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Quick Reference Properties
    protected PauseManager pauseManager => GameManager.instance.pauseManager;
    protected ActorInstance focusedActor => GameManager.instance.focusedActor;
    protected ActorInstance previousSelectedPlayer => GameManager.instance.previousSelectedPlayer;
    protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
    protected bool hasSelectedActor => focusedActor != null;
    protected bool hasSelectedPlayer => selectedPlayer != null;
    protected SelectedPlayerManager selectedPlayerManager => GameManager.instance.selectedPlayerManager;
    protected StageManager stageManager => GameManager.instance.stageManager;
    protected Vector3 mousePosition3D => GameManager.instance.mousePosition3D;
    protected float tileSize => GameManager.instance.tileSize;

    private bool isHoldingMouse = false;
    private Vector3 initialMousePosition;

    void Update()
    {
        if (pauseManager.IsPaused)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            selectedPlayerManager.Focus();
            isHoldingMouse = true;
            initialMousePosition = mousePosition3D;  // Store the initial position on click
        }

        if (isHoldingMouse && Input.GetMouseButton(0))
        {
            if (IsDragging())
            {
                selectedPlayerManager.Drag();
                isHoldingMouse = false;  // Prevent duplicate calls
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHoldingMouse = false;
            selectedPlayerManager.Drop();
        }
    }

    private bool IsDragging()
    {
        var dragThreshold = tileSize / 8;
        return Vector3.Distance(initialMousePosition, mousePosition3D) > dragThreshold;
    }
}
