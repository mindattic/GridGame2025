using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.Scripts.Canvas
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class TimelineTag : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField] private Image image;
        [SerializeField] private CanvasGroup canvasGroup; // for fade-out
        [Tooltip("Optional: Text label (TMP) shown below the tag to display seconds remaining.")]
        [SerializeField] private TextMeshProUGUI label;

        [Header("Runtime")]
        public ActorInstance Owner; // enemy owning this tag
        public RectTransform Rect { get; private set; }

        // Normalized motion state (resolution-independent)
        private float leftX; // bar-local x at u=0 (left edge target)
        private float spawnX; // bar-local x at u=1 (right edge spawn)
        private float u; // normalized position in [0..1],1 = far right,0 = left
        private float uPerSec; // normalized speed per second (u units per sec)

        private System.Action<TimelineTag> onReached;
        private bool isFading;
        private bool paused;
        private bool fired;

        // Tolerance for deciding a tag reached the left edge (in local pixels)
        private const float ReachTolerance = 0.25f;

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            // Prefer exact-name children, then fall back to any in-tree
            if (image == null)
            {
                var imgTf = transform.Find("Image");
                image = imgTf != null ? imgTf.GetComponent<Image>() : GetComponentInChildren<Image>(true);
                if (image == null)
                    Debug.LogWarning("TimelineTag: Child Image not found. Add an Image child or assign `body`.", this);
            }
            if (label == null)
            {
                var lblTf = transform.Find("Label");
                label = lblTf != null ? lblTf.GetComponent<TextMeshProUGUI>() : GetComponentInChildren<TextMeshProUGUI>(true);
                if (label == null)
                    Debug.LogWarning("TimelineTag: Child Label (TextMeshProUGUI) not found. Add a Label child or assign `label`.", this);
            }

            // Optional: do not intercept clicks
            if (image != null) image.raycastTarget = false;
            if (label != null) label.raycastTarget = false;

            // Left-edge pivot so anchoredPosition.x represents the tag's LEFT edge exactly
            if (Rect != null)
            {
                Rect.anchorMin = new Vector2(0f, 0.5f);
                Rect.anchorMax = new Vector2(0f, 0.5f);
                Rect.pivot = new Vector2(0f, 0.5f); // changed from0.5f to0f for precise alignment
            }
            // Ignore layout so manual positioning is preserved
            var le = gameObject.GetComponent<LayoutElement>();
            if (le == null) le = gameObject.AddComponent<LayoutElement>();
            le.ignoreLayout = true;
        }

        public void Wire(Image bodyImage, CanvasGroup group)
        {
            if (bodyImage != null) image = bodyImage;
            if (group != null) canvasGroup = group;
            if (label == null)
            {
                var lblTf = transform.Find("Label");
                label = lblTf != null ? lblTf.GetComponent<TextMeshProUGUI>() : GetComponentInChildren<TextMeshProUGUI>(true);
            }
            if (image == null)
            {
                var imgTf = transform.Find("Image");
                image = imgTf != null ? imgTf.GetComponent<Image>() : GetComponentInChildren<Image>(true);
            }
            if (image != null) image.raycastTarget = false;
            if (label != null) label.raycastTarget = false;
        }

        // Initialize using normalized coordinates and speed
        public void InitializeNormalized(ActorInstance owner, float leftX, float spawnX, float startU, float uPerSec, System.Action<TimelineTag> onReached)
        {
            Owner = owner;
            this.leftX = leftX;
            this.spawnX = Mathf.Max(spawnX, leftX + 1f);
            this.u = Mathf.Clamp01(startU);
            this.uPerSec = Mathf.Max(0.0001f, uPerSec);
            this.onReached = onReached;
            if (canvasGroup != null) canvasGroup.alpha = 1f;
            isFading = false;
            paused = true; // start paused; TimelineBar controls advance
            fired = false;
            ApplyPosition();
            UpdateLabel();
        }

        // Backward-compatible initializer
        public void Initialize(ActorInstance owner, float leftEdgeX, float startX, float moveSpeedPxPerSec, System.Action<TimelineTag> onReached)
        {
            float width = Mathf.Max(1f, spawnX - leftEdgeX);
            float startU = Mathf.InverseLerp(leftEdgeX, spawnX, startX);
            float uSpeed = Mathf.Abs(moveSpeedPxPerSec) / width;
            InitializeNormalized(owner, leftEdgeX, spawnX, startU, uSpeed, onReached);
        }

        public void UpdateEndpoints(float newLeftX, float newSpawnX)
        {
            // Preserve normalized u while endpoints shift
            leftX = newLeftX;
            spawnX = Mathf.Max(newSpawnX, newLeftX + 1f);
            ApplyPosition();
            UpdateLabel();
        }

        public void Pause() => paused = true;
        public void Resume() => paused = false;
        public void SetAlpha(float a) { if (canvasGroup != null) canvasGroup.alpha = Mathf.Clamp01(a); }

        // Reset this tag's trigger state so it can fire on the next cycle
        public void ResetForNextCycle()
        {
            fired = false;
            UpdateLabel();
        }

        // Set anchored x from normalized u (left-edge pivot guarantees alignment)
        private void ApplyPosition()
        {
            if (Rect == null) return;
            float xLeft = Mathf.Lerp(leftX, spawnX, Mathf.Clamp01(u));
            var p = Rect.anchoredPosition;
            Rect.anchoredPosition = new Vector2(xLeft, p.y);
        }

        public void SetX(float xLeft)
        {
            if (Rect != null)
            {
                var p = Rect.anchoredPosition;
                Rect.anchoredPosition = new Vector2(xLeft, p.y);
                u = (spawnX - leftX) > 0.0001f ? Mathf.InverseLerp(leftX, spawnX, xLeft) : u;
            }
            else
            {
                var lp = transform.localPosition;
                transform.localPosition = new Vector3(xLeft, lp.y, lp.z);
            }
            UpdateLabel();
        }

        public void SetU(float value)
        {
            u = Mathf.Clamp01(value);
            ApplyPosition();
            UpdateLabel();
        }

        public float GetU() => u;
        public float GetUPerSec() => uPerSec;
        public float GetSecondsRemaining() => uPerSec <= 0f ? 0f : Mathf.Max(0f, u / uPerSec);

        private void Update()
        {
            if (!isFading && !paused)
            {
                // Move toward left (u =0)
                u = Mathf.MoveTowards(u, 0f, uPerSec * Time.deltaTime);
                ApplyPosition();
            }
            // Update label after we potentially moved this frame
            UpdateLabel();

            // Left-edge strict check using anchoredPosition.x (left pivot)
            if (!fired && Rect != null && Rect.anchoredPosition.x <= leftX + ReachTolerance)
            {
                fired = true;
                onReached?.Invoke(this);
            }
        }

        private void UpdateLabel()
        {
            if (label == null) return;
            float sec = GetSecondsRemaining();
            label.text = sec.ToString("0.0");
        }

        public void FadeAndDestroy(float duration = 0.25f)
        {
            if (isFading) return;
            isFading = true;
            StartCoroutine(FadeOutAndDestroy(duration));
        }

        private IEnumerator FadeOutAndDestroy(float duration)
        {
            float t = 0f;
            float start = canvasGroup != null ? canvasGroup.alpha : 1f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float a = Mathf.Lerp(start, 0f, Mathf.Clamp01(t / duration));
                if (canvasGroup != null) canvasGroup.alpha = a;
                else if (image != null) image.color = new Color(image.color.r, image.color.g, image.color.b, a);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
