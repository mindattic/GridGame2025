using Assets.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;

public class OverworldManager : MonoBehaviour
{
    // Fields
    private Label header;
    private RectTransform scrollView;
    private ScrollRect scrollRect;
    private RectTransform viewport;
    private RectTransform content;
    private RectTransform map;
    private PlayerStageMover hero;
    private float screenWidth;
    private float screenHeight;
    private float buttonWidth;
    private float buttonHeight;
    private FadeInstance fade;
    private List<Button> stageButtons = new List<Button>();

    private void Awake()
    {
        // Verify that game is ready to run
        if (!ProfileRepo.HasProfiles())
            return;

        scrollView = GameObject.Find(GameObjectHelper.Overworld.ScrollView).GetComponent<RectTransform>();
        scrollRect = GameObject.Find(GameObjectHelper.Overworld.ScrollView).GetComponent<ScrollRect>();
        viewport = GameObject.Find(GameObjectHelper.Overworld.Viewport).GetComponent<RectTransform>();
        content = GameObject.Find(GameObjectHelper.Overworld.Content).GetComponent<RectTransform>();
        map = GameObject.Find(GameObjectHelper.Overworld.Map).GetComponent<RectTransform>();
        hero = GameObject.Find(GameObjectHelper.Overworld.Hero).GetComponent<PlayerStageMover>();
        fade = GameObject.Find(GameObjectHelper.Overworld.Fade).GetComponent<FadeInstance>();

        FindStageButtons();
        OnCenterOnHeroClicked();
    }

    private void Start()
    {
        StartCoroutine(fade.FadeInRoutine());
    }

    /// <summary>
    /// Finds all stage buttons in the scene without using tags.
    /// </summary>
    private void FindStageButtons()
    {
        stageButtons.Clear();
        var foundButtons = GameObject.FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (var button in foundButtons)
        {
            if (button.name.Contains("StageButton"))
            {
                stageButtons.Add(button);
            }
        }

        Debug.Log($"Found {stageButtons.Count} stage buttons.");
    }

    public void OnStageSelectButtonClicked(Button stageButton)
    {
        hero.MoveToStage(stageButton);
    }

    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOutRoutine(SceneHelper.LoadPreviousScene()));
    }

    public void OnCenterOnHeroClicked()
    {
        CenterOnPosition(hero.transform.localPosition, 5f, 0.001f);
    }

    public void CenterOnPosition(Vector2 targetLocalPosition, float speed, float snapThreshold)
    {
        StartCoroutine(SmoothCenteringRoutine(targetLocalPosition, speed, snapThreshold));
    }

    private IEnumerator SmoothCenteringRoutine(Vector2 targetLocalPosition, float speed, float snapThreshold)
    {
        Vector2 viewportSize = viewport.rect.size;
        Vector2 contentSize = content.rect.size;

        float offsetX = -viewportSize.x;
        float offsetY = viewportSize.y * 3.33333f;

        float adjustedX = targetLocalPosition.x + offsetX + viewportSize.x / 2;
        float adjustedY = targetLocalPosition.y + offsetY - viewportSize.y / 2;

        float targetX = Mathf.Clamp01(adjustedX / (contentSize.x - viewportSize.x));
        float targetY = Mathf.Clamp01(1 - (adjustedY / (contentSize.y - viewportSize.y)));

        Vector2 targetPosition = new Vector2(targetX, targetY);

        while (Vector2.Distance(scrollRect.normalizedPosition, targetPosition) > snapThreshold)
        {
            scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, targetPosition, Time.deltaTime * speed);
            yield return Wait.None();
        }

        scrollRect.normalizedPosition = targetPosition;
    }
}
