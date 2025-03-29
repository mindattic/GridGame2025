using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HubManager : MonoBehaviour
{
    private RectTransform panel; // Assign the Panel
    private List<RectTransform> slides; // Assign 6 slides in order
    private float slideWidth;
    private float spacing = 0f;
    private float swipeThreshold = 100f;

    private Vector2 touchStart;
    private bool dragging = false;

    private float velocity = 0f;
    private float deceleration = 2000f; // pixels/second²
    private float maxSpeed = 3000f;

    private void Awake()
    {
        panel = GameObject.Find(ComponentHelper.Hub.Panel).GetComponent<RectTransform>();

        //Auto-populate slides from all direct children of the panel
        slides = new List<RectTransform>();
        foreach (Transform child in panel)
        {
            RectTransform rt = child as RectTransform;
            if (rt != null)
                slides.Add(rt);
        }

        slideWidth = slides[0].rect.width;
        PositionSlides();
    }


    private void Start()
    {
        slideWidth = slides[0].rect.width;
        PositionSlides();
    }

    private void Update()
    {
        HandleTouch();

        if (!dragging && Mathf.Abs(velocity) > 0.1f)
        {
            float delta = velocity * Time.deltaTime;
            MoveSlides(delta);

            float decel = deceleration * Time.deltaTime;
            if (velocity > 0)
                velocity = Mathf.Max(0, velocity - decel);
            else
                velocity = Mathf.Min(0, velocity + decel);
        }

        WrapSlides();
    }

    private void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
                dragging = true;
                velocity = 0f;
            }
            else if (touch.phase == TouchPhase.Moved && dragging)
            {
                Vector2 current = touch.position;
                float deltaX = current.x - touchStart.x;
                MoveSlides(deltaX);
                velocity = Mathf.Clamp(deltaX / Time.deltaTime, -maxSpeed, maxSpeed);
                touchStart = current;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                dragging = false;
            }
        }

    }

    private void MoveSlides(float deltaX)
    {
        foreach (var slide in slides)
        {
            Vector3 pos = slide.anchoredPosition;
            pos.x += deltaX;
            slide.anchoredPosition = pos;
        }
    }

    private void WrapSlides()
    {
        float totalWidth = slideWidth + spacing;

        for (int i = 0; i < slides.Count; i++)
        {
            Vector3 pos = slides[i].anchoredPosition;

            if (pos.x < -totalWidth * 1.5f)
            {
                float rightMostX = GetRightmostX();
                pos.x = rightMostX + totalWidth;
                slides[i].anchoredPosition = pos;
            }
            else if (pos.x > totalWidth * (slides.Count - 1.5f))
            {
                float leftMostX = GetLeftmostX();
                pos.x = leftMostX - totalWidth;
                slides[i].anchoredPosition = pos;
            }
        }
    }

    private float GetLeftmostX()
    {
        float min = float.MaxValue;
        foreach (var slide in slides)
            min = Mathf.Min(min, slide.anchoredPosition.x);
        return min;
    }

    private float GetRightmostX()
    {
        float max = float.MinValue;
        foreach (var slide in slides)
            max = Mathf.Max(max, slide.anchoredPosition.x);
        return max;
    }

    private void PositionSlides()
    {
        for (int i = 0; i < slides.Count; i++)
        {
            slides[i].anchoredPosition = new Vector2(i * (slideWidth + spacing), 0);
        }
    }
}
