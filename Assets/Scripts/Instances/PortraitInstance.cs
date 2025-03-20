using Game.Behaviors.Actor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitInstance : MonoBehaviour
{
    //Quick Reference Properties
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;

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
    public Vector3 scale
    {
        get => gameObject.transform.localScale;
        set => gameObject.transform.localScale = value;
    } 
    public Sprite sprite
    {
        get => spriteRenderer.sprite;
        set => spriteRenderer.sprite = value;
    }
    public Color color
    {
        get => spriteRenderer.color;
        set => spriteRenderer.color = value;
    }
    public int sortingOrder
    {
        set
        {
            spriteRenderer.sortingOrder = value;
        }
    }

    //Fields
    //[SerializeField] public ActorInstance actor;
    [SerializeField] public Direction direction;
    [SerializeField] public float startTime;
    [SerializeField] public Vector2 startPosition;
    [SerializeField] public AnimationCurve slide;
    public SpriteRenderer spriteRenderer;
    float minAlpha = Opacity.Transparent;
    float maxAlpha = Opacity.Opaque;
    public ActorInstance actor;

    float screenHalfHeight;
    float screenHalfWidth;

    //Method which is used for initialization tasks that need to occur before the game starts 
    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        screenHalfHeight = Camera.main.orthographicSize;
        screenHalfWidth = screenHalfHeight * Camera.main.aspect;
    }

    public IEnumerator Spawn()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        Vector2 spriteSize = spriteRenderer.bounds.size; // Get sprite's world size
        Vector3 startPos = Vector3.zero;
        Vector3 endPos = Vector3.zero;

        // Calculate spawn positions (off-screen) and destinations (on-screen)
        switch (direction)
        {
            case Direction.North:
                startPos = new Vector3(0, -(screenHalfHeight + spriteSize.y), 1);
                endPos = new Vector3(0, screenHalfHeight - spriteSize.y, 1);
                break;

            case Direction.East:
                startPos = new Vector3(-(screenHalfWidth + spriteSize.x), 0, 1);
                endPos = new Vector3(screenHalfWidth - spriteSize.x, 0, 1);
                break;

            case Direction.South:
                startPos = new Vector3(0, screenHalfHeight + spriteSize.y, 1);
                endPos = new Vector3(0, -(screenHalfHeight - spriteSize.y), 1);
                break;

            case Direction.West:
                startPos = new Vector3(screenHalfWidth + spriteSize.x, 0, 1);
                endPos = new Vector3(-(screenHalfWidth - spriteSize.x), 0, 1);
                break;
        }

        // Set starting position
        this.position = startPos;

        // Slide animation using the curve
        float elapsedTime = 0f;
        float duration = slide.keys[slide.length - 1].time; // Get animation curve duration

        while (elapsedTime < duration)
        {
            float progress = slide.Evaluate(elapsedTime / duration); // Get curve value
            this.position = Vector3.Lerp(startPos, endPos, progress);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        this.position = endPos; // Ensure final position is exact

        Destroy(this.gameObject); // Cleanup after animation
    }



    public IEnumerator Dissolve()
    {

        //Begin:
        var alpha = maxAlpha;
        spriteRenderer.color = new Color(1, 1, 1, alpha);

        //During:
        while (alpha > minAlpha)
        {
          
            //Shake
            position = startPosition;
            position += new Vector3(Random.Range(ShakeIntensity.Medium), Random.Range(ShakeIntensity.Medium), 1); //TODO: Use Shake Coroutine

            //Shrink
            transform.localScale *= 0.99f;

            //Fade
            alpha -= Increment.OnePercent;
            alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha);
            spriteRenderer.color = new Color(1, 1, 1, alpha);
            yield return Wait.OneTick();
        }

        //After:
        Destroy(this.gameObject);
    }

}
