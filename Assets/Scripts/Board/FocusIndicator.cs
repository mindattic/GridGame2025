using Game.Behaviors;
using UnityEngine;
using g = GameManagerHelper;

// FocusIndicator is a MonoBehaviour responsible for displaying an indicator
// that highlights the currently focused actor (if any) on the game board.
public class FocusIndicator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    #region Instance Properties
    public Transform parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true);
    }
    public Vector3 position
    {
        get => gameObject.transform.position;
        set => gameObject.transform.position = value;
    }
    public Quaternion rotation
    {
        get => gameObject.transform.rotation;
        set => gameObject.transform.rotation = value;
    }
    public Vector3 scale
    {
        get => gameObject.transform.localScale;
        set => gameObject.transform.localScale = value;
    }
    #endregion

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize()
    {
        // Scale the indicator based on the tile scale, making it 10% larger than the standard tile.
        scale = g.TileScale * 1.1f;
        // Hide the sprite so that it is not visible until explicitly assigned.
        spriteRenderer.enabled = false;
    }

    // SelectProfile activates and positions the FocusIndicator based on whether a focused actor exists.
    public void Assign()
    {
        // Enable the sprite only if there is a focused actor.
        spriteRenderer.enabled = g.HasFocusedActor;
        // PositionHelper the indicator on the focused actor's position if available;
        // otherwise, place it at a designated 'Nowhere' location.
        position = g.HasFocusedActor ? g.FocusedActor.position : PositionHelper.Nowhere;
    }

    // Clear deactivates the FocusIndicator and moves it off-screen.
    public void Clear()
    {
        // Disable the indicator's sprite.
        spriteRenderer.enabled = false;
        // Assign its position to 'Nowhere', effectively removing it from the board.
        position = PositionHelper.Nowhere;
    }
}
