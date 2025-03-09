using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Models;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{

    //Fields
    [SerializeField] public RectTransform canvasRect;
    [SerializeField] public RectTransform scrollViewRect;
    [SerializeField] public Transform contentPanel;
    [SerializeField] public GameObject buttonPrefab;
    private DataManager dataManager;
    private float screenWidth;
    private float screenHeight;
    private float buttonWidth;
    private float buttonHeight;
    private float spacing;

    void Start()
    {

        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();

        screenWidth = canvasRect.rect.width;
        screenHeight = canvasRect.rect.height;

        buttonWidth = 0.9f * screenWidth;
        buttonHeight = screenHeight / 16f;

        spacing = 0; //0.01f * screenHeight;

        // 2. Configure the vertical layout group spacing to 5% of screen height
        var layoutGroup = contentPanel.GetComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = spacing;

        // 3. Create each stage button
        foreach (KeyValuePair<string, StageData> stage in dataManager.Stages)
        {
            CreateStageButton(stage.Key, stage.Value, screenWidth, screenHeight);
        }
    }

    void CreateStageButton(string stageKey, StageData stageData, float screenWidth, float screenHeight)
    {
        // Instantiate the prefab as a child of the contentPanel
        GameObject instance = Instantiate(buttonPrefab, contentPanel);
        instance.name = $"Button_{stageKey}";

        // Set the button size: 90% of width, 1/16th of height
        RectTransform buttonRect = instance.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        // Set the button's click event
        Button button = instance.GetComponent<Button>();
        button.onClick.AddListener(() => OnStageSelected(stageKey));

        // Set the button text (using TextMeshProUGUI)
        TextMeshProUGUI label = instance.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = stageKey;
        }
    }

    void OnStageSelected(string stageKey)
    {
        Debug.Log("Selected stage: " + stageKey);

        //SceneManager.LoadScene("Game");
    }
}
