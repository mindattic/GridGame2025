using Assets.Scripts.Models;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class StatsDisplay : MonoBehaviour
{

    private RectTransform levelRow;
    private RectTransform hpRow;
    private RectTransform strRow;
    private RectTransform vitRow;
    private RectTransform agiRow;
    private RectTransform spdRow;
    private RectTransform lckRow;

    private float centeredX;

    private void Awake()
    {
        var statsDisplay = GameObject.Find(ComponentHelper.PartyManager.StatsDisplay).GetComponent<RectTransform>();
       

        levelRow = statsDisplay.transform.GetChild("LVL").GetComponent<RectTransform>();
        hpRow = statsDisplay.transform.GetChild("HP").GetComponent<RectTransform>();
        strRow = statsDisplay.transform.GetChild("STR").GetComponent<RectTransform>();
        vitRow = statsDisplay.transform.GetChild("VIT").GetComponent<RectTransform>();
        agiRow = statsDisplay.transform.GetChild("AGI").GetComponent<RectTransform>();
        spdRow = statsDisplay.transform.GetChild("SPD").GetComponent<RectTransform>();
        lckRow = statsDisplay.transform.GetChild("LCK").GetComponent<RectTransform>();


        float parentWidth = statsDisplay.rect.width;
        float barBackWidth = levelRow.rect.width;
        centeredX = (parentWidth - barBackWidth) / 2;
        Debug.Log(centeredX);

       
    }

    /// <summary>
    /// Updates the stats display with the given ActorStats.
    /// </summary>
    /// <param name="stats">The ActorStats object containing the stats to display.</param>
    public void Load(string character, int level)
    {
        var actorData = ActorRepo.instance.Actors[character];
        var stats = actorData.GetStats(level);
  
        // Update each stat row
        UpdateStatRow(levelRow, "LVL", stats.Level); // Assuming max level is 100
        UpdateStatRow(hpRow, "HP", stats.HP, stats.MaxHP);
        UpdateStatRow(strRow, "STR", stats.Strength); // Assuming max stat value is 100
        UpdateStatRow(vitRow, "VIT", stats.Vitality);
        UpdateStatRow(agiRow, "AGI", stats.Agility);
        UpdateStatRow(spdRow, "SPD", stats.Speed);
        UpdateStatRow(lckRow, "LCK", stats.Luck);
    }

    /// <summary>
    /// Updates a single stat row with the given values.
    /// </summary>
    /// <param name="row">The RectTransform of the stat row.</param>
    /// <param name="label">The label for the stat (e.g., "STR").</param>
    /// <param name="value">The current value of the stat.</param>
    /// <param name="maxValue">The maximum value of the stat.</param>
    private void UpdateStatRow(RectTransform row, string label, float value, float maxValue = 99)
    {
        //row.anchoredPosition = new Vector2(centeredX, row.anchoredPosition.y);

        // Update the label text
        var labelComponent = row.Find("Label").GetComponent<TextMeshProUGUI>();
        labelComponent.text = label;

        // Update the bar fill amount
        var backImage = row.Find("Bar/Back").GetComponent<Image>();
        var fillImage = row.Find("Bar/Fill").GetComponent<Image>();
        var width = backImage.rectTransform.rect.width * (value / maxValue);
        var height = backImage.rectTransform.rect.height;
        fillImage.rectTransform.sizeDelta = new Vector2(width, 32);

        // Update the value text
        var valueComponent = row.Find("Value").GetComponent<TextMeshProUGUI>();
        valueComponent.text = $"{value}/{maxValue}";
    }
}