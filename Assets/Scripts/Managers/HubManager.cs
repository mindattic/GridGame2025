using Assets.Scripts.Repositories;
using System.Collections;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    public GameObject slidePrefab;
    public RectTransform hubPanel;
    public RectTransform rosterPanel;
    public HubCarousel hubCarousel;
    public RosterCarousel rosterCarousel;
    private FadeInstance fade;

    public int hubSlideCount = 4;

    public void Awake()
    {
        fade = GameObject.Find(ComponentHelper.Hub.Fade).GetComponent<FadeInstance>();

        LoadHubSlides();
        LoadRosterSlides();
    }

    private void LoadHubSlides()
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
    }

    private void LoadRosterSlides()
    {
        // Roster Slides
        string[] sprites = { "Barbarian", "Cleric", "GreenNinja", "Paladin", "Pugilist", "RedNinja", "Ronin", "Thief", "Vampire" };
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

    void Start()
    {
      
        StartCoroutine(fade.FadeIn());
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadPreviousScene()));
    }

}
