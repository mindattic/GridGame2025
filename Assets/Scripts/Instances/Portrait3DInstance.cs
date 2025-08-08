using Game.Behaviors.Actor;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using g = Assets.Helpers.GameManagerHelper;
public class Portrait3DInstance : MonoBehaviour
{
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
        get => spriteRenderer?.sprite;
        set { if (spriteRenderer != null) spriteRenderer.sprite = value; }
    }


    public SortingGroup sortingGroup
    {
        get => this.GetComponent<SortingGroup>();
    }

    public void SetSorting(string sortingLayer, int sortingOrder = 0)
    {
        sortingGroup.sortingLayerID = SortingLayer.NameToID(sortingLayer);
        sortingGroup.sortingOrder = sortingOrder;
    }

    [SerializeField] public Direction direction;
    [SerializeField] public float startTime;
    [SerializeField] public Vector2 startPosition;
    [SerializeField] public AnimationCurve slide;
    public SpriteRenderer spriteRenderer;
    public ActorInstance actor;
    float startY;
    float startX;

    private float popInRotY = 0f;
    private Quaternion lastPopInRot = Quaternion.identity;
    private Vector3 popOutFrontRestorePos;

    private bool isBeingDestroyed = false;

    // Initialization
    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //startY = CameraManager.main.orthographicSize * 2;
        //startX = startY * CameraManager.main.aspect;

        startY = 10f;
        startX = 10f;
    }

    private void OnDestroy()
    {
        isBeingDestroyed = true;
    }

    // SlideIn by duration (lerps in a set time)
    //public IEnumerator SlideIn(float duration = 0.5f)
    //{
    //    if (isBeingDestroyed || spriteRenderer == null)
    //        yield break;

    //    Vector3 destination = Vector3.zero;

    //    switch (direction)
    //    {
    //        case AdjacentDirection.North:
    //            this.position = new Vector3(1, -startY, 1);
    //            destination = new Vector3(1, startY, 1);
    //            break;
    //        case AdjacentDirection.East:
    //            this.position = new Vector3(-startX, 1, 1);
    //            destination = new Vector3(startX, 1, 1);
    //            break;
    //        case AdjacentDirection.South:
    //            this.position = new Vector3(-1, startY, 1);
    //            destination = new Vector3(-1, -startY, 1);
    //            break;
    //        case AdjacentDirection.West:
    //            this.position = new Vector3(startX, -1, 1);
    //            destination = new Vector3(-startX, -1, 1);
    //            break;
    //    }

    //    Vector3 start = this.position;
    //    float elapsed = 0f;
    //    while (elapsed < duration)
    //    {
    //        if (isBeingDestroyed || spriteRenderer == null)
    //            yield break;

    //        float t = Mathf.Clamp01(elapsed / duration);
    //        float curveT = slide != null ? slide.Evaluate(t) : t;
    //        this.position = Vector3.Lerp(start, destination, curveT);
    //        elapsed += Time.deltaTime;
    //        yield return Wait.UntilNextFrame();
    //    }

    //    this.position = destination;
    //    Despawn();
    //}
    public IEnumerator SlideIn()
    {
        spriteRenderer.color = ColorHelper.Solid.White;
        Vector3 destination = new Vector3();

        switch (direction)
        {
            case Direction.North:
                this.position = new Vector3(1, -10, 1);
                destination = new Vector3(1, 10, 1);
                break;

            case Direction.East:
                this.position = new Vector3(-10, 1, 1);
                destination = new Vector3(10, 1, 1);
                break;

            case Direction.South:
                this.position = new Vector3(-1, 10, 1);
                destination = new Vector3(-1, -10, 1);
                break;

            case Direction.West:
                this.position = new Vector3(10, -1, 1);
                destination = new Vector3(-10, -1, 1);
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

    // PopInOut: rotates, fades in, holds, fades out, restores
    public IEnumerator PopInOut(
       float fadeDuration = 0.25f,
       float holdDuration = 0.25f,
       float rotateDuration = 0.2f)
    {
        if (isBeingDestroyed || spriteRenderer == null)
            yield break;

        Color baseColor = spriteRenderer.color;
        spriteRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        yield return PopIn(rotateDuration, fadeDuration);

        for (float elapsed = 0; elapsed < holdDuration; elapsed += Time.deltaTime)
        {
            if (isBeingDestroyed || spriteRenderer == null)
                yield break;

            Vector3 frontAnchorPos = actor.render.front.transform.position;
            AlignPortraitWithFront(frontAnchorPos);
            yield return Wait.UntilNextFrame();
        }

        yield return PopOut(rotateDuration, fadeDuration);
    }

    // PopIn: rotates and lowers Front, fades portrait in from transparent
    public IEnumerator PopIn(float rotateDuration = 0.2f, float fadeDuration = 0.25f)
    {
        if (isBeingDestroyed || spriteRenderer == null)
            yield break;

        
        Transform front = actor.render.front.transform;
        Vector3 originalFrontPos = front.position;
        float yOffset = -g.TileSize * 0.33f; // Lowered by 33%

        float y = Random.Float(20f, 25f);
        popInRotY = Random.Float() < 0.5f ? -y : y;
        Quaternion startRot = front.rotation;
        Quaternion targetRot = Quaternion.Euler(75, popInRotY, 0);
        lastPopInRot = targetRot;

        // Animate rotation and lowering position.y
        for (float elapsed = 0; elapsed < rotateDuration; elapsed += Time.deltaTime)
        {
            if (isBeingDestroyed || spriteRenderer == null)
                yield break;

            float t = elapsed / rotateDuration;
            front.rotation = Quaternion.Slerp(startRot, targetRot, t);
            Vector3 loweredPos = originalFrontPos + new Vector3(0, yOffset, 0);
            front.position = Vector3.Lerp(originalFrontPos, loweredPos, t);
            AlignPortraitWithFront(front.position);
            yield return Wait.UntilNextFrame();
        }
        front.rotation = targetRot;
        front.position = originalFrontPos + new Vector3(0, yOffset, 0);
        AlignPortraitWithFront(front.position);

        // Fade in portrait (start fully transparent)
        Color c = spriteRenderer.color;
        for (float elapsed = 0; elapsed < fadeDuration; elapsed += Time.deltaTime)
        {
            if (isBeingDestroyed || spriteRenderer == null)
                yield break;

            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = Mathf.Lerp(0, 1, t); // Fade in: 0 -> 1
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            AlignPortraitWithFront(front.position);
            yield return Wait.UntilNextFrame();
        }
        spriteRenderer.color = new Color(c.r, c.g, c.b, 1f);
        AlignPortraitWithFront(front.position);

        popOutFrontRestorePos = originalFrontPos;
    }

    // PopOut: fades out, restores Front rotation/position
    public IEnumerator PopOut(float rotateDuration = 0.2f, float fadeDuration = 0.25f)
    {
        if (isBeingDestroyed || spriteRenderer == null)
            yield break;

        Transform front = actor.render.front.transform;
        Vector3 loweredPos = front.position;
        Vector3 originalPos = popOutFrontRestorePos;

        // UpdateFill fully opaque before fade out
        Color c = spriteRenderer.color;
        spriteRenderer.color = new Color(c.r, c.g, c.b, 1f);

        // Fade out portrait
        for (float elapsed = 0; elapsed < fadeDuration; elapsed += Time.deltaTime)
        {
            if (isBeingDestroyed || spriteRenderer == null)
                yield break;

            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = Mathf.Lerp(1, 0, t); // Fade out: 1 -> 0
            spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            AlignPortraitWithFront(front.position);
            yield return Wait.UntilNextFrame();
        }
        spriteRenderer.color = new Color(c.r, c.g, c.b, 0f);
        AlignPortraitWithFront(front.position);

        // Restore rotation and position
        Quaternion startRot = front.rotation;
        Quaternion targetRot = Quaternion.Euler(0, 0, 0);
        for (float elapsed = 0; elapsed < rotateDuration; elapsed += Time.deltaTime)
        {
            if (isBeingDestroyed || spriteRenderer == null)
                yield break;

            float t = elapsed / rotateDuration;
            front.rotation = Quaternion.Slerp(startRot, targetRot, t);
            front.position = Vector3.Lerp(loweredPos, originalPos, t);
            AlignPortraitWithFront(front.position);
            yield return Wait.UntilNextFrame();
        }
        front.rotation = targetRot;
        front.position = originalPos;
        AlignPortraitWithFront(front.position);

        Despawn();
    }

    // Utility to keep portrait's feet on top of Front (even while Front rotates)
    private void AlignPortraitWithFront(Vector3 frontAnchorPos)
    {
        if (isBeingDestroyed || spriteRenderer == null)
            return;

        float halfPortraitHeight = spriteRenderer.bounds.size.y / 2f;
        transform.position = frontAnchorPos + Vector3.up * halfPortraitHeight;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    // Dissolve: fancy fade-out with shake/shrink
    public IEnumerator Dissolve()
    {
        if (isBeingDestroyed || spriteRenderer == null)
            yield break;

        float alpha = 1f;
        spriteRenderer.color = new Color(1, 1, 1, alpha);

        while (alpha > 0)
        {
            if (isBeingDestroyed || spriteRenderer == null)
                yield break;

            position = startPosition;
            position += new Vector3(Random.Range(ShakeIntensity.Medium), Random.Range(ShakeIntensity.Medium), 1);
            transform.localScale *= 0.99f;
            alpha -= Increment.Percent1;
            alpha = Mathf.Clamp(alpha, 0, 1);
            spriteRenderer.color = new Color(1, 1, 1, alpha);
            yield return Wait.UntilNextFrame();
        }

        Despawn();
    }

    private void Despawn()
    {
        if (isBeingDestroyed) return;
        isBeingDestroyed = true;
        Destroy(this.gameObject);
    }
}
