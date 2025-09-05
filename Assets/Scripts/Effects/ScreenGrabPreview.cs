using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple: capture the screen and show it fullscreen on a RawImage so you can verify the screenshot path.
/// Attach this to a GameObject and call Show() to display the captured frame.
/// </summary>
[RequireComponent(typeof(ScreenGrabber))]
public class ScreenGrabPreview : MonoBehaviour
{
    [Tooltip("Optional target canvas. If null, a temporary Screen Space - Overlay canvas will be created.")]
    public Canvas targetCanvas;

    [Tooltip("Sorting order for the preview canvas.")]
    public int sortingOrder = 50000;

    private RawImage _image;
    private GameObject _tempCanvasGO;

    public void Show()
    {
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        Texture2D shot = null;
        yield return StartCoroutine(GetComponent<ScreenGrabber>().CaptureToTexture(t => shot = t));

        if (shot == null)
        {
            Debug.LogError("ScreenGrabPreview: Capture failed (Texture2D is null)");
            yield break;
        }

        // Get or create a canvas
        Canvas canvas = targetCanvas;
        if (canvas == null)
        {
            _tempCanvasGO = new GameObject("ScreenGrabPreviewCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = _tempCanvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
        }

        // Create RawImage to display the screenshot full-screen
        var go = new GameObject("ScreenGrabPreview", typeof(RectTransform), typeof(RawImage));
        go.transform.SetParent(canvas.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        _image = go.GetComponent<RawImage>();
        _image.texture = shot;
        _image.raycastTarget = false;

        Debug.Log($"ScreenGrabPreview: Showing {shot.width}x{shot.height} screenshot on overlay RawImage.");
    }

    /// <summary>
    /// Hides the preview and destroys the captured texture.
    /// </summary>
    public void Hide()
    {
        if (_image != null)
        {
            var tex = _image.texture as Texture2D;
            _image.texture = null;
            if (tex != null) Destroy(tex);
            Destroy(_image.gameObject);
            _image = null;
        }
        if (_tempCanvasGO != null)
        {
            Destroy(_tempCanvasGO);
            _tempCanvasGO = null;
        }
    }
}
