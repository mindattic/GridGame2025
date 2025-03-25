using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Label = TMPro.TextMeshProUGUI;
using System.Linq;
using System;

public class KeyboardDialogInstance : MonoBehaviour
{
    private RectTransform canvas2D;
    private RectTransform panel;
    private RectTransform prompt;
    private RectTransform inputBackdrop;
    private RectTransform inputLabel;
    private RectTransform keysContainer;

    // Row 1
    private RectTransform row1;
    private RectTransform key1;
    private RectTransform key2;
    private RectTransform key3;
    private RectTransform key4;
    private RectTransform key5;
    private RectTransform key6;
    private RectTransform key7;
    private RectTransform key8;
    private RectTransform key9;
    private RectTransform key0;

    // Row 2
    private RectTransform row2;
    private RectTransform keyQ;
    private RectTransform keyW;
    private RectTransform keyE;
    private RectTransform keyR;
    private RectTransform keyT;
    private RectTransform keyY;
    private RectTransform keyU;
    private RectTransform keyI;
    private RectTransform keyO;
    private RectTransform keyP;

    // Row 3
    private RectTransform row3;
    private RectTransform keyA;
    private RectTransform keyS;
    private RectTransform keyD;
    private RectTransform keyF;
    private RectTransform keyG;
    private RectTransform keyH;
    private RectTransform keyJ;
    private RectTransform keyK;
    private RectTransform keyL;

    // Row 4
    private RectTransform row4;
    private RectTransform keyCapsLock;
    private RectTransform keyZ;
    private RectTransform keyX;
    private RectTransform keyC;
    private RectTransform keyV;
    private RectTransform keyB;
    private RectTransform keyN;
    private RectTransform keyM;
    private RectTransform keyBackspace;

    //Row 5
    private RectTransform row5;
    private RectTransform keySpace;
    private RectTransform keyEnter;

    //Confirmation
    private RectTransform confirmationContainer;
    private RectTransform confirmation;
    private RectTransform buttonYes;
    private RectTransform buttonNo;

    public int minLength = 3;
    public int maxLength = 32;
    public System.Action<string> onSubmitClicked;

    private float screenWidth;
    private float screenHeight;
    private bool isCapsLockOn = true;
    private char[] validCharacters = new char[] {
        '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',
        'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P',
        'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L',
        'Z', 'X', 'C', 'V', 'B', 'N', 'M',
        'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p',
        'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l',
        'z', 'x', 'c', 'v', 'b', 'n', 'm', ' '
    };

    //Properties
    public string InputText
    {
        get => inputLabel.GetComponent<Label>().text;
        set => inputLabel.GetComponent<Label>().text = Sanitize(value);
    }

    public void Assign(
      string promptText,
      string confirmText = "Are you sure?",
      string initialText = "", 
      int minLength = 3,
      int maxLength = 32,
      Action<string> onSubmit = default)
    {
        Initialize();
        UpdateKeyLabels();
        ResizeUI();
        BindEvents();

        prompt.GetComponent<Label>().text = promptText;
        confirmation.GetComponent<Label>().text = confirmText;
        InputText = initialText;
        this.minLength = minLength;
        this.maxLength = maxLength;
        onSubmitClicked = onSubmit;
    }

    private void Initialize()
    {
        canvas2D = GameObject.Find(ComponentHelper.KeyboardDialog.Canvas2D).GetComponent<RectTransform>();
        panel = GameObject.Find(ComponentHelper.KeyboardDialog.Panel).GetComponent<RectTransform>();
        prompt = GameObject.Find(ComponentHelper.KeyboardDialog.Prompt).GetComponent<RectTransform>();
        inputBackdrop = GameObject.Find(ComponentHelper.KeyboardDialog.InputBackdrop).GetComponent<RectTransform>();
        inputLabel = GameObject.Find(ComponentHelper.KeyboardDialog.InputLabel).GetComponent<RectTransform>();
        keysContainer = GameObject.Find(ComponentHelper.KeyboardDialog.KeysContainer).GetComponent<RectTransform>();

        //Row1
        row1 = GameObject.Find(ComponentHelper.KeyboardDialog.Row1).GetComponent<RectTransform>();
        key1 = GameObject.Find(ComponentHelper.KeyboardDialog.Key1).GetComponent<RectTransform>();
        key2 = GameObject.Find(ComponentHelper.KeyboardDialog.Key2).GetComponent<RectTransform>();
        key3 = GameObject.Find(ComponentHelper.KeyboardDialog.Key3).GetComponent<RectTransform>();
        key4 = GameObject.Find(ComponentHelper.KeyboardDialog.Key4).GetComponent<RectTransform>();
        key5 = GameObject.Find(ComponentHelper.KeyboardDialog.Key5).GetComponent<RectTransform>();
        key6 = GameObject.Find(ComponentHelper.KeyboardDialog.Key6).GetComponent<RectTransform>();
        key7 = GameObject.Find(ComponentHelper.KeyboardDialog.Key7).GetComponent<RectTransform>();
        key8 = GameObject.Find(ComponentHelper.KeyboardDialog.Key8).GetComponent<RectTransform>();
        key9 = GameObject.Find(ComponentHelper.KeyboardDialog.Key9).GetComponent<RectTransform>();
        key0 = GameObject.Find(ComponentHelper.KeyboardDialog.Key0).GetComponent<RectTransform>();

        //Row2
        row2 = GameObject.Find(ComponentHelper.KeyboardDialog.Row2).GetComponent<RectTransform>();
        keyQ = GameObject.Find(ComponentHelper.KeyboardDialog.KeyQ).GetComponent<RectTransform>();
        keyW = GameObject.Find(ComponentHelper.KeyboardDialog.KeyW).GetComponent<RectTransform>();
        keyE = GameObject.Find(ComponentHelper.KeyboardDialog.KeyE).GetComponent<RectTransform>();
        keyR = GameObject.Find(ComponentHelper.KeyboardDialog.KeyR).GetComponent<RectTransform>();
        keyT = GameObject.Find(ComponentHelper.KeyboardDialog.KeyT).GetComponent<RectTransform>();
        keyY = GameObject.Find(ComponentHelper.KeyboardDialog.KeyY).GetComponent<RectTransform>();
        keyU = GameObject.Find(ComponentHelper.KeyboardDialog.KeyU).GetComponent<RectTransform>();
        keyI = GameObject.Find(ComponentHelper.KeyboardDialog.KeyI).GetComponent<RectTransform>();
        keyO = GameObject.Find(ComponentHelper.KeyboardDialog.KeyO).GetComponent<RectTransform>();
        keyP = GameObject.Find(ComponentHelper.KeyboardDialog.KeyP).GetComponent<RectTransform>();

        //Row3
        row3 = GameObject.Find(ComponentHelper.KeyboardDialog.Row3).GetComponent<RectTransform>();
        keyA = GameObject.Find(ComponentHelper.KeyboardDialog.KeyA).GetComponent<RectTransform>();
        keyS = GameObject.Find(ComponentHelper.KeyboardDialog.KeyS).GetComponent<RectTransform>();
        keyD = GameObject.Find(ComponentHelper.KeyboardDialog.KeyD).GetComponent<RectTransform>();
        keyF = GameObject.Find(ComponentHelper.KeyboardDialog.KeyF).GetComponent<RectTransform>();
        keyG = GameObject.Find(ComponentHelper.KeyboardDialog.KeyG).GetComponent<RectTransform>();
        keyH = GameObject.Find(ComponentHelper.KeyboardDialog.KeyH).GetComponent<RectTransform>();
        keyJ = GameObject.Find(ComponentHelper.KeyboardDialog.KeyJ).GetComponent<RectTransform>();
        keyK = GameObject.Find(ComponentHelper.KeyboardDialog.KeyK).GetComponent<RectTransform>();
        keyL = GameObject.Find(ComponentHelper.KeyboardDialog.KeyL).GetComponent<RectTransform>();

        //Row4
        row4 = GameObject.Find(ComponentHelper.KeyboardDialog.Row4).GetComponent<RectTransform>();    
        keyZ = GameObject.Find(ComponentHelper.KeyboardDialog.KeyZ).GetComponent<RectTransform>();
        keyX = GameObject.Find(ComponentHelper.KeyboardDialog.KeyX).GetComponent<RectTransform>();
        keyC = GameObject.Find(ComponentHelper.KeyboardDialog.KeyC).GetComponent<RectTransform>();
        keyV = GameObject.Find(ComponentHelper.KeyboardDialog.KeyV).GetComponent<RectTransform>();
        keyB = GameObject.Find(ComponentHelper.KeyboardDialog.KeyB).GetComponent<RectTransform>();
        keyN = GameObject.Find(ComponentHelper.KeyboardDialog.KeyN).GetComponent<RectTransform>();
        keyM = GameObject.Find(ComponentHelper.KeyboardDialog.KeyM).GetComponent<RectTransform>();
        
        //Row5
        row5 = GameObject.Find(ComponentHelper.KeyboardDialog.Row5).GetComponent<RectTransform>();
        keyCapsLock = GameObject.Find(ComponentHelper.KeyboardDialog.KeyCapsLock).GetComponent<RectTransform>();
        keySpace = GameObject.Find(ComponentHelper.KeyboardDialog.KeySpace).GetComponent<RectTransform>();
        keyEnter = GameObject.Find(ComponentHelper.KeyboardDialog.KeyEnter).GetComponent<RectTransform>();
        keyBackspace = GameObject.Find(ComponentHelper.KeyboardDialog.KeyBackspace).GetComponent<RectTransform>();

        //Confirmation
        confirmationContainer = GameObject.Find(ComponentHelper.KeyboardDialog.ConfirmationContainer).GetComponent<RectTransform>();
        confirmation = GameObject.Find(ComponentHelper.KeyboardDialog.Confirmation).GetComponent<RectTransform>();
        buttonYes = GameObject.Find(ComponentHelper.KeyboardDialog.ButtonYes).GetComponent<RectTransform>();
        buttonNo = GameObject.Find(ComponentHelper.KeyboardDialog.ButtonNo).GetComponent<RectTransform>();
    }

    private void ResizeUI()
    {
        //Screen dimension references
        screenWidth = canvas2D.rect.width;
        screenHeight = canvas2D.rect.height;

        float currentY = 0f;
        float keySpacing = screenWidth * 0.0025f;
        float rowSpacing = screenWidth * 0.0025f;     
        int guideRowKeyCount = 10;
        float keyWidth = screenWidth * 0.9f / guideRowKeyCount;
        float keyHeight = keyWidth;
        float guideRowWidth = (keyWidth * guideRowKeyCount) + (keySpacing * (guideRowKeyCount - 1));

        panel.sizeDelta = new Vector2(screenWidth, screenHeight);
        panel.anchoredPosition = new Vector2(0, 0);

        prompt.sizeDelta = new Vector2(keyWidth * guideRowKeyCount, keyHeight);
        prompt.anchoredPosition = new Vector2(0, keyHeight * 3);

        inputBackdrop.GetComponent<Image>().color = Color.white;

        inputLabel.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        inputLabel.anchoredPosition = new Vector2(0, keyHeight * 2);

        keysContainer.anchorMin = new Vector2(0.5f, 0.5f);
        keysContainer.anchorMax = new Vector2(0.5f, 0.5f);
        keysContainer.pivot = new Vector2(0.5f, 0.5f);
        keysContainer.anchoredPosition = new Vector2(-guideRowWidth / 2, keyHeight);

        //Row1 (Numbers)
        row1.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row1.anchoredPosition = new Vector2(0f, currentY);
        SetupRow(row1, new RectTransform[] { key1, key2, key3, key4, key5, key6, key7, key8, key9, key0 }, keyWidth, keyHeight, keySpacing, 0);
        currentY -= (keyHeight + rowSpacing);

        //Row2 (Q-P)
        row2.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row2.anchoredPosition = new Vector2(0, currentY);
        SetupRow(row2, new RectTransform[] { keyQ, keyW, keyE, keyR, keyT, keyY, keyU, keyI, keyO, keyP }, keyWidth, keyHeight, keySpacing, 0);
        currentY -= (keyHeight + rowSpacing);

        //Row3 (A-L)
        row3.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row3.anchoredPosition = new Vector2(0, currentY);
        SetupRow(row3, new RectTransform[] { keyA, keyS, keyD, keyF, keyG, keyH, keyJ, keyK, keyL }, keyWidth, keyHeight, keySpacing, 0);
        currentY -= (keyHeight + rowSpacing);

        //Row4 (Z-M)
        row4.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row4.anchoredPosition = new Vector2(0, currentY);
        SetupRow(row4, new RectTransform[] { keyZ, keyX, keyC, keyV, keyB, keyN, keyM }, keyWidth, keyHeight, keySpacing, 0);
        currentY -= (keyHeight + rowSpacing);

        //Row5: CapsLock, Space, Backspace, Enter
        row5.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row5.anchoredPosition = new Vector2(0, currentY);

        keyCapsLock.sizeDelta = new Vector2(keyWidth * 2 + keySpacing, keyHeight);
        keyCapsLock.anchoredPosition = new Vector2(0, 0);

        keySpace.sizeDelta = new Vector2(keyWidth * 4 + keySpacing * 3, keyHeight);
        keySpace.anchoredPosition = new Vector2(keyCapsLock.anchoredPosition.x + keyCapsLock.sizeDelta.x + keySpacing, 0);

        keyBackspace.sizeDelta = new Vector2(keyWidth * 2 + keySpacing, keyHeight);
        keyBackspace.anchoredPosition = new Vector2(keySpace.anchoredPosition.x + keySpace.sizeDelta.x + keySpacing, 0);

        keyEnter.sizeDelta = new Vector2(keyWidth * 2 + keySpacing, keyHeight);
        keyEnter.anchoredPosition = new Vector2(keyBackspace.anchoredPosition.x + keyBackspace.sizeDelta.x + keySpacing, 0);

        //Confirmation
        buttonYes.sizeDelta = new Vector2(keyWidth * 2, keyHeight);
        buttonYes.anchoredPosition = new Vector2(keyWidth / 2 - keySpacing, -keyHeight);

        buttonNo.sizeDelta = new Vector2(keyWidth * 2, keyHeight);
        buttonNo.anchoredPosition = new Vector2(keyWidth / 2 + keySpacing, -keyHeight);
    }

    private void SetupRow(RectTransform row, RectTransform[] keys, float keyWidth, float keyHeight, float keySpacing, float offsetX)
    {
        float currentX = offsetX;
        foreach (var key in keys)
        {
            key.sizeDelta = new Vector2(keyWidth, keyHeight);
            key.anchoredPosition = new Vector2(currentX, 0f);
            currentX += (keyWidth + keySpacing);
        }
    }

    private void BindEvents()
    {
        //Row1: Numbers
        key1.GetComponent<Button>().onClick.AddListener(() => Append('1'));
        key2.GetComponent<Button>().onClick.AddListener(() => Append('2'));
        key3.GetComponent<Button>().onClick.AddListener(() => Append('3'));
        key4.GetComponent<Button>().onClick.AddListener(() => Append('4'));
        key5.GetComponent<Button>().onClick.AddListener(() => Append('5'));
        key6.GetComponent<Button>().onClick.AddListener(() => Append('6'));
        key7.GetComponent<Button>().onClick.AddListener(() => Append('7'));
        key8.GetComponent<Button>().onClick.AddListener(() => Append('8'));
        key9.GetComponent<Button>().onClick.AddListener(() => Append('9'));
        key0.GetComponent<Button>().onClick.AddListener(() => Append('0'));

        //Row2: Q–P
        keyQ.GetComponent<Button>().onClick.AddListener(() => Append('Q'));
        keyW.GetComponent<Button>().onClick.AddListener(() => Append('W'));
        keyE.GetComponent<Button>().onClick.AddListener(() => Append('E'));
        keyR.GetComponent<Button>().onClick.AddListener(() => Append('R'));
        keyT.GetComponent<Button>().onClick.AddListener(() => Append('T'));
        keyY.GetComponent<Button>().onClick.AddListener(() => Append('Y'));
        keyU.GetComponent<Button>().onClick.AddListener(() => Append('U'));
        keyI.GetComponent<Button>().onClick.AddListener(() => Append('I'));
        keyO.GetComponent<Button>().onClick.AddListener(() => Append('O'));
        keyP.GetComponent<Button>().onClick.AddListener(() => Append('P'));

        //Row3: A–L
        keyA.GetComponent<Button>().onClick.AddListener(() => Append('A'));
        keyS.GetComponent<Button>().onClick.AddListener(() => Append('S'));
        keyD.GetComponent<Button>().onClick.AddListener(() => Append('D'));
        keyF.GetComponent<Button>().onClick.AddListener(() => Append('F'));
        keyG.GetComponent<Button>().onClick.AddListener(() => Append('G'));
        keyH.GetComponent<Button>().onClick.AddListener(() => Append('H'));
        keyJ.GetComponent<Button>().onClick.AddListener(() => Append('J'));
        keyK.GetComponent<Button>().onClick.AddListener(() => Append('K'));
        keyL.GetComponent<Button>().onClick.AddListener(() => Append('L'));

        //Row4: Z–M, 
    
        keyZ.GetComponent<Button>().onClick.AddListener(() => Append('Z'));
        keyX.GetComponent<Button>().onClick.AddListener(() => Append('X'));
        keyC.GetComponent<Button>().onClick.AddListener(() => Append('C'));
        keyV.GetComponent<Button>().onClick.AddListener(() => Append('V'));
        keyB.GetComponent<Button>().onClick.AddListener(() => Append('B'));
        keyN.GetComponent<Button>().onClick.AddListener(() => Append('N'));
        keyM.GetComponent<Button>().onClick.AddListener(() => Append('M'));
     

        //Row5: Shift, Space, Backspace, Enter
        keyCapsLock.GetComponent<Button>().onClick.AddListener(ToggleCapsLock);
        keySpace.GetComponent<Button>().onClick.AddListener(() => Append(' '));
        keyBackspace.GetComponent<Button>().onClick.AddListener(() => Backspace());
        keyEnter.GetComponent<Button>().onClick.AddListener(() => ToggleConfirmation());

        //Confirmation 
        buttonYes.GetComponent<Button>().onClick.AddListener(() => Submit());
        buttonNo.GetComponent<Button>().onClick.AddListener(() => ToggleConfirmation());
    }

    private void ToggleCapsLock()
    {
        isCapsLockOn = !isCapsLockOn;
        UpdateKeyLabels();
    }

    private void UpdateKeyLabels()
    {
        //Row2: Q–P
        var row2Letters = new char[] { 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P' };
        var row2Buttons = new RectTransform[] { keyQ, keyW, keyE, keyR, keyT, keyY, keyU, keyI, keyO, keyP };
        for (int i = 0; i < row2Buttons.Length; i++)
        {
            var label = row2Buttons[i].GetComponentInChildren<Label>();
            label.text = isCapsLockOn ? row2Letters[i].ToString() : row2Letters[i].ToString().ToLower();
        }

        //Row3: A–L
        var row3Letters = new char[] { 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L' };
        var row3Buttons = new RectTransform[] { keyA, keyS, keyD, keyF, keyG, keyH, keyJ, keyK, keyL };
        for (int i = 0; i < row3Buttons.Length; i++)
        {
            var label = row3Buttons[i].GetComponentInChildren<Label>();
            label.text = isCapsLockOn ? row3Letters[i].ToString() : row3Letters[i].ToString().ToLower();
        }

        //Row4: Z–M
        var row4Letters = new char[] { 'Z', 'X', 'C', 'V', 'B', 'N', 'M' };
        var row4Buttons = new RectTransform[] { keyZ, keyX, keyC, keyV, keyB, keyN, keyM };
        for (int i = 0; i < row4Buttons.Length; i++)
        {
            var label = row4Buttons[i].GetComponentInChildren<Label>();
            label.text = isCapsLockOn ? row4Letters[i].ToString() : row4Letters[i].ToString().ToLower();
        }
    }
    private void Append(char c)
    {
        var character = isCapsLockOn && char.IsLetter(c) ? char.ToUpper(c) : char.ToLower(c);
        InputText += character;

        //Toggle off caps lock off after every keypress
        if (isCapsLockOn)
            ToggleCapsLock();
    }

    private void Backspace()
    {
        if (InputText.Length > 0)
            InputText = InputText.Substring(0, InputText.Length - 1);
    }

    private void ToggleConfirmation()
    {
        if (InputText.Length < 1)
            return;

        InputText = Sanitize(InputText);
        var isVisible = !confirmationContainer.gameObject.activeSelf;
        keysContainer.gameObject.SetActive(!isVisible);
        confirmationContainer.gameObject.SetActive(isVisible);
    }

    private void Submit()
    {
        InputText = Sanitize(InputText);
        if (InputText.Length < minLength)
        {
            Debug.Log("Input is too short.");
            return;
        }
        if (InputText.Length > maxLength)
        {
            Debug.Log("Input is too long.");
            return;
        }

        // Invoke the callback to return the input.
        onSubmitClicked?.Invoke(InputText);

        // Optionally destroy the keyboard instance after submission.
        Destroy(gameObject);
    }

    private string Sanitize(string input)
    {
        return new string(input.Where(c => validCharacters.Contains(c)).ToArray());
    }
}

public static class KeyboardDialog
{
    public static KeyboardDialogInstance Show(
        RectTransform canvas2D,
        string promptText,
        string confirmText = "Are you sure?",
        string initialText = "",
        int minLength = 3,
        int maxLength = 32,
        Action<string> onSubmit = default)
    {
        //Reload prefab from Resources
        var prefabPath = $"{ResourceFolderHelper.Prefabs}/KeyboardDialog";
        var prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
            throw new UnityException($"Prefab `{prefabPath}` not found");

        //Find a parent canvas2D
        if (canvas2D == null)
            throw new UnityException("Canvas2D not found");

        //Instantiate prefab
        GameObject go = GameObject.Instantiate(prefab, canvas2D);
        if (go == null)
            throw new UnityException("Failed to instantiate prefab");
        go.name = $"Keyboard";

        //Get the Virtual Keyboard instance
        KeyboardDialogInstance instance = go.GetComponent<KeyboardDialogInstance>();
        if (instance == null)
            throw new UnityException("KeyboardDialogInstance component not found on the game object");

        //Assign properties
        instance.Assign(promptText, confirmText, initialText, minLength, maxLength, onSubmit);

        //Return the instance
        return instance;
    }
}

