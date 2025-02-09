using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    public Vector2 scrollSpeed = new Vector2(0f, 0.1f); // Scrolling speed
    private RawImage rawImage;
    private Rect uvRect;

    void Start()
    {
        // Get the RawImage component
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogError("ScrollingUITexture: No RawImage component found!");
        }

        // Initialize uvRect
        uvRect = rawImage.uvRect;
        uvRect.y = Random.Float();
    }

    void Update()
    {
        if (rawImage == null) return;

        // Increment the UV rect's position over time
        uvRect.position += scrollSpeed * Time.deltaTime;

        // Apply the modified UV rect back to the RawImage
        rawImage.uvRect = uvRect;
    }
}
