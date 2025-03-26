using Assets.Scripts.Repositories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;

public class OverworldManager : MonoBehaviour
{
    //Fields
    private Label header;
    private RectTransform canvas2D;
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
        //Verify that game is ready to run
        if (!ProfileRepo.instance.HasProfiles)
        {
            SceneManager.LoadScene(SceneHelper.ProfileCreate);
            return;
        }

        canvas2D = GameObject.Find(ComponentHelper.Overworld.Canvas2D).GetComponent<RectTransform>();
        scrollView = GameObject.Find(ComponentHelper.Overworld.ScrollView).GetComponent<RectTransform>();
        scrollRect = GameObject.Find(ComponentHelper.Overworld.ScrollView).GetComponent<ScrollRect>();
        viewport = GameObject.Find(ComponentHelper.Overworld.Viewport).GetComponent<RectTransform>();
        content = GameObject.Find(ComponentHelper.Overworld.Content).GetComponent<RectTransform>();
        map = GameObject.Find(ComponentHelper.Overworld.Map).GetComponent<RectTransform>();
        hero = GameObject.Find(ComponentHelper.Overworld.Player).GetComponent<PlayerStageMover>();
        fade = GameObject.Find(ComponentHelper.Overworld.Fade).GetComponent<FadeInstance>();

        screenWidth = canvas2D.rect.width;
        screenHeight = canvas2D.rect.height;

        buttonWidth = 64;
        buttonHeight = 32;

        header.fontSize = screenHeight / 16f / 2;
        scrollView.sizeDelta = new Vector2(screenWidth, screenHeight);

        //scrollView.anchoredPosition = scrollView.anchoredPosition.SetY(-buttonHeight);
        FindStageButtons();
        OnCenterOnHeroClicked();
    }

    private void Start()
    {
        StartCoroutine(fade.FadeIn());
    }

    private void FindStageButtons()
    {
        GameObject[] stageObjects = GameObject.FindGameObjectsWithTag("StageButton");

        foreach (GameObject obj in stageObjects)
        {
            Button button = obj.GetComponent<Button>();
            if (button != null)
            {
                stageButtons.Add(button);
            }
        }

        Debug.Log($"Found {stageButtons.Count} stage buttons.");
    }


    //private void AddStageIcon(Vector2 position, string stageName)
    //{
    //    // Instantiate the prefab as a child of the content
    //    GameObject instance = Instantiate(stageIconPrefab, content);
    //    instance.name = $"StageIcon_{stageName}";

    //    // Set the button size: 90% of width, 1/16th of height
    //    RectTransform buttonRect = instance.GetComponent<RectTransform>();
    //    buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

    //    // Set the button's click event
    //    Button button = instance.GetComponent<Button>();
    //    button.transform.localPosition = position;
    //    button.onClick.AddListener(() => OnStageSelectButtonClicked(stageName));

    //    // Set the button textarea
    //    Label label = instance.GetComponentInChildren<Label>();
    //    label.textarea = stageName;
    //}

    public void OnStageSelectButtonClicked(Button stageButton)
    {
        hero.MoveToStage(stageButton);
    }


    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadPreviousScene()));
    }

    public void OnCenterOnHeroClicked()
    {
        CenterOnPosition(hero.transform.localPosition, 5f, 0.001f);
    }

    public void CenterOnPosition(Vector2 targetLocalPosition, float speed, float snapThreshold)
    {
        StartCoroutine(SmoothCentering(targetLocalPosition, speed, snapThreshold));
    }

    private IEnumerator SmoothCentering(Vector2 targetLocalPosition, float speed, float snapThreshold)
    {
        // GetProfile viewport and content sizes
        Vector2 viewportSize = viewport.rect.size;
        Vector2 contentSize = content.rect.size;

        // Compute offsets dynamically
        float offsetX = -viewportSize.x;
        float offsetY = viewportSize.y * 3.33333f;

        // Adjust target position by applying calculated offsets
        float adjustedX = targetLocalPosition.x + offsetX + viewportSize.x / 2;
        float adjustedY = targetLocalPosition.y + offsetY - viewportSize.y / 2; // Negative Y adjustment due to UI axis

        // Normalize values (0 = left/top, 1 = right/bottom)
        float targetX = Mathf.Clamp01(adjustedX / (contentSize.x - viewportSize.x));
        float targetY = Mathf.Clamp01(1 - (adjustedY / (contentSize.y - viewportSize.y))); // Inverting for UI coordinate system

        Vector2 targetPosition = new Vector2(targetX, targetY);

        // Smooth movement loop
        while (Vector2.Distance(scrollRect.normalizedPosition, targetPosition) > snapThreshold)
        {
            scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, targetPosition, Time.deltaTime * speed);
            yield return null; // Wait for next frame
        }

        // Snap to final position
        scrollRect.normalizedPosition = targetPosition;
    }


    //public void CenterOnPosition(Vector2 targetLocalPosition, float speed = 2f, float snapThreshold = 0.001f)
    //{
    //    StartCoroutine(SmoothCentering(targetLocalPosition, speed, snapThreshold));
    //}

    //private IEnumerator SmoothCentering(Vector2 targetLocalPosition, float speed, float snapThreshold)
    //{
    //    // GetProfile viewport and content sizes
    //    Vector2 viewportSize = viewport.rect.size;
    //    Vector2 contentSize = content.rect.size;

    //    // Compute offsets dynamically
    //    float offsetX = -viewportSize.x;
    //    float offsetY = viewportSize.y * 3.33333f;

    //    // Adjust target position by applying calculated offsets
    //    float adjustedX = targetLocalPosition.x + offsetX + viewportSize.x / 2;
    //    float adjustedY = targetLocalPosition.y + offsetY - viewportSize.y / 2; // Negative Y adjustment due to UI axis

    //    // Normalize values (0 = left/top, 1 = right/bottom)
    //    float targetX = Mathf.Clamp01(adjustedX / (contentSize.x - viewportSize.x));
    //    float targetY = Mathf.Clamp01(1 - (adjustedY / (contentSize.y - viewportSize.y))); // Inverting for UI coordinate system

    //    Vector2 targetPosition = new Vector2(targetX, targetY);

    //    // Smooth movement loop
    //    float progress = 0f;
    //    Vector2 startPosition = scrollRect.normalizedPosition;

    //    while (progress < 1f)
    //    {
    //        progress += Time.deltaTime * speed;
    //        float smoothProgress = Mathf.SmoothStep(0, 1, progress); // Smooth transition

    //        scrollRect.normalizedPosition = Vector2.Lerp(startPosition, targetPosition, smoothProgress);

    //        // If close enough, reduce speed dynamically
    //        if (Vector2.Distance(scrollRect.normalizedPosition, targetPosition) < snapThreshold)
    //        {
    //            break;
    //        }

    //        yield return null; // Wait for next frame
    //    }

    //    // Smooth final snap
    //    scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, targetPosition, 0.2f);
    //}



}
