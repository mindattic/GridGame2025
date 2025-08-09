using Assets.Helper;
using System;
using UnityEngine;
using UnityEngine.UI;
using c = Assets.Helpers.CanvasHelper;
using Label = TMPro.TextMeshProUGUI;

public class ConfirmationDialogInstance : MonoBehaviour
{
    private RectTransform panel;
    private RectTransform prompt;
    private RectTransform buttonYes;
    private RectTransform buttonNo;
    private float screenWidth;
    private float screenHeight;
    public Action<bool> onSubmitClicked;

    public void Assign(string text, Action<bool> onSubmit = default)
    {
        Setup();
        ResizeUI();
        BindEvents();

        prompt.GetComponent<Label>().text = text;
        onSubmitClicked = onSubmit;
    }

    /// <summary>
    /// Call this method to initialize the dialog after instantiating the prefab.
    /// </summary>
    /// <param name="text">CreditsLabel to display to the user.</param>
    /// <param name="onSubmitCallback">Callback for handling Yes/No attackResult.</param>
    private void Setup()
    {
        panel = GameObject.Find(GameObjectHelper.ConfirmationDialog.Panel).GetComponent<RectTransform>();
        prompt = GameObject.Find(GameObjectHelper.ConfirmationDialog.Prompt).GetComponent<RectTransform>();
        buttonYes = GameObject.Find(GameObjectHelper.ConfirmationDialog.ButtonYes).GetComponent<RectTransform>();
        buttonNo = GameObject.Find(GameObjectHelper.ConfirmationDialog.ButtonNo).GetComponent<RectTransform>();
    }

    /// <summary>
    /// Optionally resize the panel to match the canvas dimensions.
    /// </summary>
    private void ResizeUI()
    {
        //Screen dimension references
        screenWidth = c.CanvasRect.rect.width;
        screenHeight = c.CanvasRect.rect.height;
        //float keySpacing = startX * 0.0025f;
        float keyWidth = screenWidth * 0.9f / 10;
        float keyHeight = keyWidth;

        panel.sizeDelta = new Vector2(screenWidth, screenHeight);
        panel.anchoredPosition = new Vector2(0, 0);

        //Confirmation
        buttonYes.sizeDelta = new Vector2(keyWidth * 2, keyHeight);
        //buttonYes.anchoredPosition = new Vector2(keyWidth / 2 - keySpacing, -keyHeight);

        buttonNo.sizeDelta = new Vector2(keyWidth * 2, keyHeight);
        //buttonNo.anchoredPosition = new Vector2(keyWidth / 2 + keySpacing, -keyHeight);
    }

    private void BindEvents()
    {
        buttonYes.GetComponent<Button>().onClick.AddListener(() => Submit(true));
        buttonNo.GetComponent<Button>().onClick.AddListener(() => Submit(false));
    }

    /// <summary>
    /// When the user presses Yes or No, invoke the callback and destroy this instance.
    /// </summary>
    private void Submit(bool result)
    {
        onSubmitClicked?.Invoke(result);
        Destroy(gameObject);
    }
}

public static class ConfirmationDialog
{
    /// <summary>
    /// Shows a ConfirmationDialog prefab in the specified canvas, with the given textarea and callback.
    /// </summary>
    /// <param name="text">Prompt to display in the confirmation dialog.</param>
    /// <param name="onSubmit">Callback receiving true (Yes) or false (No).</param>
    public static ConfirmationDialogInstance Show(
        string text = "Are you sure?",
        Action<bool> onSubmit = null)
    {
        var prefab = PrefabRepo.Prefabs["ConfirmationDialog"];
        if (prefab == null)
            throw new UnityException($"Prefab not found");

        // Instantiate the dialog prefab.
        GameObject go = GameObject.Instantiate(prefab, c.CanvasRect);
        if (go == null)
            throw new UnityException("Failed to instantiate ConfirmationDialog prefab");
        go.name = "ConfirmationDialog";

        // Get the ConfirmationDialogInstance component.
        var instance = go.GetComponent<ConfirmationDialogInstance>();
        if (instance == null)
            throw new UnityException("ConfirmationDialogInstance component not found on the prefab");

        // Setup with user-specified textarea and callback.
        instance.Assign(text, onSubmit);

        // Return the instance so caller can manage it further if needed.
        return instance;
    }
}