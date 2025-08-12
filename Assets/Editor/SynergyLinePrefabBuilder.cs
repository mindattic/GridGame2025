#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SynergyLinePrefabBuilder
{
    private const string RootFolder = "Assets/Prefabs/Synergy";
    private const string MatFolder = "Assets/Materials/Synergy";

    [MenuItem("Tools/Synergy/Build Synergy Prefabs")]
    public static void BuildAll()
    {
        EnsureFolders();

        // Material
        var mat = CreateOrLoadMaterial($"{MatFolder}/SynergyAdditive.mat");

        // Prefabs
        var segPrefab = Build_SynergyLineSegment(mat, $"{RootFolder}/SynergyLineSegment.prefab");
        var linePrefab = Build_SynergyLine(segPrefab, $"{RootFolder}/SynergyLine.prefab");
        var mgrPrefab = Build_SynergyLineManager(linePrefab, $"{RootFolder}/SynergyLineManager.prefab");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "Synergy prefabs created:\n" +
            AssetDatabase.GetAssetPath(segPrefab) + "\n" +
            AssetDatabase.GetAssetPath(linePrefab) + "\n" +
            AssetDatabase.GetAssetPath(mgrPrefab)
        );
    }

    private static void EnsureFolders()
    {
        CreateFolderIfMissing("Assets/Prefabs");
        CreateFolderIfMissing(RootFolder);
        CreateFolderIfMissing("Assets/Materials");
        CreateFolderIfMissing(MatFolder);
        CreateFolderIfMissing("Assets/Editor");
    }

    private static void CreateFolderIfMissing(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        var parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
        var name = Path.GetFileName(path);
        if (!string.IsNullOrEmpty(parent) && !string.IsNullOrEmpty(name))
            AssetDatabase.CreateFolder(parent, name);
    }

    private static Material CreateOrLoadMaterial(string matPath)
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat != null) return mat;

        // Use URP Unlit if available, fall back to Unlit/Color
        var shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        mat = new Material(shader);

        // Teal additive look. Works for URP Unlit and legacy Unlit/Color.
        TrySetColor(mat, new Color(0.20f, 1.00f, 0.90f, 0.60f));
        // Common additive settings (URP compatible properties if present)
        TrySetFloat(mat, "_Surface", 1f); // 0 Opaque, 1 Transparent
        TrySetFloat(mat, "_ZWrite", 0f);
        TrySetFloat(mat, "_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
        TrySetFloat(mat, "_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
        TrySetFloat(mat, "_Cull", (float)UnityEngine.Rendering.CullMode.Off);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        AssetDatabase.CreateAsset(mat, matPath);
        return mat;
    }

    private static GameObject Build_SynergyLineSegment(Material mat, string prefabPath)
    {
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existing != null) return existing;

        var go = new GameObject("SynergyLineSegment");

        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.textureMode = LineTextureMode.Stretch;
        lr.alignment = LineAlignment.View;
        lr.numCornerVertices = 3;
        lr.numCapVertices = 3;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.sharedMaterial = mat;
        lr.sortingLayerName = "Default";  // runtime will follow actors
        lr.sortingOrder = 200;
        lr.widthMultiplier = 0.045f;

        go.AddComponent<SynergyLineSegment>();

        var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject Build_SynergyLine(GameObject segPrefab, string prefabPath)
    {
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existing != null) return existing;

        var go = new GameObject("SynergyLine");
        var line = go.AddComponent<SynergyLineInstance>();

        var so = new SerializedObject(line);
        // Assign segment prefab
        so.FindProperty("segmentPrefab").objectReferenceValue = segPrefab.GetComponent<SynergyLineSegment>();

        // Group settings
        so.FindProperty("waveformCount").intValue = 6;
        so.FindProperty("durationSeconds").floatValue = 2.0f;
        so.FindProperty("baseRadius").floatValue = 0.16f;
        so.FindProperty("baseWidth").floatValue = 0.05f;
        so.FindProperty("frequency").floatValue = 7.5f;

        // Noise
        so.FindProperty("noiseAmplitude").floatValue = 0.06f;
        so.FindProperty("noiseScale").floatValue = 1.0f;
        so.FindProperty("noiseSpeed").floatValue = 1.2f;

        // Fade
        so.FindProperty("fadeInTime").floatValue = 0.25f;
        so.FindProperty("fadeOutTime").floatValue = 0.25f;

        // Sorting follow params
        so.FindProperty("orderOffsetPerWave").intValue = 1;
        so.FindProperty("extraFrontBias").intValue = 0;

        // Gradients for Lifestream vibe
        SetGradient(so.FindProperty("gradientStrength"),
            new Color(0.20f, 1.00f, 0.90f, 0.90f),
            new Color(0.95f, 1.00f, 1.00f, 0.90f));
        SetGradient(so.FindProperty("gradientVitality"),
            new Color(0.25f, 0.95f, 1.00f, 0.85f),
            new Color(0.85f, 1.00f, 0.95f, 0.90f));
        SetGradient(so.FindProperty("gradientStamina"),
            new Color(0.30f, 1.00f, 0.85f, 0.85f),
            new Color(0.90f, 1.00f, 0.95f, 0.90f));
        SetGradient(so.FindProperty("gradientIntelligence"),
            new Color(0.28f, 0.95f, 0.98f, 0.85f),
            new Color(0.85f, 0.98f, 0.95f, 0.90f));
        SetGradient(so.FindProperty("gradientWisdom"),
            new Color(0.32f, 1.00f, 0.90f, 0.85f),
            new Color(0.95f, 1.00f, 1.00f, 0.90f));
        SetGradient(so.FindProperty("gradientLuck"),
            new Color(0.24f, 0.98f, 0.92f, 0.85f),
            new Color(0.88f, 1.00f, 0.98f, 0.90f));

        so.ApplyModifiedPropertiesWithoutUndo();

        var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject Build_SynergyLineManager(GameObject linePrefab, string prefabPath)
    {
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existing != null) return existing;

        var go = new GameObject("SynergyLineManager");
        var mgr = go.AddComponent<SynergyLineManager>();

        var so = new SerializedObject(mgr);
        so.FindProperty("linePrefab").objectReferenceValue = linePrefab.GetComponent<SynergyLineInstance>();
        so.FindProperty("initialPool").intValue = 2;
        so.ApplyModifiedPropertiesWithoutUndo();

        var prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static void SetGradient(SerializedProperty gradProp, Color c0, Color c1)
    {
        var g = new Gradient();
        g.SetKeys(
            new[]
            {
                new GradientColorKey(c0, 0f),
                new GradientColorKey(c1, 1f)
            },
            new[]
            {
                new GradientAlphaKey(c0.a, 0f),
                new GradientAlphaKey(c1.a, 1f)
            }
        );
        gradProp.gradientValue = g;
    }

    private static void TrySetColor(Material m, Color c)
    {
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        else if (m.HasProperty("_Color")) m.SetColor("_Color", c);
    }

    private static void TrySetFloat(Material m, string prop, float value)
    {
        if (m.HasProperty(prop)) m.SetFloat(prop, value);
    }
}
#endif
