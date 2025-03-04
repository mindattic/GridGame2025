using UnityEngine;

// InputManager handles player mouse input and delegates focus, drag, and drop actions
// to the SelectedPlayerManager, while also considering the game's paused state.
public class InputManager : MonoBehaviour
{
    // Quick reference properties that retrieve core game systems and actor information from GameManager.

    // Access the PauseManager to check if the game is paused.
    protected PauseManager pauseManager => GameManager.instance.pauseManager;
    // Access the currently focused actor.
    protected ActorInstance focusedActor => GameManager.instance.focusedActor;
    // Access the previously selected player.
    protected ActorInstance previousSelectedPlayer => GameManager.instance.previousSelectedPlayer;
    // Access the currently selected player.
    protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
    // Boolean indicating if an actor is focused (non-null).
    protected bool hasSelectedActor => focusedActor != null;
    // Boolean indicating if a player is selected (non-null).
    protected bool hasSelectedPlayer => selectedPlayer != null;
    // Access the SelectedPlayerManager to manage actor selection, dragging, and dropping.
    protected SelectedPlayerManager selectedPlayerManager => GameManager.instance.selectedPlayerManager;
    // Access the StageManager for stage-related events (if needed).
    protected StageManager stageManager => GameManager.instance.stageManager;
    // Get the current mouse position in 3D space.
    protected Vector3 mousePosition3D => GameManager.instance.mousePosition3D;
    // Retrieve the tile size used in the game grid for calculating drag thresholds.
    protected float tileSize => GameManager.instance.tileSize;

    // Internal flag to track if the mouse button is currently held down.
    private bool isHoldingMouse = false;
    // Stores the initial mouse position when the mouse button was first pressed.
    private Vector3 initialMousePosition;

    // Update is called once per frame to process player input.
    void Update()
    {
        // Do not process input if the game is paused.
        if (pauseManager.IsPaused)
            return;

        // Check if the left mouse button was pressed down this frame.
        if (Input.GetMouseButtonDown(0))
        {
            // Attempt to focus on an actor under the mouse.
            selectedPlayerManager.Focus();
            // Begin tracking the drag operation.
            isHoldingMouse = true;
            // Record the initial mouse position to later determine if the user is dragging.
            initialMousePosition = mousePosition3D;  // Store the initial position on click.
        }

        // If the mouse is held down, continuously check if a drag is in progress.
        if (isHoldingMouse && Input.GetMouseButton(0))
        {
            // If the mouse has moved past a defined threshold, interpret it as a drag.
            if (IsDragging())
            {
                // Start the dragging operation which transitions the actor from focus to movement.
                selectedPlayerManager.Drag();
                // Reset the holding flag to avoid multiple drag calls.
                isHoldingMouse = false;  // Prevent duplicate calls.
            }
        }

        // Check if the left mouse button was released.
        if (Input.GetMouseButtonUp(0))
        {
            // Reset the holding flag.
            isHoldingMouse = false;
            // Trigger the drop logic to finalize any movement.
            selectedPlayerManager.Drop();
        }
    }

    // IsDragging checks whether the mouse has moved far enough from its initial position
    // to be considered a drag. The threshold is set as a fraction of the tile size.
    private bool IsDragging()
    {
        var dragThreshold = tileSize / 8; // Define the drag threshold relative to tile size.
        // Calculate the distance between the initial mouse position and the current mouse position.
        return Vector3.Distance(initialMousePosition, mousePosition3D) > dragThreshold;
    }
}
