using UnityEngine;
using UnityEngine.UI;
using Label = TMPro.TextMeshProUGUI;
using TMPro;
using Assets.Helpers;

public class HeroExperiencePane : MonoBehaviour
{
    [Header("Wiring")] public Image Panel;
    public Image Portrait;
    public Label NameLabel;
    public Label LevelLabel;
    public Slider XPBar;
    public Label XPText;
    public Label LevelUpLabel;

    [Header("State")] public bool IsFillComplete; // exposed for VictoryManager

    private int _targetGain;

    private void Awake()
    {
        // Fallback auto-wiring if prefab fields not assigned
        if (Panel == null) Panel = GetComponent<Image>();
        if (Portrait == null) Portrait = transform.Find("Portrait")?.GetComponent<Image>();
        if (NameLabel == null) NameLabel = transform.Find("Name")?.GetComponent<Label>();
        if (LevelLabel == null) LevelLabel = transform.Find("Level")?.GetComponent<Label>();
        if (XPBar == null) XPBar = transform.Find("XPBar")?.GetComponent<Slider>();
        if (XPText == null) XPText = transform.Find("XPText")?.GetComponent<Label>();
        if (LevelUpLabel == null) LevelUpLabel = transform.Find("LevelUp")?.GetComponent<Label>();
    }

    /// <summary>
    /// Configure existing prefab UI (no dynamic hierarchy creation).
    /// </summary>
    public void Build(string character, int xpGained, bool highlight)
    {
        IsFillComplete = false;
        _targetGain = xpGained;
        if (Panel != null)
            Panel.color = highlight ? new Color(0.12f, 0.25f, 0.45f, 0.55f) : new Color(0.1f, 0.1f, 0.1f, 0.4f);

        // Portrait sprite
        if (Portrait != null)
        {
            var sprite = ActorLibrary.Get(character).Portrait;
            //var sprite = AssetHelper.LoadAsset<Sprite>($"{GameHelper.TextureResolution.ToInt()}/{character}");
            Portrait.sprite = sprite;
            Portrait.preserveAspect = true;
        }

        // Fetch save data
        int level = 1, currentXP = 0;
        var save = ProfileHelper.CurrentProfile?.CurrentSave;
        var entry = save?.Party?.Members?.Find(m => m.Character == character) ?? save?.Roster?.Members?.Find(m => m.Character == character);
        if (entry != null)
        {
            level = Mathf.Max(1, entry.Level);
            currentXP = Mathf.Max(0, entry.CurrentXP);
        }

        int needed = Assets.Helpers.ExperienceHelper.NextLevel(level);

        if (NameLabel != null) NameLabel.text = character;
        if (LevelLabel != null) LevelLabel.text = $"Lvl. {level}";
        if (XPBar != null)
        {
            XPBar.minValue = 0;
            XPBar.maxValue = needed;
            XPBar.wholeNumbers = true;
            XPBar.value = Mathf.Clamp(currentXP, 0, needed);
        }
        if (XPText != null) XPText.text = $"EXP: {currentXP} / {needed} (+{xpGained})";
        if (LevelUpLabel != null) LevelUpLabel.color = new Color(1, 0.95f, 0.3f, 0f);

        // Start fill animation (or mark complete immediately if no gain)
        if (xpGained > 0)
            StartCoroutine(FillRoutine(level, currentXP, xpGained));
        else
            IsFillComplete = true;
    }

    private System.Collections.IEnumerator FillRoutine(int level, int currentXP, int gained)
    {
        if (XPBar == null)
        {
            IsFillComplete = true;
            yield break;
        }
        int needed = Assets.Helpers.ExperienceHelper.NextLevel(level);
        int cur = currentXP;
        int remaining = gained;

        while (remaining > 0)
        {
            int step = Mathf.Min(remaining, Mathf.Max(1, needed / 30));
            cur += step;
            remaining -= step;

            if (cur >= needed)
            {
                cur -= needed; level += 1; needed = Assets.Helpers.ExperienceHelper.NextLevel(level);
                XPBar.maxValue = needed;
                if (LevelLabel != null) LevelLabel.text = $"Lvl. {level}";
                if (LevelUpLabel != null) StartCoroutine(FlashLevelUp());
            }

            XPBar.value = cur;
            if (XPText != null) XPText.text = $"EXP: {cur} / {needed} (+{gained})";
            yield return null;
        }

        IsFillComplete = true;
    }

    private System.Collections.IEnumerator FlashLevelUp()
    {
        if (LevelUpLabel == null) yield break;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            LevelUpLabel.color = new Color(1, 0.95f, 0.3f, Mathf.PingPong(t, 0.7f));
            yield return null;
        }
        LevelUpLabel.color = new Color(1, 0.95f, 0.3f, 0);
    }
}
