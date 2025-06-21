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


    private float popInRotY = 0f;
    private Quaternion lastPopInRot = Quaternion.identity;

    public IEnumerator PopInOut(
       float fadeDuration = 0.25f,
       float holdDuration = 0.25f,
       float rotateDuration = 0.2f)
    {
        color = ColorHelper.Transparent.White;

        yield return PopIn(rotateDuration, fadeDuration);

        for (float elapsed = 0; elapsed < holdDuration; elapsed += Time.deltaTime)
        {
            Vector3 frontAnchorPos = actor.render.front.transform.position;
            AlignPortraitWithFront(frontAnchorPos);
            yield return null;
        }

        yield return PopOut(rotateDuration, fadeDuration);
    }


    public IEnumerator PopIn(float rotateDuration = 0.2f, float fadeDuration = 0.25f)
    {
        color = ColorHelper.Transparent.White;
        Vector3 frontAnchorPos = actor.render.front.transform.position;
        Transform front = actor.render.front.transform;
        float minAlpha = Opacity.Transparent;
        float maxAlpha = Opacity.Percent90;

        float y = Random.Float(20f, 25f);
        popInRotY = Random.Float() < 0.5f ? -y : y;
        Quaternion startRot = front.rotation;
        Quaternion targetRot = Quaternion.Euler(75, popInRotY, 0);
        lastPopInRot = targetRot;

        // Rotate up
        for (float elapsed = 0; elapsed < rotateDuration; elapsed += Time.deltaTime)
        {
            float t = elapsed / rotateDuration;
            front.rotation = Quaternion.Slerp(startRot, targetRot, t);
            AlignPortraitWithFront(frontAnchorPos);
            yield return null;
        }
        front.rotation = targetRot;
        AlignPortraitWithFront(frontAnchorPos);

        // Fade in
        Color c = color;
        for (float elapsed = 0; elapsed < fadeDuration; elapsed += Time.deltaTime)
        {
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            AlignPortraitWithFront(frontAnchorPos);
            yield return null;
        }
        spriteRenderer.color = new Color(c.r, c.g, c.b, maxAlpha);
        AlignPortraitWithFront(frontAnchorPos);
    }

    public IEnumerator PopOut(float rotateDuration = 0.2f, float fadeDuration = 0.25f)
    {
        Vector3 frontAnchorPos = actor.render.front.transform.position;
        Transform front = actor.render.front.transform;
        float minAlpha = Opacity.Transparent;
        float maxAlpha = Opacity.Percent90;

        // Fade out portrait (opaque -> transparent)
        Color c = color;
        for (float elapsed = 0; elapsed < fadeDuration; elapsed += Time.deltaTime)
        {
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = Mathf.Lerp(maxAlpha, minAlpha, t);
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            AlignPortraitWithFront(frontAnchorPos);
            yield return null;
        }
        spriteRenderer.color = new Color(c.r, c.g, c.b, minAlpha);

        // Unrotate front from previous PopIn pose to default
        Quaternion startRot = lastPopInRot;
        Quaternion targetRot = Quaternion.Euler(0, 0, 0);
        for (float elapsed = 0; elapsed < rotateDuration; elapsed += Time.deltaTime)
        {
            float t = elapsed / rotateDuration;
            front.rotation = Quaternion.Slerp(startRot, targetRot, t);
            AlignPortraitWithFront(frontAnchorPos);
            yield return null;
        }
        front.rotation = targetRot;
        AlignPortraitWithFront(frontAnchorPos);

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
