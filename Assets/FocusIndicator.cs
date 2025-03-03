using Game.Behaviors;
using UnityEngine;

public class FocusIndicator : MonoBehaviour
{

    //Quick Reference Properties
    protected Vector3 tileScale => GameManager.instance.tileScale;
    protected bool hasFocusedActor => GameManager.instance.hasFocusedActor;
    protected ActorInstance focusedActor => GameManager.instance.focusedActor;
  
    //Fields
    private SpriteRenderer spriteRenderer;

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

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();     
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    public void Initialize()
    {
        scale = tileScale * 1.1f;
        spriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Assign()
    {
        spriteRenderer.enabled = hasFocusedActor;
        position = hasFocusedActor ? focusedActor.position : Position.Nowhere;
    }

    public void Clear()
    {
        spriteRenderer.enabled = false;
        position = Position.Nowhere;
    }

}
