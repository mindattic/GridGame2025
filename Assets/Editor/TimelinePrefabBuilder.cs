//#if UNITY_EDITOR
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using Assets.Scripts.Canvas.Timeline;
//using System.IO;

//public static class TimelinePrefabBuilder
//{
//    private const string PrefabFolder = "Assets/TimelinePackage/Prefabs";
//    private const string PrefabPath = PrefabFolder + "/TimelineBlockPrefab.prefab";

//    [MenuItem("Tools/Timeline/Create TimelineBlock Prefab")]
//    public static void CreateTimelineBlockPrefab()
//    {
//        if (!AssetDatabase.IsValidFolder("Assets/TimelinePackage"))
//            AssetDatabase.CreateFolder("Assets", "TimelinePackage");

//        if (!AssetDatabase.IsValidFolder("Assets/TimelinePackage/Prefabs"))
//            AssetDatabase.CreateFolder("Assets/TimelinePackage", "Prefabs");

//        // Root
//        var root = new GameObject("TimelineBlockPrefab", typeof(RectTransform));
//        var rect = root.GetComponent<RectTransform>();
//        rect.sizeDelta = new Vector2(200, 120);

//        var block = root.AddComponent<TimelineBlockInstance>();

//        // Back image
//        var backGO = new GameObject("Back", typeof(RectTransform), typeof(Image));
//        backGO.transform.SetParent(root.transform, false);
//        var backRT = backGO.GetComponent<RectTransform>();
//        backRT.anchorMin = Vector2.zero;
//        backRT.anchorMax = Vector2.one;
//        backRT.offsetMin = Vector2.zero;
//        backRT.offsetMax = Vector2.zero;
//        var backImg = backGO.GetComponent<Image>();
//        backImg.color = Color.white;

//        // Portrait group with mask
//        var portraitGroup = new GameObject("PortraitGroup", typeof(RectTransform), typeof(Image), typeof(Mask));
//        portraitGroup.transform.SetParent(root.transform, false);
//        var pgRT = portraitGroup.GetComponent<RectTransform>();
//        pgRT.anchorMin = new Vector2(0, 0);
//        pgRT.anchorMax = new Vector2(0, 1);
//        pgRT.pivot = new Vector2(0, 0.5f);
//        pgRT.sizeDelta = new Vector2(120, 0);
//        var pgImg = portraitGroup.GetComponent<Image>();
//        pgImg.color = new Color(1, 1, 1, 0.08f);
//        var mask = portraitGroup.GetComponent<Mask>();
//        mask.showMaskGraphic = false;

//        var portraitGO = new GameObject("Portrait", typeof(RectTransform), typeof(Image));
//        portraitGO.transform.SetParent(portraitGroup.transform, false);
//        var portraitRT = portraitGO.GetComponent<RectTransform>();
//        portraitRT.anchorMin = new Vector2(0, 0);
//        portraitRT.anchorMax = new Vector2(1, 1);
//        portraitRT.offsetMin = Vector2.zero;
//        portraitRT.offsetMax = Vector2.zero;
//        var portraitImg = portraitGO.GetComponent<Image>();
//        portraitImg.preserveAspect = true;
//        portraitImg.color = Color.white;

//        // Label
//        var labelGO = new GameObject("Label", typeof(RectTransform));
//        labelGO.transform.SetParent(root.transform, false);
//        var labelRT = labelGO.GetComponent<RectTransform>();
//        labelRT.anchorMin = new Vector2(0.2f, 0);
//        labelRT.anchorMax = new Vector2(1, 1);
//        labelRT.offsetMin = Vector2.zero;
//        labelRT.offsetMax = Vector2.zero;

//#if TMP_PRESENT
//        var label = labelGO.AddComponent<TextMeshProUGUI>();
//        label.alignment = TextAlignmentOptions.Center;
//        label.fontSize = 36;
//        label.enableWordWrapping = false;
//        label.text = "Label";
//#else
//        var label = labelGO.AddComponent<Text>();
//        label.alignment = TextAnchor.MiddleCenter;
//        label.fontSize = 24;
//        label.text = "Label";
//#endif

//        // Wire fields
//        block.GetType().GetField("back", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(block, backImg);
//        block.GetType().GetField("portrait", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(block, portraitImg);
//        block.GetType().GetField("portraitMask", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(block, mask);
//#if TMP_PRESENT
//        block.GetType().GetField("label", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(block, label as TMP_Text);
//#else
//        block.GetType().GetField("label", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(block, null);
//#endif

//        // Save as prefab
//        var prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
//        GameObject.DestroyImmediate(root);

//        if (prefab != null)
//            Debug.Log("Created TimelineBlockPrefab at " + PrefabPath);
//        else
//            Debug.LogError("Failed to create TimelineBlockPrefab.");

//        AssetDatabase.SaveAssets();
//        AssetDatabase.Refresh();
//    }

//    [MenuItem("Tools/Timeline/Setup Timeline Under Selected Canvas")]
//    public static void SetupTimelineUnderSelectedCanvas()
//    {
//        var sel = Selection.activeGameObject;
//        if (sel == null || sel.GetComponentInParent<Canvas>() == null)
//        {
//            Debug.LogError("Select a Canvas or a child of a Canvas first.");
//            return;
//        }

//        // Root
//        var root = new GameObject("TimelineRoot", typeof(RectTransform));
//        root.transform.SetParent(sel.transform, false);
//        var rootRT = root.GetComponent<RectTransform>();
//        rootRT.anchorMin = new Vector2(0, 0);
//        rootRT.anchorMax = new Vector2(1, 0);
//        rootRT.pivot = new Vector2(0, 0);
//        rootRT.sizeDelta = new Vector2(0, 120);
//        rootRT.anchoredPosition = new Vector2(0, 0);

//        // Viewport
//        var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image));
//        viewport.transform.SetParent(root.transform, false);
//        var vpRT = viewport.GetComponent<RectTransform>();
//        vpRT.anchorMin = new Vector2(0, 0);
//        vpRT.anchorMax = new Vector2(1, 1);
//        vpRT.offsetMin = Vector2.zero;
//        vpRT.offsetMax = Vector2.zero;
//        var vpImg = viewport.GetComponent<Image>();
//        vpImg.color = new Color(0, 0, 0, 0.35f);

//        // Content
//        var content = new GameObject("Content", typeof(RectTransform));
//        content.transform.SetParent(viewport.transform, false);
//        var ctRT = content.GetComponent<RectTransform>();
//        ctRT.anchorMin = new Vector2(0, 0);
//        ctRT.anchorMax = new Vector2(0, 1);
//        ctRT.pivot = new Vector2(0, 0.5f);
//        ctRT.sizeDelta = new Vector2(0, 0);
//        ctRT.anchoredPosition = new Vector2(0, 0);

//        // Indicator
//        var indicator = new GameObject("Indicator", typeof(RectTransform), typeof(Image));
//        indicator.transform.SetParent(viewport.transform, false);
//        var indRT = indicator.GetComponent<RectTransform>();
//        indRT.anchorMin = new Vector2(0, 0);
//        indRT.anchorMax = new Vector2(0, 1);
//        indRT.pivot = new Vector2(0.5f, 0.5f);
//        indRT.sizeDelta = new Vector2(4, 0);
//        var indImg = indicator.GetComponent<Image>();
//        indImg.color = Color.white;

//        // Add TimelineManager and assign fields if possible
//        var mgr = root.AddComponent<TimelineManager>();

//        // Try to load the prefab asset
//        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TimelinePackage/Prefabs/TimelineBlockPrefab.prefab");
//        var prefabInst = prefab != null ? prefab.GetComponent<TimelineBlockInstance>() : null;

//        // Use reflection to assign serialized fields
//        var mgrType = typeof(TimelineManager);
//        mgrType.GetField("viewport", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(mgr, vpRT);
//        mgrType.GetField("content", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(mgr, ctRT);
//        mgrType.GetField("indicator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(mgr, indImg);
//        if (prefabInst != null)
//            mgrType.GetField("blockPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(mgr, prefabInst);

//        Debug.Log("TimelineRoot created under selected Canvas. Assign fields if any are missing, then call Rebuild at runtime.");
//        Selection.activeGameObject = root;
//    }
//}
//#endif