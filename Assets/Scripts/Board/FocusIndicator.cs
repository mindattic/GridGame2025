using Game.Behaviors;
using UnityEngine;

// FocusIndicator is a MonoBehaviour responsible for displaying an indicator
// that highlights the currently focused actor (if any) on the game board.
public class FocusIndicator : MonoBehaviour
{
    // Quick reference properties accessing global game settings through the GameManager singleton.
    // These properties provide shortcuts to important values such as the tile scale and the current focused actor.

    // Retrieves the global tile scale used in the game.
    protected Vector3 tileScale => GameManager.instance.tileScale;

    // Checks whether a focused actor is currently assigned.
    protected bool hasFocusedActor => GameManager.instance.hasFocusedActor;

    // Retrieves the currently focused actor from the GameManager.
    protected ActorInstance focusedActor => GameManager.instance.focusedActor;

    // Private field for the SpriteRenderer component used to visually display the indicator.
    private SpriteRenderer spriteRenderer;

    // Public property to get or set the parent Transform of this game object.
    // Setting the parent with 'worldPositionStays' set to true preserves the object's world position.
    public Transform parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true);
    }

    // Public property to get or set the position of this game object in the world.
    public Vector3 position
    {
        get => gameObject.transform.position;
        set => gameObject.transform.position = value;
    }

    // Public property to get or set the rotation of this game object.
    public Quaternion rotation
    {
        get => gameObject.transform.rotation;
        set => gameObject.transform.rotation = value;
    }

    // Public property to get or set the local scale of this game object.
    public Vector3 scale
    {
        get => gameObject.transform.localScale;
        set => gameObject.transform.localScale = value;
    }

    // Awake is called when the script instance is being loaded.
    // Here, we cache the SpriteRenderer component for later use.
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update.
    // This is reserved for initialization logic that might be needed before the Update loop.
    void Start()
    {
        // Currently no initialization is needed at start.
    }

    // Initialize sets up the FocusIndicator by scaling it slightly larger than a tile and hiding it initially.
    public void Initialize()
    {
        // Scale the indicator based on the tile scale, making it 10% larger than the standard tile.
        scale = tileScale * 1.1f;
        // Hide the sprite so that it is not visible until explicitly assigned.
        spriteRenderer.enabled = false;
    }

    // Update is called once per frame.
    // Currently, no per-frame logic is required for the FocusIndicator.
    void Update()
    {
    }

    // Assign activates and positions the FocusIndicator based on whether a focused actor exists.
    public void Assign()
    {
        // Enable the sprite only if there is a focused actor.
        spriteRenderer.enabled = hasFocusedActor;
        // Position the indicator on the focused actor's position if available;
        // otherwise, place it at a designated 'Nowhere' location.
        position = hasFocusedActor ? focusedActor.position : Position.Nowhere;
    }

    // Clear deactivates the FocusIndicator and moves it off-screen.
    public void Clear()
    {
        // Disable the indicator's sprite.
        spriteRenderer.enabled = false;
        // Set its position to 'Nowhere', effectively removing it from the board.
        position = Position.Nowhere;
    }
}
