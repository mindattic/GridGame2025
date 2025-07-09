using Game.Behaviors;
using UnityEngine;
using game = GameManagerHelper;

// TargetIndicator is a MonoBehaviour responsible for displaying an indicator
// that highlights the currently targeted actor (if any) on the game board.
public class TargetIndicator : MonoBehaviour
{
    #region Game Properies
    protected Vector3 tileScale => GameManager.instance.tileScale;
    protected bool hasTargetActor => GameManager.instance.hasTargetActor;
    protected ActorInstance targetActor => GameManager.instance.targetActor;
    #endregion

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
        scale = tileScale * 1.1f;
        spriteRenderer.enabled = false;
    }

    // Activates and positions the TargetIndicator based on whether a focused actor exists.
    public void Assign()
    {
        spriteRenderer.enabled = hasTargetActor;
        position = hasTargetActor ? targetActor.position : PositionHelper.Nowhere;
    }

    // Clear deactivates the TargetIndicator and moves it off-screen.
    public void Clear()
    {
        spriteRenderer.enabled = false;
        position = PositionHelper.Nowhere;
    }
}
