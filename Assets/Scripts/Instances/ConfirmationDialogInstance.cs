using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Label = TMPro.TextMeshProUGUI;

public class ConfirmationDialogInstance : MonoBehaviour
{
    private RectTransform canvas2D;
    private RectTransform panel;
    private RectTransform prompt;
    private RectTransform buttonYes;
    private RectTransform buttonNo;
    public Action<bool> onSubmitClicked;

    public void Assign(string text, Action<bool> onSubmit = default)
    {
        Initialize();
        ResizeUI();
        prompt.GetComponent<Label>().text = text;
        onSubmitClicked = onSubmit;
    }

    /// <summary>
    /// Call this method to initialize the dialog after instantiating the prefab.
    /// </summary>
    /// <param name="text">Text to display to the user.</param>
    /// <param name="onSubmitCallback">Callback for handling Yes/No result.</param>
    public void Initialize()
    {
        canvas2D = GameObject.Find(ComponentHelper.ConfirmationDialog.Canvas2D).GetComponent<RectTransform>();
        panel = GameObject.Find(ComponentHelper.ConfirmationDialog.Panel).GetComponent<RectTransform>();
        prompt = GameObject.Find(ComponentHelper.ConfirmationDialog.Prompt).GetComponent<RectTransform>();
        buttonYes = GameObject.Find(ComponentHelper.ConfirmationDialog.ButtonYes).GetComponent<RectTransform>();
        buttonNo = GameObject.Find(ComponentHelper.ConfirmationDialog.ButtonNo).GetComponent<RectTransform>();

        //Bind event listeners
        buttonYes.GetComponent<Button>().onClick.AddListener(() => Submit(true));
        buttonNo.GetComponent<Button>().onClick.AddListener(() => Submit(false));
    }

    /// <summary>
    /// Optionally resize the panel to match the canvas dimensions.
    /// </summary>
    private void ResizeUI()
    {
        panel.sizeDelta = new Vector2(canvas2D.rect.width, canvas2D.rect.height);
        panel.anchoredPosition = Vector2.zero;
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
    /// Shows a ConfirmationDialog prefab in the specified canvas, with the given text and callback.
    /// </summary>
    /// <param name="canvas2D">The RectTransform of your 2D canvas.</param>
    /// <param name="text">Prompt to display in the confirmation dialog.</param>
    /// <param name="onSubmit">Callback receiving true (Yes) or false (No).</param>
    public static ConfirmationDialogInstance Show(
        RectTransform canvas2D,
        string text = "Are you sure?",
        Action<bool> onSubmit = null)
    {
        // Load the prefab from Resources.
        var prefabPath = $"{ResourceFolderHelper.Prefabs}/ConfirmationDialog";
        var prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
            throw new UnityException($"Prefab `{prefabPath}` not found");

        // Check for a valid parent canvas.
        if (canvas2D == null)
            throw new UnityException("Canvas2D not found");

        // Instantiate the dialog prefab.
        GameObject go = GameObject.Instantiate(prefab, canvas2D);
        if (go == null)
            throw new UnityException("Failed to instantiate ConfirmationDialog prefab");
        go.name = "ConfirmationDialog";

        // Get the ConfirmationDialogInstance component.
        var instance = go.GetComponent<ConfirmationDialogInstance>();
        if (instance == null)
            throw new UnityException("ConfirmationDialogInstance component not found on the prefab");

        // Initialize with user-specified text and callback.
        instance.Assign(text, onSubmit);

        // Return the instance so caller can manage it further if needed.
        return instance;
    }
}