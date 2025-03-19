using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Label = TMPro.TextMeshProUGUI;

public class CanvasKeyboard : MonoBehaviour
{
    private RectTransform canvas2D;
    private RectTransform panel;
    private RectTransform prompt;
    private RectTransform inputBackdrop;
    private RectTransform inputLabel;
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
    public RectTransform keyCapsLock;
    public RectTransform keyZ;
    public RectTransform keyX;
    public RectTransform keyC;
    public RectTransform keyV;
    public RectTransform keyB;
    public RectTransform keyN;
    public RectTransform keyM;
    public RectTransform keyBackspace;

    //Row 5
    private RectTransform row5;
    public RectTransform keySpace;
    public RectTransform keyEnter;

    //Confirmation
    public RectTransform confirmationContainer;
    public RectTransform areYouSure;
    public RectTransform buttonYes;
    public RectTransform buttonNo;

    private float screenWidth;
    private float screenHeight;
    private float inputWidth;
    private float inputHeight;
    private float buttonWidth;
    private float buttonHeight;
    private bool isCapsOn = true;

    public string InputText
    {
        get => inputLabel.GetComponent<Label>().text;
        set => inputLabel.GetComponent<Label>().text = value;
    }

    private void Awake()
    {
        canvas2D = GameObject.Find(ComponentHelper.CanvasKeyboard.Canvas2D).GetComponent<RectTransform>() ?? throw new UnityException("Canvas2D is null");
        panel = GameObject.Find(ComponentHelper.CanvasKeyboard.Panel).GetComponent<RectTransform>() ?? throw new UnityException("Panel is null");
        prompt = GameObject.Find(ComponentHelper.CanvasKeyboard.Prompt).GetComponent<RectTransform>() ?? throw new UnityException("Prompt is null");
        inputBackdrop = GameObject.Find(ComponentHelper.CanvasKeyboard.InputLabel).GetComponent<RectTransform>() ?? throw new UnityException("InputLabel is null");
        inputLabel = GameObject.Find(ComponentHelper.CanvasKeyboard.InputLabel).GetComponent<RectTransform>() ?? throw new UnityException("InputLabel is null");
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
        keyCapsLock = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyCapsLock).GetComponent<RectTransform>();
        keyZ = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyZ).GetComponent<RectTransform>();
        keyX = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyX).GetComponent<RectTransform>();
        keyC = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyC).GetComponent<RectTransform>();
        keyV = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyV).GetComponent<RectTransform>();
        keyB = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyB).GetComponent<RectTransform>();
        keyN = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyN).GetComponent<RectTransform>();
        keyM = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyM).GetComponent<RectTransform>();
        keyBackspace = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyBackspace).GetComponent<RectTransform>();

        //Row5
        row5 = GameObject.Find(ComponentHelper.CanvasKeyboard.Row5).GetComponent<RectTransform>() ?? throw new UnityException("Row5 is null");
        keySpace = GameObject.Find(ComponentHelper.CanvasKeyboard.KeySpace).GetComponent<RectTransform>();
        keyEnter = GameObject.Find(ComponentHelper.CanvasKeyboard.KeyEnter).GetComponent<RectTransform>() ?? throw new UnityException("KeyEnter is null");

        confirmationContainer = GameObject.Find(ComponentHelper.CanvasKeyboard.ConfirmationContainer).GetComponent<RectTransform>() ?? throw new UnityException("ConfirmationContainer is null");
        areYouSure = GameObject.Find(ComponentHelper.CanvasKeyboard.AreYouSure).GetComponent<RectTransform>() ?? throw new UnityException("AreYouSure is null");
        buttonYes = GameObject.Find(ComponentHelper.CanvasKeyboard.ButtonYes).GetComponent<RectTransform>() ?? throw new UnityException("ButtonYes is null");
        buttonNo = GameObject.Find(ComponentHelper.CanvasKeyboard.ButtonNo).GetComponent<RectTransform>() ?? throw new UnityException("buttonNo is null");

        ToggleCapsLock();
        ResizeUI();
        BindAllKeys();
    }

    private void ResizeUI()
    {
        // Screen dimension references
        screenWidth = canvas2D.rect.width;
        screenHeight = canvas2D.rect.height;

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

        inputLabel.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        inputLabel.anchoredPosition = new Vector2(0, keyHeight * 2);

        keysContainer.anchorMin = new Vector2(0.5f, 0.5f);
        keysContainer.anchorMax = new Vector2(0.5f, 0.5f);
        keysContainer.pivot = new Vector2(0.5f, 0.5f);
        keysContainer.anchoredPosition = new Vector2(-guideRowWidth / 2, keyHeight);

        float currentY = 0f;

        //Row1 (Numbers)
        row1.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row1.anchoredPosition = new Vector2(0f, currentY);
        PositionRowKeys(row1, new RectTransform[] { key1, key2, key3, key4, key5, key6, key7, key8, key9, key0 }, keyWidth, keyHeight, keySpacing, 0);
        currentY -= (keyHeight + rowSpacing);

        //Row2 (Q-P), shifted right by half key width
        row2.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row2.anchoredPosition = new Vector2(0, currentY);
        PositionRowKeys(row2, new RectTransform[] { keyQ, keyW, keyE, keyR, keyT, keyY, keyU, keyI, keyO, keyP }, keyWidth, keyHeight, keySpacing, 0);
        currentY -= (keyHeight + rowSpacing);

        //Row3 (A-L), shifted slightly more
        row3.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row3.anchoredPosition = new Vector2(0, currentY);
        PositionRowKeys(row3, new RectTransform[] { keyA, keyS, keyD, keyF, keyG, keyH, keyJ, keyK, keyL }, keyWidth, keyHeight, keySpacing, 0);
        currentY -= (keyHeight + rowSpacing);

        //Row4 (Z-M), shifted one full key width
        row4.sizeDelta = new Vector2(guideRowWidth, keyHeight);
        row4.anchoredPosition = new Vector2(0, currentY);
        PositionRowKeys(row4, new RectTransform[] { keyZ, keyX, keyC, keyV, keyB, keyN, keyM }, keyWidth, keyHeight, keySpacing, 0);
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


    }

    private void PositionRowKeys(RectTransform row, RectTransform[] keys, float keyW, float keyH, float spacingBetweenKeys, float offsetX)
    {
        float currentX = offsetX;
        foreach (var key in keys)
        {
            key.sizeDelta = new Vector2(keyW, keyH);
            key.anchoredPosition = new Vector2(currentX, 0f);
            currentX += (keyW + spacingBetweenKeys);
        }
    }



    private void BindAllKeys()
    {
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

        //Row4: CapsLock, Z–M, Backspace
        keyCapsLock.GetComponent<Button>().onClick.AddListener(ToggleCapsLock);
        keyZ.GetComponent<Button>().onClick.AddListener(() => Append('Z'));
        keyX.GetComponent<Button>().onClick.AddListener(() => Append('X'));
        keyC.GetComponent<Button>().onClick.AddListener(() => Append('C'));
        keyV.GetComponent<Button>().onClick.AddListener(() => Append('V'));
        keyB.GetComponent<Button>().onClick.AddListener(() => Append('B'));
        keyN.GetComponent<Button>().onClick.AddListener(() => Append('N'));
        keyM.GetComponent<Button>().onClick.AddListener(() => Append('M'));
        keyBackspace.GetComponent<Button>().onClick.AddListener(() => Backspace());

        //Row5: Space, Enter
        keySpace.GetComponent<Button>().onClick.AddListener(() => Append(' '));
        keyEnter.GetComponent<Button>().onClick.AddListener(() => ToggleConfirmation());


        //Confirmation 
        buttonYes.GetComponent<Button>().onClick.AddListener(() => Submit());
        buttonNo.GetComponent<Button>().onClick.AddListener(() => ToggleConfirmation());
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
        InputText += character;

        if(isCapsOn && InputText.Length == 1)
        {
            ToggleCapsLock();
        }

    }

    void Backspace()
    {
        if (InputText.Length > 0)
            InputText = InputText.Substring(0, InputText.Length - 1);
    }

    void ToggleConfirmation()
    {
        if (InputText.Length < 1)
            return;

        var isShowing = confirmationContainer.gameObject.activeSelf;
        var show = !isShowing;
        keysContainer.gameObject.SetActive(!show);
        confirmationContainer.gameObject.SetActive(show);
    }


    void Submit()
    {
        Debug.Log(InputText);
    }

    public string GetSanitizedInput()
    {
        return SanitizeInput(InputText);
    }

    private string SanitizeInput(string input)
    {
        // Remove HTML-like tags to avoid markup injection
        return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
    }


    private void FixedUpdate()
    {
        ResizeUI();
    }

}
