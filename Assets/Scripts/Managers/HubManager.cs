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
            instance.Key = $"HubSlide{i}";
            instance.Width = 1000f;
            instance.Height = 1000f;
            slide.name = instance.Key;

            // Load sprite from Resources/Sprites/Forest.png (or .jpg, etc.)
            Sprite hubSprite = Resources.Load<Sprite>("Sprites/Forest");
            slide.GetComponent<Image>().sprite = hubSprite;
            
            hubCarousel.AddItem(instance.Key, instance.Rect);
        }
        hubCarousel.Initialize();


        // Roster Slides
        string[] sprites = { "Paladin", "Barbarian", "Cleric", "Ninja"};
        for (int i = 0; i < sprites.Length; i++)
        {
            GameObject slide = Instantiate(slidePrefab, rosterPanel);
            var instance = slide.GetComponent<CanvasCarouselSlideInstance>();
            instance.Key = sprites[i];
            instance.Width = 256f;
            instance.Height = 256f;
            slide.name = $"RosterSlide_{instance.Key}";

            // Load from Resources/Portraits/{spriteName}.png
            string spriteName = sprites[i % sprites.Length];
            Sprite portraitSprite = Resources.Load<Sprite>($"Portraits/{spriteName}");
            slide.GetComponent<Image>().sprite = portraitSprite;

            //Button btn = slide.GetComponent<Button>();
            //if (btn != null)
            //{
            //    string matchKey = $"HubSlide{i % hubSlideCount}";
            //    btn.onClick.AddListener(() => hubCarousel.CenterOn(matchKey));
            //}

            rosterCarousel.AddItem(instance.Key, instance.Rect);
        }
        rosterCarousel.Initialize();


    }
}
