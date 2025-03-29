using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] public Vector2 scrollSpeed = new Vector2(0f, 0f);
    private RawImage rawImage;
    private Rect uvRect;

    void Start()
    {
        //Get the RawImage component
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            Debug.LogError("ScrollingUITexture: No RawImage component found!");
        }

        // Assign uvRect
        uvRect = rawImage.uvRect;
    }

    void Update()
    {
        if (rawImage == null || !gameObject.activeInHierarchy)
            return;

        // Increment the UV rect's position over time
        uvRect.position += scrollSpeed * Time.unscaledDeltaTime;

        // Apply the modified UV rect back to the RawImage
        rawImage.uvRect = uvRect;
    }
}
