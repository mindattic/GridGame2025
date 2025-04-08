//using Assets.Scripts.Models;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.RegularExpressions;
//using UnityEngine;
//using UnityEngine.UI;

//public class RosterCarousel: MonoBehaviour 
//{
//    [Header("UI References")]
//    public RectTransform panel;

//    [Header("Slide Settings")]
//    public float spacing = 0f;
//    public float deceleration = 3000f;
//    public float maxSpeed = 3000f;
//    public float scrollSpeed = 10f;
//    public float dragThreshold = 15f;
//    public float wrapThresholdMultiplier = 1.5f;

//    protected Dictionary<string, RosterSlideInstance> slides = new Dictionary<string, RosterSlideInstance>();
//    protected float itemWidth;

//    protected Vector2 touchStart;
//    protected bool dragging = false;
//    protected float velocity = 0f;
//    protected float targetOffset = 0f;
//    protected bool scrollingToCenter = false;
//    protected bool clickAllowed = true;

//    protected virtual void Awake()
//    {
//        // Initialization moved to manual call
//    }

//    public void Initialize()
//    {
//        if (slides.Count > 0)
//        {
//            itemWidth = slides.Values.First().rectTransform.rect.width;
//            RepositionSlides();
//        }
//    }

//    protected virtual void Update()
//    {
//        HandleTouch();

//        if (!dragging && Mathf.Abs(velocity) > 0.1f)
//        {
//            float delta = velocity * Time.deltaTime;
//            MoveSlides(delta);

//            float decel = deceleration * Time.deltaTime;
//            velocity = velocity > 0 ? Mathf.Max(0, velocity - decel) : Mathf.Min(0, velocity + decel);
//        }

//        if (scrollingToCenter)
//        {
//            float move = Mathf.Lerp(0, targetOffset, 10f * Time.deltaTime);
//            MoveSlides(move);
//            targetOffset -= move;

//            if (Mathf.Abs(targetOffset) < 0.5f)
//            {
//                scrollingToCenter = false;
//                targetOffset = 0f;
//            }
//        }

//        WrapSlides();
//    }

//    protected virtual void HandleTouch()
//    {
//        if (Input.touchCount > 0)
//        {
//            Touch touch = Input.GetTouch(0);

//            Vector2 localPoint;
//            RectTransformUtility.ScreenPointToLocalPointInRectangle(panel, touch.position, null, out localPoint);
//            if (!panel.rect.Contains(localPoint)) return;

//            if (touch.phase == TouchPhase.Began)
//            {
//                touchStart = touch.position;
//                dragging = true;
//                velocity = 0f;
//                clickAllowed = true;
//            }
//            else if (touch.phase == TouchPhase.Moved && dragging)
//            {
//                Vector2 current = touch.position;
//                float deltaX = current.x - touchStart.x;

//                if (Mathf.Abs(deltaX) > dragThreshold)
//                    clickAllowed = false;

//                MoveSlides(deltaX);
//                velocity = Mathf.Clamp(deltaX / Time.deltaTime, -maxSpeed, maxSpeed);
//                touchStart = current;
//            }
//            else if (touch.phase == TouchPhase.Ended)
//            {
//                dragging = false;
//            }
//        }
//    }

//    protected void MoveSlides(float deltaX)
//    {
//        foreach (var item in slides.Values)
//        {
//            Vector3 pos = item.rectTransform.anchoredPosition;
//            pos.x += deltaX;
//            item.rectTransform.anchoredPosition = pos;
//        }
//    }

//    protected void WrapSlides()
//    {
//        var itemList = slides.Values.ToList();

//        for (int i = 0; i < itemList.Count; i++)
//        {
//            var item = itemList[i];
//            float width = GetItemWidth(item.rectTransform);
//            float totalWidth = width + spacing;
//            Vector3 pos = item.rectTransform.anchoredPosition;

//            if (pos.x < -totalWidth * wrapThresholdMultiplier)
//            {
//                float rightMostX = GetRightmostX();
//                pos.x = rightMostX + totalWidth;
//                item.rectTransform.anchoredPosition = pos;
//            }
//            else if (pos.x > totalWidth * (itemList.Count - wrapThresholdMultiplier))
//            {
//                float leftMostX = GetLeftmostX();
//                pos.x = leftMostX - totalWidth;
//                item.rectTransform.anchoredPosition = pos;
//            }
//        }
//    }

//    protected float GetItemWidth(RectTransform item)
//    {
//        var slide = item.GetComponent<RosterSlideInstance>();
//        return slide != null ? slide.Width : itemWidth;
//    }

//    protected float GetLeftmostX()
//    {
//        float min = float.MaxValue;
//        foreach (var item in slides.Values)
//            min = Mathf.Min(min, item.rectTransform.anchoredPosition.x);
//        return min;
//    }

//    protected float GetRightmostX()
//    {
//        float max = float.MinValue;
//        foreach (var item in slides.Values)
//            max = Mathf.Max(max, item.rectTransform.anchoredPosition.x);
//        return max;
//    }

//    protected void RepositionSlides()
//    {
//        float x = 0;
//        foreach (var item in slides.Values)
//        {
//            var slide = item.GetComponent<RosterSlideInstance>();
//            float width = slide != null ? slide.Width : itemWidth;
//            item.rectTransform.anchoredPosition = new Vector2(x, 0);
//            x += width + spacing;
//        }
//    }

//    public virtual void CenterOn(RosterSlideInstance slide)
//    {
//        if (!clickAllowed) return;

//        float offset = slide.rectTransform.anchoredPosition.x;
//        if (slide.rectTransform.parent != panel)
//            offset += slide.rectTransform.parent.GetComponent<RectTransform>().anchoredPosition.x;

//        targetOffset = -offset;
//        scrollingToCenter = true;

//    }

//    public virtual void CenterOn(string targetName)
//    {
//        if (slides.TryGetValue(targetName, out RosterSlideInstance slide))
//        {
//            CenterOn(slide);
//        }
//        else
//        {
//            Debug.LogWarning($"CenterOn failed: No item named '{targetName}' found in {panel.name}.");
//        }
//    }

//    public virtual void AddItem(RosterSlideInstance slide)
//    {
//        if (!slides.ContainsKey(slide.Key))
//        {
//            slides.Add(slide.Key, slide);
//        }
//    }
//}

