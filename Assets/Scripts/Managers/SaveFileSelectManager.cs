using Assets.Scripts.Models;
using Assets.Scripts.Store;
using Game.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;

public class SaveFileSelectManager : MonoBehaviour
{
    // Fields
    [SerializeField] public GameObject buttonPrefab;
    private RectTransform canvas2D;
    private Label header;
    private RectTransform scrollView;
    private Transform content;
    private VerticalLayoutGroup verticalLayoutGroup;
    private float screenWidth;
    private float screenHeight;
    private float buttonWidth;
    private float buttonHeight;
    private float spacing;
    private Fade fade;

    private void Awake()
    {
        // Use appropriate ComponentHelper names or adjust as needed
        canvas2D = GameObject.Find(ComponentHelper.StageSelect.Canvas2D).GetComponent<RectTransform>() ?? throw new UnityException("Canvas2D is null");
        header = GameObject.Find(ComponentHelper.StageSelect.Header).GetComponent<Label>() ?? throw new UnityException("Label is null");
        scrollView = GameObject.Find(ComponentHelper.StageSelect.ScrollView).GetComponent<RectTransform>() ?? throw new UnityException("ScrollView is null");
        content = GameObject.Find(ComponentHelper.StageSelect.Content).GetComponent<Transform>() ?? throw new UnityException("Content is null");
        verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>() ?? throw new UnityException("VerticalLayoutGroup is null");
        fade = GameObject.Find(ComponentHelper.StageSelect.Fade).GetComponent<Fade>() ?? throw new UnityException("Fade is null");

        screenWidth = canvas2D.rect.width;
        screenHeight = canvas2D.rect.height;

        buttonWidth = 0.9f * screenWidth;
        buttonHeight = screenHeight / 16f;

        header.fontSize = buttonHeight / 2;
        scrollView.anchoredPosition = scrollView.anchoredPosition.SetY(-buttonHeight);

        spacing = 0.01f * screenHeight;
        verticalLayoutGroup.spacing = spacing;
    }

    private void Start()
    {
        Reload();
        StartCoroutine(fade.FadeIn());
    }

    private void Reload()
    {
        //Clear existing content
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        //Validate a current profile exists
        Profile currentProfile = ProfileStore.instance.CurrentProfile;
        if (currentProfile == null)
        {
            Debug.LogError("No current profile selected.");
            return;
        }

        //Retrieve all save files in profile folder
        var saveFiles = Directory.GetFiles(currentProfile.Folder, "*.json").OrderByDescending(x => x).ToList();

        //Add each save fileName as a button 
        foreach (var fileName in saveFiles)
        {
            AddLoadSaveFileButton(fileName);
        }
    }

    public void AddLoadSaveFileButton(string fileName)
    {
        var saveName = Path.GetFileNameWithoutExtension(fileName);

        //Instantiate the prefab as a child of `content`
        GameObject instance = Instantiate(buttonPrefab, content);
        instance.name = $"SaveFile_{saveName}";

        //Set the button size: 90% of width, 1/16th of height
        RectTransform buttonRect = instance.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        //Set the button click event
        Button button = instance.GetComponent<Button>();
        button.onClick.AddListener(() => OnLoadSaveFileButtonClicked(fileName));

        //Set the button text
        Label label = instance.GetComponentInChildren<Label>();
        label.text = saveName;
    }

    private void OnLoadSaveFileButtonClicked(string filePath)
    {
        try
        {
            // Read and deserialize the selected save fileName
            string json = File.ReadAllText(filePath);
            SaveFile selectedSave = JsonConvert.DeserializeObject<SaveFile>(json);
            if (selectedSave != null)
            {
                // Set the selected save as the active game state.
                // Here we simply replace the current SaveFiles with the selected one.
                ProfileStore.instance.CurrentProfile.CurrentSave = selectedSave;

                // Proceed to load the game scene using the selected save.
                StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.Game)));
            }
            else
            {
                Debug.LogError("Failed to deserialize the selected save fileName.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading save fileName {filePath}: {ex.Message}");
        }
    }

    private void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneStore.instance.LoadScene(SceneHelper.Title)));
    }
}
