using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using f = Assets.Helpers.FadeOverlayHelper;


/// <summary>
/// Captures the screen, shows it full screen, and drives a zoom/spin/smudge shader over time.
/// Works with URP (uses "Ryan/ZoomShaderURP"). Optionally disables overlay canvases so the effect is visible.
/// </summary>
[RequireComponent(typeof(ScreenGrabber))]
public class ZoomEffect : MonoBehaviour
{
    [Header("Material")]
    public Material zoomMaterial; // Optional. If null, a material is created from the URP shader.

    [Header("Rendering")]
    [Tooltip("Layer to place the renderer on. Use a layer visible to the main camera.")]
    public string renderLayerName = "Default";

    [Tooltip("Temporarily disable all Screen Space - Overlay Canvases during the effect so it is visible.")]
    public bool disableOverlayCanvases = true;

    [Tooltip("Temporarily hide FadeOverlayInstance Images during the capture to avoid a black capture.")]
    public bool hideFadeOverlayImages = true;

    [Tooltip("Parent the quad to the main camera so its transform never culls it.")]
    public bool parentToCamera = true;

    [Header("Motion")]
    public float duration = 0.75f;
    public float zoomStrength = 2.0f; // final zoom amount contribution
    public float spinRadians = 6.0f;  // spin over the duration (radians)
    public float smudgeStrength = 1.0f;
    public Vector2 centerUV = new Vector2(0.5f, 0.5f);

    [Header("Capture")]
    [Tooltip("Flip the captured image vertically to correct upside-down orientation.")]
    public bool flipY = true;

    private const string URPShaderPath = "Ryan/ZoomShaderURP";

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

        // 2) Build mesh object (simple full-screen grid)
        var go = new GameObject("ZoomFullScreen");

        // Place on a camera-visible layer (default to "Default"). Avoid inheriting UI layer accidentally.
        int targetLayer = LayerMask.NameToLayer(string.IsNullOrEmpty(renderLayerName) ? "Default" : renderLayerName);
        if (targetLayer < 0) targetLayer = LayerMask.NameToLayer("Default");
        go.layer = targetLayer;

        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();

        // Small grid to allow good interpolation if needed
        mf.sharedMesh = ShardMeshBuilder.BuildGrid(2, 2);

        // Parent to main camera so culling and transform are trivial
        if (parentToCamera && Camera.main != null)
        {
            go.transform.SetParent(Camera.main.transform, worldPositionStays: false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }

        // 3) Setup material instance
        Material matInst = null;
        if (zoomMaterial != null)
        {
            matInst = new Material(zoomMaterial);
        }
        else
        {
            var shader = Shader.Find(URPShaderPath);
            if (shader != null)
            {
                matInst = new Material(shader);
            }
        }

        if (matInst == null)
        {
            Debug.LogError("ZoomEffect: Could not create material. Assign zoomMaterial or add the Ryan/ZoomShaderURP shader.");
            Destroy(go);
            if (frame != null) Destroy(frame);

            // Restore UI state before exiting
            for (int i = 0; i < disabledCanvases.Count; i++) disabledCanvases[i].enabled = true;
            for (int i = 0; i < fadedImages.Count; i++) fadedImages[i].img.enabled = fadedImages[i].wasEnabled;
            yield break;
        }

        matInst.mainTexture = frame;
        matInst.SetFloat("_Progress", 0f);
        matInst.SetFloat("_Zoom", zoomStrength);
        matInst.SetFloat("_Spin", spinRadians);
        matInst.SetFloat("_Smudge", smudgeStrength);
        matInst.SetVector("_CenterUV", new Vector4(centerUV.x, centerUV.y, 0, 0));
        matInst.SetFloat("_FlipY", flipY ? 1f : 0f);
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
