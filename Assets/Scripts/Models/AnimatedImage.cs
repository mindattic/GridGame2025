using UnityEngine;
using UnityEngine.UI;

public class AnimatedImage : MonoBehaviour
{
    public Image image; // Drag and drop Hero Image component in Inspector

    public void SetSprite(Sprite newSprite)
    {
        image.sprite = newSprite; // Update sprite manually
    }
}
