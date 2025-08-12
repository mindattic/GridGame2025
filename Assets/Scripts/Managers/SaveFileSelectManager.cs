using Assets.Helper;
using Assets.Helpers;
using Game.Models.Profile;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using scene = Assets.Helpers.SceneHelper;
using Button = UnityEngine.UI.Button;
using Label = TMPro.TextMeshProUGUI;

public class SaveFileSelectManager : MonoBehaviour
{
    // Fields
    private GameObject buttonPrefab;
    private Label header;
    private RectTransform scrollView;
    private Transform content;
    private VerticalLayoutGroup verticalLayoutGroup;


    private float screenWidth;
    private float screenHeight;
    private float buttonWidth;
    private float buttonHeight;
    private float fontSize;
    private float rowSpacing;


    private void Awake()
    {
        buttonPrefab = PrefabLibrary.Prefabs["SaveFileButtonPrefab"];
        content = GameObject.Find(GameObjectHelper.StageSelect.Content).GetComponent<Transform>();
    }

    private void Start()
    {
        //Validate a current profile exists
        if (!ProfileHelper.HasCurrentProfile)
        {
            Debug.LogError("No current profile selected.");
            scene.Change.ToProfileCreate();
            return;
        }

        Reload();
        scene.FadeIn();
    }


    private void Clear()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    private void Reload()
    {
        //Hide existing content
        Clear();

        //Retrieve all saves in profile
        string savesPath = Path.Combine(ProfileHelper.CurrentProfile.Folder, "Saves");
        var saveFiles = Directory.GetFiles(savesPath, "*.json").ToArray();

        //Add each save as a button
        foreach (var item in ProfileHelper.CurrentProfile.SaveStates)
        {
            AddLoadSaveFileButton(item);
        }
    }

    public void AddLoadSaveFileButton(SaveState item)
    {
        string savesPath = Path.Combine(ProfileHelper.CurrentProfile.Folder, "Saves");
        string filePath = Path.Combine(savesPath, item.FileName);

        //Instantiate the prefab as a child of `content`
        GameObject instance = Instantiate(buttonPrefab, content);
        instance.name = $"Button_{Path.GetFileNameWithoutExtension(item.FileName)}";

        //Show the button size: 90% of width, 1/16th of height
        //RectTransform buttonRect = instance.GetComponent<RectTransform>();
        //buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        //Show the button click event
        Button button = instance.GetComponent<Button>();
        button.onClick.AddListener(() => OnLoadSaveFileButtonClicked(filePath));

        //Apply textarea to labels
        instance.transform.Find("SaveNumber").GetComponent<Label>().text = $"Save {item.Index:D3}";
        instance.transform.Find("Timestamp").GetComponent<Label>().text = DateTimeHelper.ParseTimeElapsed(item.Timestamp);
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
                ProfileHelper.CurrentProfile.CurrentSave = selectedSave;

                // Proceed to load the game scene using the active save.
                scene.Change.ToGame();
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
        scene.Change.ToPreviousScene();
    }
}
