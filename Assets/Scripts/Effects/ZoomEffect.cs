using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives a full-screen shatter transition by capturing the frame, rendering a shard grid, and animating a shader.
/// Works with URP (uses "Universal Render Pipeline/Unlit/ScreenShatter") and falls back to Built-in ("Unlit/ScreenShatter").
/// Note: If you use a Screen Space - Overlay Canvas, it will render on top of this effect. This script can temporarily disable such canvases.
/// </summary>
[RequireComponent(typeof(ScreenGrabber))]
public class ZoomEffect : MonoBehaviour
{
    [Header("Grid")]
    public int cols = 40;
    public int rows = 22;

    [Header("Material")]
    public Material shatterMat; // Optional. If null, a material is created from the URP shader (or Built-in fallback).

    [Header("Rendering")] 
    [Tooltip("Layer to place the ScreenShards renderer on. Use a layer visible to the main camera.")]
    public string renderLayerName = "Default";

    [Tooltip("Temporarily disable all Screen Space - Overlay Canvases during the shatter so the effect is visible.")]
    public bool disableOverlayCanvases = true;

    [Tooltip("Temporarily hide FadeOverlayInstance Images during the shatter and capture to avoid a full-black capture.")]
    public bool hideFadeOverlayImages = true;

    [Tooltip("Parent the shards to the main camera so their transform never culls them.")]
    public bool parentToCamera = true;

    [Header("Motion")]
    public float duration = 0.6f;
    public float explode = 1.2f;
    public float spin = 9.0f;
    public float jitter = 0.8f;
    public Vector2 centerUV = new Vector2(0.5f, 0.5f);

    private const string URPShaderPath = "Universal Render Pipeline/Unlit/ScreenShatter";
    private const string BuiltinShaderPath = "Unlit/ScreenShatter";

    public IEnumerator Play(System.Action onFinished = null)
    {
        // Optionally disable overlay canvases (they render above everything else)
        var disabledCanvases = new List<Canvas>();
        if (disableOverlayCanvases)
        {
            var canvases = FindObjectsOfType<Canvas>(true);
            for (int i = 0; i < canvases.Length; i++)
            {
                if (canvases[i].renderMode == RenderMode.ScreenSpaceOverlay && canvases[i].enabled)
                {
                    canvases[i].enabled = false;
                    disabledCanvases.Add(canvases[i]);
                }
            }
        }

        // Optionally hide FadeOverlayInstance Images so capture isn't full black
        var fadedImages = new List<(Image img, bool wasEnabled)>();
        if (hideFadeOverlayImages)
        {
            var overlays = FindObjectsOfType<FadeOverlayInstance>(true);
            foreach (var o in overlays)
            {
                var img = o.GetComponent<Image>();
                if (img != null && img.enabled)
                {
                    fadedImages.Add((img, true));
                    img.enabled = false;
                }
            }
        }

        // 1) Grab frame
        Texture2D frame = null;
        yield return StartCoroutine(GetComponent<ScreenGrabber>().CaptureToTexture(t => frame = t));

        // 2) Build mesh object
        var go = new GameObject("ScreenShards");

        // Place on a camera-visible layer (default to "Default"). Avoid inheriting UI layer accidentally.
        int targetLayer = LayerMask.NameToLayer(string.IsNullOrEmpty(renderLayerName) ? "Default" : renderLayerName);
        if (targetLayer < 0) targetLayer = LayerMask.NameToLayer("Default");
        go.layer = targetLayer;

        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();

        mf.sharedMesh = ShardMeshBuilder.BuildGrid(cols, rows);

        // Parent to main camera so culling and transform are trivial
        if (parentToCamera && Camera.main != null)
        {
            go.transform.SetParent(Camera.main.transform, worldPositionStays: false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }

        // 3) Setup material instance (prefer URP shader, fallback to Built-in)
        Material matInst = null;
        if (shatterMat != null)
        {
            matInst = new Material(shatterMat);
        }
        else
        {
            var shader = Shader.Find(URPShaderPath);
            if (shader == null)
                shader = Shader.Find(BuiltinShaderPath);

            if (shader != null)
            {
                matInst = new Material(shader);
            }
        }

        if (matInst == null)
        {
            Debug.LogError("ScreenShatter: Could not create material. Assign shatterMat or add the ScreenShatter shader (URP or Built-in).");
            Destroy(go);
            if (frame != null) Destroy(frame);

            // Restore UI state before exiting
            for (int i = 0; i < disabledCanvases.Count; i++) disabledCanvases[i].enabled = true;
            for (int i = 0; i < fadedImages.Count; i++) fadedImages[i].img.enabled = fadedImages[i].wasEnabled;
            yield break;
        }

        matInst.mainTexture = frame;
        matInst.SetFloat("_Progress", 0f);
        matInst.SetFloat("_Explode", explode);
        matInst.SetFloat("_Spin", spin);
        matInst.SetFloat("_Jitter", jitter);
        matInst.SetVector("_CenterUV", new Vector4(centerUV.x, centerUV.y, 0, 0));
        // Ensure it renders after most transparents
        matInst.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent + 10;
        mr.sharedMaterial = matInst;

        // Make sure it renders on top of world (optional: set sorting order)
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
        mr.sortingOrder = short.MaxValue; // best effort within the same sorting layer

        // 4) Animate
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duration);
            mr.sharedMaterial.SetFloat("_Progress", p);
            yield return null;
        }

        // 5) Cleanup and restore UI
        Object.Destroy(go);
        Object.Destroy(frame);
        for (int i = 0; i < disabledCanvases.Count; i++) disabledCanvases[i].enabled = true;
        for (int i = 0; i < fadedImages.Count; i++) fadedImages[i].img.enabled = fadedImages[i].wasEnabled;
        onFinished?.Invoke();
    }
}
