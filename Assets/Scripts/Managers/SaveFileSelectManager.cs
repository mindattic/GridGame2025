using Assets.Scripts.Models;
using Assets.Scripts.Repositories;
using Game.Models;
using Game.Models.Profile;
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
        header = GameObject.Find(ComponentHelper.StageSelect.Header).GetComponent<Label>() ?? throw new UnityException("Header is null");
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
        if (!ProfileRepo.instance.HasCurrentProfile)
        {
            Debug.LogError("No current profile selected.");
            return;
        }

        //Retrieve all save files in profile folder
        string savesPath = Path.Combine(ProfileRepo.instance.CurrentProfile.Folder, "Saves");
        var saveFiles = Directory.GetFiles(savesPath, "*.json").ToArray();
 
        //TODO: Need to retrieve and parse all save files so that save data is available to populate button
        foreach(var saveState in ProfileRepo.instance.CurrentProfile.SaveStates)
        {
            AddLoadSaveFileButton(saveState);
        }
    }

    public void AddLoadSaveFileButton(SaveState saveState)
    {
        string savesPath = Path.Combine(ProfileRepo.instance.CurrentProfile.Folder, "Saves");
        string filePath = Path.Combine(savesPath, saveState.FileName);

        //Instantiate the prefab as a child of `content`
        GameObject instance = Instantiate(buttonPrefab, content);
        instance.name = $"Button_{Path.GetFileNameWithoutExtension(saveState.FileName)}";

        //Set the button size: 90% of width, 1/16th of height
        RectTransform buttonRect = instance.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        //Set the button click event
        Button button = instance.GetComponent<Button>();
        button.onClick.AddListener(() => OnLoadSaveFileButtonClicked(filePath));

        //Apply text to labels
        instance.transform.Find("SaveNumber").GetComponent<Label>().text = $"Save {saveState.Index:D3}";
        instance.transform.Find("Timestamp").GetComponent<Label>().text = DateTimeHelper.ParseTimeElapsed(saveState.Timestamp);
    }

    private void OnLoadSaveFileButtonClicked(string filePath)
    {
        try
        {
            // Read and deserialize the selected save file.
            string json = File.ReadAllText(filePath);
            SaveState selectedSave = JsonConvert.DeserializeObject<SaveState>(json);
            if (selectedSave != null)
            {
                ProfileRepo.instance.CurrentProfile.CurrentSave = selectedSave;

                // Proceed to load the game scene using the active save.
                StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadScene(SceneHelper.Game)));
            }
            else
            {
                Debug.LogError("Failed to deserialize the selected save file.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading save file {filePath}: {ex.Message}");
        }
    }


    public void OnBackButtonClicked()
    {
        StartCoroutine(fade.FadeOut(SceneRepo.instance.LoadPreviousScene()));
    }
}
