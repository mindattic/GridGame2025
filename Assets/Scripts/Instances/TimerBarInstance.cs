using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TimerBarInstance : MonoBehaviour
{
    #region Properties
    protected DebugManager debugManager => GameManager.instance.debugManager;
    protected float tileSize => GameManager.instance.tileSize;
    protected SelectedPlayerManager selectedPlayerManager => GameManager.instance.selectedPlayerManager;
    protected float snapDistance => GameManager.instance.snapDistance;
    #endregion

    //Fields
    private bool isRunning = false;
    private const float maxDuration = 6f;
    private float timeRemaining = 6f;
    private SpriteRenderer back;
    private SpriteRenderer bar;
    private SpriteRenderer front;
    Vector3 scale = new Vector3(1f, 1f, 1f);
    Vector3 initialPosition;
    Vector3 offscreenPosition;
    float slideSpeed;
    float width;

    //Method which is used for initialization tasks that need to occur before the game starts 
    private void Awake()
    {
        back = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        bar = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
        front = gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>();
    }

    //Method which is automatically called before the first frame update  
    private void Start()
    {
        bar.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
        slideSpeed = tileSize * 0.25f;
        width = front.bounds.size.x + 2.4f;
        initialPosition = transform.position;   
        offscreenPosition = initialPosition.SubtractX(width);
    }

    void FixedUpdate()
    {
        if (debugManager.isTimerInfinite)
            return;

        if (isRunning && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            bar.transform.localScale = new Vector3(scale.x * (timeRemaining / maxDuration), scale.y, scale.z);
        }
        else
        {
            isRunning = false;

            //DEBUG: This is where the "three body problem" can occur...
            selectedPlayerManager.Unselect();
        }
    }

    public void TriggerInitialize()
    {
        StopCoroutine(Initialize());
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        //Before:
        bar.transform.localScale = new Vector3(1f, scale.y, scale.z);
        timeRemaining = maxDuration;
        isRunning = false;

        //During:
        while (transform.position != initialPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, slideSpeed);

            //Determine whether to snap
            bool isSnapDistance = Vector2.Distance(transform.position, initialPosition) <= snapDistance;
            if (isSnapDistance)
                transform.position = initialPosition;

            yield return Wait.OneTick();
        }

        //After:
        gameObject.transform.position = initialPosition;
    }

    public void Play()
    {
        isRunning = true;
    }

    public void Pause()
    {
        isRunning = false;
    }

    public void Hide()
    {
       
        StartCoroutine(SlideOut());
    }

    IEnumerator SlideOut()
    {
        //Before:

        //During:
        while (transform.position != offscreenPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, offscreenPosition, slideSpeed);

            //Determine whether to snap
            bool isSnapDistance = Vector2.Distance(transform.position, offscreenPosition) <= snapDistance;
            if (isSnapDistance)
                transform.position = offscreenPosition;

            yield return Wait.OneTick();
        }

        //After:
        gameObject.transform.position = offscreenPosition;
    }

    public void Show()
    {
        bar.enabled = true;
    }

}
