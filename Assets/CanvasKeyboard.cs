using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Label = TMPro.TextMeshProUGUI;
using Textbox = TMPro.TMP_InputField;
using UnityEditor.Experimental.GraphView;

public class CanvasKeyboard : MonoBehaviour
{
    private RectTransform canvas2D;
    private RectTransform panel;
    private RectTransform instructions;
    private RectTransform inputField;
    [Header("Submit Button")] public RectTransform submitButton;
    private RectTransform keysContainer;

    // Row 1
    private RectTransform row1;
    public RectTransform key1;
    public RectTransform key2;
    public RectTransform key3;
    public RectTransform key4;
    public RectTransform key5;
    public RectTransform key6;
    public RectTransform key7;
    public RectTransform key8;
    public RectTransform key9;
    public RectTransform key0;

    // Row 2
    private RectTransform row2;
    public RectTransform keyQ;
    public RectTransform keyW;
    public RectTransform keyE;
    public RectTransform keyR;
    public RectTransform keyT;
    public RectTransform keyY;
    public RectTransform keyU;
    public RectTransform keyI;
    public RectTransform keyO;
    public RectTransform keyP;

    // Row 3
    private RectTransform row3;
    public RectTransform keyA;
    public RectTransform keyS;
    public RectTransform keyD;
    public RectTransform keyF;
    public RectTransform keyG;
    public RectTransform keyH;
    public RectTransform keyJ;
    public RectTransform keyK;
    public RectTransform keyL;

    // Row 4
    private RectTransform row4;
    public RectTransform keyZ;
    public RectTransform keyX;
    public RectTransform keyC;
    public RectTransform keyV;
    public RectTransform keyB;
    public RectTransform keyN;
    public RectTransform keyM;

    //Row 5
    private RectTransform row5;
    public RectTransform keyCapsLock;
    public RectTransform keySpace;
    public RectTransform keyBackspace;

    private float screenWidth;
    private float screenHeight;
    private float inputWidth;
    private float inputHeight;

    private float buttonWidth;
    private float buttonHeight;

    public bool IsSubmitted { get; private set; } = false;

    private void Awake()
    {
        canvas2D = GameObject.Find(ComponentHelper.CanvasKeyboard.Canvas2D).GetComponent<RectTransform>() ?? throw new UnityException("Canvas2D is null");
        panel = GameObject.Find(ComponentHelper.CanvasKeyboard.Panel).GetComponent<RectTransform>() ?? throw new UnityException("Panel is null");
        instructions = GameObject.Find(ComponentHelper.CanvasKeyboard.Instructions).GetComponent<RectTransform>() ?? throw new UnityException("Instructions is null");
        inputField = GameObject.Find(ComponentHelper.CanvasKeyboard.InputField).GetComponent<RectTransform>() ?? throw new UnityException("InputField is null");
        submitButton = GameObject.Find(ComponentHelper.CanvasKeyboard.SubmitButton).GetComponent<RectTransform>() ?? throw new UnityException("SubmitButton is null");
        keysContainer = GameObject.Find(ComponentHelper.CanvasKeyboard.KeysContainer).GetComponent<RectTransform>() ?? throw new UnityException("KeysContainer is null");

        //Row1
        row1 = GameObject.Find(ComponentHelper.CanvasKeyboard.Row1).GetComponent<RectTransform>() ?? throw new UnityException("Row1 is null");
        key1 = GameObject.Find(ComponentHelper.CanvasKeyboard.Key1).GetComponent<RectTransform>();
        key2 = GameObject.Find(ComponentHelper.CanvasKeyboard.Key2).GetComponent<RectTransform>();
        key3 = GameObject.Find(ComponentHelper.CanvasKeyboard.Key3).GetComponent<RectTransform>();
        key4 = GameObject.Find(ComponentHelper.CanvasKeyboard.Key4).GetComponent<RectTransform>();
        key5 = GameObject.Find(ComponentHelper.CanvasKeyboard.Key5).GetComponent<RectTransform>();
        key6 = GameObject.Find(ComponentHelper.CanvasKeyboard.Key6).GetComponent<RectTransform>();
        key7 = GameObject.Find(ComponentHelper.CanvasKeyboard.Key7).GetComponent<RectTransform>();
        key8 = GameObject.Find(ComponentHelper.CanvasKeyboard.Key8).GetComponent<RectTransform>();
        key9 = GameObject.Find(ComponentHelper.CanvasKeyboard.Key9).GetComponent<RectTransform>();
        key0 = GameObject.Find(ComponentHelper.CanvasKeyboard.Key0).GetComponent<RectTransform>();

        //Row2
        row2 = GameObject.Find(ComponentHelper.CanvasKeyboard.Row2).GetComponent<RectTransform>() ?? throw new UnityException("Row2 is null");
        keyQ = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyQ).GetComponent<RectTransform>();
        keyW = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyW).GetComponent<RectTransform>();
        keyE = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyE).GetComponent<RectTransform>();
        keyR = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyR).GetComponent<RectTransform>();
        keyT = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyT).GetComponent<RectTransform>();
        keyY = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyY).GetComponent<RectTransform>();
        keyU = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyU).GetComponent<RectTransform>();
        keyI = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyI).GetComponent<RectTransform>();
        keyO = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyO).GetComponent<RectTransform>();
        keyP = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyP).GetComponent<RectTransform>();

        //Row3
        row3 = GameObject.Find(ComponentHelper.CanvasKeyboard.Row3).GetComponent<RectTransform>() ?? throw new UnityException("Row3 is null");
        keyA = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyA).GetComponent<RectTransform>();
        keyS = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyS).GetComponent<RectTransform>();
        keyD = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyD).GetComponent<RectTransform>();
        keyF = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyF).GetComponent<RectTransform>();
        keyG = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyG).GetComponent<RectTransform>();
        keyH = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyH).GetComponent<RectTransform>();
        keyJ = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyJ).GetComponent<RectTransform>();
        keyK = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyK).GetComponent<RectTransform>();
        keyL = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyL).GetComponent<RectTransform>();

        //Row4
        row4 = GameObject.Find(ComponentHelper.CanvasKeyboard.Row4).GetComponent<RectTransform>() ?? throw new UnityException("Row4 is null");
        keyZ = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyZ).GetComponent<RectTransform>();
        keyX = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyX).GetComponent<RectTransform>();
        keyC = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyC).GetComponent<RectTransform>();
        keyV = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyV).GetComponent<RectTransform>();
        keyB = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyB).GetComponent<RectTransform>();
        keyN = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyN).GetComponent<RectTransform>();
        keyM = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyM).GetComponent<RectTransform>();

        //Row5
        row5 = GameObject.Find(ComponentHelper.CanvasKeyboard.Row5).GetComponent<RectTransform>() ?? throw new UnityException("Row5 is null");
        keyCapsLock = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyCapsLock).GetComponent<RectTransform>();
        keySpace = GameObject.Find(ComponentHelper.CanvasKeyboard.KeySpace).GetComponent<RectTransform>();
        keyBackspace = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyBackspace).GetComponent<RectTransform>();

   

        ResizeUI();

        // Hook up the onClick events for each key
        BindAllKeys();
    }

    private void ResizeUI()
    {
        // Screen dimension references
        screenWidth = canvas2D.rect.width;
        screenHeight = canvas2D.rect.height;

      
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.anchoredPosition = Vector2.zero;

        float keySpacing = 0;
        float rowSpacing = 0;

        int guideRowKeyCount = 10;
        float keyWidth = screenWidth * 0.9f / guideRowKeyCount;
        float keyHeight = keyWidth;
        float guideRowWidth = (keyWidth * guideRowKeyCount);

        inputWidth = screenWidth * 0.9f;
        buttonWidth = inputWidth / guideRowKeyCount - (keySpacing * guideRowKeyCount - 1);
        buttonHeight = buttonWidth;

        inputField.sizeDelta = new Vector2(inputWidth, keyHeight);


        keysContainer.anchorMin = new Vector2(0.5f, 0.5f);
        keysContainer.anchorMax = new Vector2(0.5f, 0.5f);
        keysContainer.pivot = new Vector2(0.5f, 0.5f);
        keysContainer.anchoredPosition = new Vector2(-guideRowWidth/2, 0);

        // Place Row1 near the top-left of keysContainer
        float topPadding = 0f;
        float row1Y = -topPadding;  // pivot(0.5,0.5) => negative means going downward
        row1.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row1.anchoredPosition = new Vector2(0f, row1Y);

        RectTransform[] row1Keys = { key1, key2, key3, key4, key5, key6, key7, key8, key9, key0 };
        PositionRowKeys(row1, row1Keys, keyWidth, keyHeight, keySpacing);

        float currentY = row1Y;

        // Row2
        currentY -= (keyHeight + rowSpacing);
        row2.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row2.anchoredPosition = new Vector2(0f, currentY);
        RectTransform[] row2Keys = { keyQ, keyW, keyE, keyR, keyT, keyY, keyU, keyI, keyO, keyP };
        PositionRowKeys(row2, row2Keys, keyWidth, keyHeight, keySpacing);

        // Row3
        currentY -= (keyHeight + rowSpacing);
        row3.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row3.anchoredPosition = new Vector2(0f, currentY);
        RectTransform[] row3Keys = { keyA, keyS, keyD, keyF, keyG, keyH, keyJ, keyK, keyL };
        PositionRowKeys(row3, row3Keys, keyWidth, keyHeight, keySpacing);

        // Row4
        currentY -= (keyHeight + rowSpacing);
        row4.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row4.anchoredPosition = new Vector2(0f, currentY);
        RectTransform[] row4Keys = { keyZ, keyX, keyC, keyV, keyB, keyN, keyM };
        PositionRowKeys(row4, row4Keys, keyWidth, keyHeight, keySpacing);

        // Row5
        currentY -= (keyHeight + rowSpacing);
        row5.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row5.anchoredPosition = new Vector2(0f, currentY);

        float totalRatio = 0.33f + 0.66f + 0.33f;
        float leftoverWidth = guideRowWidth - 2 * keySpacing;
        if (leftoverWidth < 0) leftoverWidth = 0;

        float capsLockW = (0.33f / totalRatio) * leftoverWidth;
        float spaceW = (0.66f / totalRatio) * leftoverWidth;
        float backspaceW = (0.33f / totalRatio) * leftoverWidth;

        float currentX = 0f;
        keyCapsLock.sizeDelta = new Vector2(capsLockW, keyHeight);
        keyCapsLock.anchoredPosition = new Vector2(currentX, 0f);
        currentX += capsLockW + keySpacing;

        keySpace.sizeDelta = new Vector2(spaceW, keyHeight);
        keySpace.anchoredPosition = new Vector2(currentX, 0f);
        currentX += spaceW + keySpacing;

        keyBackspace.sizeDelta = new Vector2(backspaceW, keyHeight);
        keyBackspace.anchoredPosition = new Vector2(currentX, 0f);
    }

    private void PositionRowKeys(
        RectTransform row,
        RectTransform[] keys,
        float keyW,
        float keyH,
        float spacingBetweenKeys
    )
    {
        float currentX = 0f;
        foreach (var key in keys)
        {
            key.sizeDelta = new Vector2(keyW, keyH);
            key.anchoredPosition = new Vector2(currentX, 0f);
            currentX += (keyW + spacingBetweenKeys);
        }
    }

    private bool isCapsOn = false;

    private void BindAllKeys()
    {
        submitButton.GetComponent<Button>().onClick.AddListener(OnSubmitClicked);

        //Row1: digits
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

        //Row4: Z–M
        keyZ.GetComponent<Button>().onClick.AddListener(() => Append('Z'));
        keyX.GetComponent<Button>().onClick.AddListener(() => Append('X'));
        keyC.GetComponent<Button>().onClick.AddListener(() => Append('C'));
        keyV.GetComponent<Button>().onClick.AddListener(() => Append('V'));
        keyB.GetComponent<Button>().onClick.AddListener(() => Append('B'));
        keyN.GetComponent<Button>().onClick.AddListener(() => Append('N'));
        keyM.GetComponent<Button>().onClick.AddListener(() => Append('M'));

        //Row5: CapsLock, Space, Backspace
        keyCapsLock.GetComponent<Button>().onClick.AddListener(ToggleCapsLock);
        keySpace.GetComponent<Button>().onClick.AddListener(() => Append(' '));
        keyBackspace.GetComponent<Button>().onClick.AddListener(() => Backspace());
    }

    private void ToggleCapsLock()
    {
        isCapsOn = !isCapsOn;
        UpdateKeyLabels();
    }

    private void UpdateKeyLabels()
    {
        //Row2: Q–P
        var row2Letters = new char[] { 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P' };
        var row2Buttons = new RectTransform[] { keyQ, keyW, keyE, keyR, keyT, keyY, keyU, keyI, keyO, keyP };
        for (int i = 0; i < row2Buttons.Length; i++)
        {
            var label = row2Buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            label.text = isCapsOn ? row2Letters[i].ToString() : row2Letters[i].ToString().ToLower();
        }

        //Row3: A–L
        var row3Letters = new char[] { 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L' };
        var row3Buttons = new RectTransform[] { keyA, keyS, keyD, keyF, keyG, keyH, keyJ, keyK, keyL };
        for (int i = 0; i < row3Buttons.Length; i++)
        {
            var label = row3Buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            label.text = isCapsOn ? row3Letters[i].ToString() : row3Letters[i].ToString().ToLower();
        }

        //Row4: Z–M
        var row4Letters = new char[] { 'Z', 'X', 'C', 'V', 'B', 'N', 'M' };
        var row4Buttons = new RectTransform[] { keyZ, keyX, keyC, keyV, keyB, keyN, keyM };
        for (int i = 0; i < row4Buttons.Length; i++)
        {
            var label = row4Buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            label.text = isCapsOn ? row4Letters[i].ToString() : row4Letters[i].ToString().ToLower();
        }
    }

    //Append character to input box
    void Append(char c)
    {
        var character = isCapsOn && char.IsLetter(c) ? char.ToUpper(c) : char.ToLower(c);
        inputField.GetComponent<Textbox>().text += character;
    }

    void Backspace()
    {
        var text = inputField.GetComponent<Textbox>().text;
        if (text.Length > 0)
            inputField.GetComponent<Textbox>().text = text.Substring(0, text.Length - 1);
    }

    void OnSubmitClicked()
    {
        IsSubmitted = true;
        Debug.Log("Input submitted: " + inputField.GetComponent<Textbox>().text);
    }

    public string GetSanitizedInput()
    {
        return SanitizeInput(inputField.GetComponent<Textbox>().text);
    }

    private string SanitizeInput(string input)
    {
        // Remove HTML-like tags to avoid markup injection
        return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
    }
}
