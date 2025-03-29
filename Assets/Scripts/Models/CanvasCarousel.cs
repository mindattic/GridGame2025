using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public abstract class CanvasCarousel : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform panel;

    [Header("Slide Settings")]
    public float spacing = 0f;
    public float deceleration = 3000f;
    public float maxSpeed = 3000f;
    public float scrollSpeed = 10f;
    public float dragThreshold = 15f;
    public float wrapThresholdMultiplier = 1.5f;

    protected Dictionary<string, RectTransform> items = new Dictionary<string, RectTransform>();
    protected float itemWidth;

    protected Vector2 touchStart;
    protected bool dragging = false;
    protected float velocity = 0f;
    protected float targetOffset = 0f;
    protected bool scrollingToCenter = false;
    protected bool clickAllowed = true;

    protected virtual void Awake()
    {
        // Initialization moved to manual call
    }

    public void Initialize()
    {
        if (items.Count > 0)
        {
            itemWidth = items.Values.First().rect.width;
            PositionItems();
        }
    }

    protected virtual void Update()
    {
        HandleTouch();

        if (!dragging && Mathf.Abs(velocity) > 0.1f)
        {
            float delta = velocity * Time.deltaTime;
            MoveItems(delta);

            float decel = deceleration * Time.deltaTime;
            velocity = velocity > 0 ? Mathf.Max(0, velocity - decel) : Mathf.Min(0, velocity + decel);
        }

        if (scrollingToCenter)
        {
            float move = Mathf.Lerp(0, targetOffset, 10f * Time.deltaTime);
            MoveItems(move);
            targetOffset -= move;

            if (Mathf.Abs(targetOffset) < 0.5f)
            {
                scrollingToCenter = false;
                targetOffset = 0f;
            }
        }

        WrapItems();
    }

    protected virtual void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panel, touch.position, null, out localPoint);
            if (!panel.rect.Contains(localPoint)) return;

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
                dragging = true;
                velocity = 0f;
                clickAllowed = true;
            }
            else if (touch.phase == TouchPhase.Moved && dragging)
            {
                Vector2 current = touch.position;
                float deltaX = current.x - touchStart.x;

                if (Mathf.Abs(deltaX) > dragThreshold)
                    clickAllowed = false;

                MoveItems(deltaX);
                velocity = Mathf.Clamp(deltaX / Time.deltaTime, -maxSpeed, maxSpeed);
                touchStart = current;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                dragging = false;
            }
        }
    }

    protected void MoveItems(float deltaX)
    {
        foreach (var item in items.Values)
        {
            Vector3 pos = item.anchoredPosition;
            pos.x += deltaX;
            item.anchoredPosition = pos;
        }
    }

    protected void WrapItems()
    {
        var itemList = items.Values.ToList();

        for (int i = 0; i < itemList.Count; i++)
        {
            var item = itemList[i];
            float width = GetItemWidth(item);
            float totalWidth = width + spacing;
            Vector3 pos = item.anchoredPosition;

            if (pos.x < -totalWidth * wrapThresholdMultiplier)
            {
                float rightMostX = GetRightmostX();
                pos.x = rightMostX + totalWidth;
                item.anchoredPosition = pos;
            }
            else if (pos.x > totalWidth * (itemList.Count - wrapThresholdMultiplier))
            {
                float leftMostX = GetLeftmostX();
                pos.x = leftMostX - totalWidth;
                item.anchoredPosition = pos;
            }
        }
    }

    protected float GetItemWidth(RectTransform item)
    {
        var slide = item.GetComponent<CanvasCarouselSlideInstance>();
        return slide != null ? slide.Width : itemWidth;
    }

    protected float GetLeftmostX()
    {
        float min = float.MaxValue;
        foreach (var item in items.Values)
            min = Mathf.Min(min, item.anchoredPosition.x);
        return min;
    }

    protected float GetRightmostX()
    {
        float max = float.MinValue;
        foreach (var item in items.Values)
            max = Mathf.Max(max, item.anchoredPosition.x);
        return max;
    }

    protected void PositionItems()
    {
        float x = 0;
        foreach (var item in items.Values)
        {
            var slide = item.GetComponent<CanvasCarouselSlideInstance>();
            float width = slide != null ? slide.Width : itemWidth;
            item.anchoredPosition = new Vector2(x, 0);
            x += width + spacing;
        }
    }

    public virtual void CenterOn(RectTransform target)
    {
        if (!clickAllowed) return;

        float offset = target.anchoredPosition.x;
        if (target.parent != panel)
            offset += target.parent.GetComponent<RectTransform>().anchoredPosition.x;

        targetOffset = -offset;
        scrollingToCenter = true;
    }

    public virtual void CenterOn(string targetName)
    {
        if (items.TryGetValue(targetName, out RectTransform target))
        {
            CenterOn(target);
        }
        else
        {
            Debug.LogWarning($"CenterOn failed: No item named '{targetName}' found in {panel.name}.");
        }
    }

    public virtual void CenterOn(RectTransform targetPanel, string targetName)
    {
        foreach (RectTransform child in targetPanel)
        {
            Transform found = child.Find(targetName);
            if (found != null && found is RectTransform rt)
            {
                CenterOn(rt);
                return;
            }
        }

        Debug.LogWarning($"CenterOn failed: No child named '{targetName}' found under {targetPanel.name}.");
    }

    public virtual void AddItem(string key, RectTransform rect)
    {
        if (!items.ContainsKey(key))
        {
            items.Add(key, rect);
        }
    }
}