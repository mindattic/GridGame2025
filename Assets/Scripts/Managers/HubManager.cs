using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class HubManager : MonoBehaviour
{
    [SerializeField] ResourceManager resourceManager;
    public GameObject slidePrefab;
    public RectTransform hubPanel;
    public RectTransform rosterPanel;
    public HubCarousel hubCarousel;
    public RosterCarousel rosterCarousel;

    public int hubSlideCount = 4;

    void Start()
    {

        // Hub Slides
        for (int i = 0; i < hubSlideCount; i++)
        {
            GameObject slide = Instantiate(slidePrefab, hubPanel);
            var instance = slide.GetComponent<CanvasCarouselSlideInstance>();
            instance.Key = $"{i}";
            instance.Width = 1000f;
            instance.Height = 1000f;
            slide.name = $"HubSlide_{instance.Key}";
            instance.Initialize(sprite: Resources.Load<Sprite>("Sprites/Forest"));
            hubCarousel.AddItem(instance.Key, instance.rectTransform);
        }
        hubCarousel.Initialize();


        // Roster Slides
        string[] sprites = { "Paladin", "Barbarian", "Cleric", "Ninja" };
        for (int i = 0; i < sprites.Length; i++)
        {
            GameObject slide = Instantiate(slidePrefab, rosterPanel);
            var instance = slide.GetComponent<CanvasCarouselSlideInstance>();
            instance.Key = sprites[i];
            instance.Width = 256f;
            instance.Height = 256f;
            slide.name = $"RosterSlide_{instance.Key}";

            instance.Initialize(
                sprite: Resources.Load<Sprite>($"Portraits/{sprites[i]}"), 
                onClick: () => rosterCarousel.CenterOn(instance.rectTransform));
          
            rosterCarousel.AddItem(instance.Key, instance.rectTransform);
        }
        rosterCarousel.Initialize();

    }
}
