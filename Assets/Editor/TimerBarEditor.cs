#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimerBar))]
public class TimerBarEditor : Editor
{
    private const float timerBarHeight = 30f;
    private const float barWidth = 300f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Auto Configure Timer Bar"))
        {
            TimerBar timerBar = (TimerBar)target;
            Configure(timerBar);
        }
    }

    private void Configure(TimerBar timerBar)
    {
        RectTransform root = (RectTransform)timerBar.transform;

        // Stretch TimerBar to 95% of screen width, anchored to top
        root.anchorMin = new Vector2(0.025f, 1f);
        root.anchorMax = new Vector2(0.975f, 1f);
        root.pivot = new Vector2(0.5f, 1f);
        root.anchoredPosition = new Vector2(0f, -20f);
        root.sizeDelta = new Vector2(0f, timerBarHeight); // Set visible height

        // Configure children
        ConfigureChild(timerBar.transform, "Back", new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), Vector2.zero);
        ConfigureChild(timerBar.transform, "Front", new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), Vector2.zero);
        ConfigureChild(timerBar.transform, "Bar", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(barWidth, timerBarHeight));
    }

    private void ConfigureChild(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 size)
    {
        Transform child = parent.Find(name);
        if (child == null)
        {
            Debug.LogWarning($"Child '{name}' not found.");
            return;
        }

        RectTransform rt = child.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;
        rt.localScale = Vector3.one;
    }
}
#endif
