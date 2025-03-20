using Game.Behaviors.Actor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ComponentHelper.Game;

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

    public bool slideFinished = false;

    public IEnumerator SlideIn()
    {
        // Calculate the start and destination positions.
        Vector3 startPos = position;
        Vector3 destination = Vector3.zero;
        switch (direction)
        {
            case Direction.North:
                startPos = new Vector3(1, -10, 1);
                destination = new Vector3(1, 10, 1);
                break;
            case Direction.East:
                startPos = new Vector3(-10, 1, 1);
                destination = new Vector3(10, 1, 1);
                break;
            case Direction.South:
                startPos = new Vector3(-1, 10, 1);
                destination = new Vector3(-1, -10, 1);
                break;
            case Direction.West:
                startPos = new Vector3(10, -1, 1);
                destination = new Vector3(-10, -1, 1);
                break;
        }

        // Set the initial position.
        position = startPos;

        // Duration of the slide (in seconds)
        float duration = slide.length; // or set a fixed value like 1f
        float elapsed = 0f;

        // Animate over the duration using the AnimationCurve to smooth the interpolation.
        while (elapsed < duration)
        {
            // Normalized progress between 0 and 1.
            float t = elapsed / duration;
            // Evaluate t on the curve.
            float curveT = slide.Evaluate(t);
            // Lerp between start and destination.
            position = Vector3.Lerp(startPos, destination, curveT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure we reach the destination.
        position = destination;
        slideFinished = true;

        // Optionally, delay a frame before destroying if you need to see the final position.
        yield return null;
        Destroy(this.gameObject);
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
