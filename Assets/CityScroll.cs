using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CityScroll : MonoBehaviour
{
    public RawImage rawImage; // Assign the RawImage in the inspector
    public float scrollSpeed = 0.1f; // Speed of scrolling

    private void Start()
    {
        StartCoroutine(ScrollUV());
    }

    private IEnumerator ScrollUV()
    {
        Vector2 offset = rawImage.uvRect.position;

        while (true)
        {
            offset.x -= scrollSpeed * Time.deltaTime; // Move UVs to the left
            if (offset.x <= -1f) offset.x += 1f; // Wrap around at -1

            rawImage.uvRect = new Rect(offset, rawImage.uvRect.size);
            yield return null;
        }
    }
}

