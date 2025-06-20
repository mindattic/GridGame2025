using Game.Behaviors.Actor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ComponentHelper.Game;

public class PortraitInstance : MonoBehaviour
{
    //Quick Reference Properties
    

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

    public ActorInstance actor;
    float screenHeight;
    float screenWidth;

    //Method which is used for initialization tasks that need to occur before the game starts 
    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        screenHeight = Camera.main.orthographicSize * 2;
        screenWidth = screenHeight * Camera.main.aspect;
    }

    public bool slideFinished = false;

    public IEnumerator SlideIn()
    {

        float minAlpha = Opacity.Transparent;
        float maxAlpha = Opacity.Opaque;
        Vector3 destination = new Vector3();

        switch (direction)
        {
            case Direction.North:
                this.position = new Vector3(1, -screenHeight, 1);
                destination = new Vector3(1, screenHeight, 1);
                break;

            case Direction.East:
                this.position = new Vector3(-screenWidth, 1, 1);
                destination = new Vector3(screenWidth, 1, 1);
                break;

            case Direction.South:
                this.position = new Vector3(-1, screenHeight, 1);
                destination = new Vector3(-1, -screenHeight, 1);
                break;

            case Direction.West:
                this.position = new Vector3(screenWidth, -1, 1);
                destination = new Vector3(-screenWidth, -1, 1);
                break;
        }


        while (!position.Equals(destination))
        {
            switch (direction)
            {
                case Direction.North:
                case Direction.South:
                    this.position = new Vector3(
                        destination.x,
                        destination.y * slide.Evaluate((Time.time - startTime) % slide.length),
                        destination.z);
                    break;

                case Direction.East:
                case Direction.West:
                    this.position = new Vector3(
                          destination.x * slide.Evaluate((Time.time - startTime) % slide.length),
                          destination.y,
                          destination.z);
                    break;
            }

            yield return Wait.UntilNextFrame();
        }

    }

    public IEnumerator PopIn(
    float fadeDuration = 0.25f,
    float holdDuration = 0.25f,
    float rotateDuration = 0.2f)
    {
        // Transparency bounds
        float minAlpha = Opacity.Transparent;
        float maxAlpha = Opacity.Opaque;

        Vector3 frontAnchorPos = actor.render.front.transform.position;

        // Set initial alpha
        Color c = spriteRenderer.color;
        c.a = minAlpha;
        spriteRenderer.color = c;

        // Reference to the actor's "Front"
        Transform front = actor.render.front.transform;

        // Determine random Y rotation: -20 or +20
        float randomY = Random.Float() < 0.5f ? -20f : 20f;

        // 1. ROTATE FRONT TO 75 ON X AND ±20 ON Y
        float rotateElapsed = 0f;
        Quaternion startRot = front.rotation;
        Quaternion targetRot = Quaternion.Euler(75, randomY, 0);
        while (rotateElapsed < rotateDuration)
        {
            float t = rotateElapsed / rotateDuration;
            front.rotation = Quaternion.Slerp(startRot, targetRot, t);
            rotateElapsed += Time.deltaTime;
            AlignPortraitWithFront(frontAnchorPos);
            yield return null;
        }
        front.rotation = targetRot;
        AlignPortraitWithFront(frontAnchorPos);

        // 2. FADE IN
        float fadeElapsed = 0f;
        while (fadeElapsed < fadeDuration)
        {
            float t = Mathf.Clamp01(fadeElapsed / fadeDuration);
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            AlignPortraitWithFront(frontAnchorPos);
            fadeElapsed += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = new Color(c.r, c.g, c.b, maxAlpha);
        AlignPortraitWithFront(frontAnchorPos);

        // 3. HOLD
        if (holdDuration > 0f)
        {
            float holdElapsed = 0f;
            while (holdElapsed < holdDuration)
            {
                AlignPortraitWithFront(frontAnchorPos);
                holdElapsed += Time.deltaTime;
                yield return null;
            }
        }

        // 4. FADE OUT
        fadeElapsed = 0f;
        while (fadeElapsed < fadeDuration)
        {
            float t = Mathf.Clamp01(fadeElapsed / fadeDuration);
            float alpha = Mathf.Lerp(maxAlpha, minAlpha, t);
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            AlignPortraitWithFront(frontAnchorPos);
            fadeElapsed += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = new Color(c.r, c.g, c.b, minAlpha);

        // 5. ROTATE FRONT BACK TO (0, 0, 0)
        rotateElapsed = 0f;
        startRot = front.rotation;
        targetRot = Quaternion.Euler(0, 0, 0);
        while (rotateElapsed < rotateDuration)
        {
            float t = rotateElapsed / rotateDuration;
            front.rotation = Quaternion.Slerp(startRot, targetRot, t);
            rotateElapsed += Time.deltaTime;
            AlignPortraitWithFront(frontAnchorPos);
            yield return null;
        }
        front.rotation = targetRot;
        AlignPortraitWithFront(frontAnchorPos);

        // 6. Optionally destroy the portrait
        Destroy(gameObject);
    }

    // Utility to keep portrait's feet on top of Front (even while Front rotates)
    private void AlignPortraitWithFront(Vector3 frontAnchorPos)
    {
        float halfPortraitHeight = spriteRenderer.bounds.size.y / 2f;
        // Only use world up, not rotated up
        transform.position = frontAnchorPos + Vector3.up * halfPortraitHeight;
        // Keep Z flat if needed
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }


    public IEnumerator Dissolve()
    {
        float minAlpha = Opacity.Transparent;
        float maxAlpha = Opacity.Opaque;

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

            //FadeInstance
            alpha -= Increment.OnePercent;
            alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha);
            spriteRenderer.color = new Color(1, 1, 1, alpha);
            yield return Wait.UntilNextFrame();
        }

        //After:
        Destroy(this.gameObject);
    }


}
