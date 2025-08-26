using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.Scripts.Canvas.Timeline
{
    /// <summary>
    /// Visual controller for a single fixed-size timeline block.
    /// Supports hero blocks and enemy blocks with optional masked portrait.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TimelineBlockInstance : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField] private Image back;
        [SerializeField] private Image portrait;
        [SerializeField] private Mask portraitMask;
        [SerializeField] private TMP_Text label;

        [Header("Portrait")]
        [Tooltip("Scale multiplier for the portrait relative to block size.")]
        [SerializeField] private float portraitScale = 1.6f;

        public RectTransform Rect { get; private set; }

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Set the block to a square of given pixel size. Height kept for compatibility,
        /// but the smaller of width and height is used to force a square.
        /// </summary>
        public void SetSize(float width, float height)
        {
            // Left-anchored, left-pivot so x = left edge
            Rect.anchorMin = new Vector2(0f, 0.5f);
            Rect.anchorMax = new Vector2(0f, 0.5f);
            Rect.pivot = new Vector2(0f, 0.5f);

            float size = Mathf.Round(Mathf.Min(width, height));
            Rect.sizeDelta = new Vector2(size, size);

            if (portrait != null)
            {
                var pr = portrait.rectTransform;
                // Grow from center of the block
                pr.anchorMin = new Vector2(0.5f, 0.5f);
                pr.anchorMax = new Vector2(0.5f, 0.5f);
                pr.pivot = new Vector2(0.5f, 0.5f);
                pr.anchoredPosition = Vector2.zero;

                float p = size * portraitScale;
                pr.sizeDelta = new Vector2(p, p);
            }
        }

        /// <summary>
        /// Apply style for hero or enemy.
        /// </summary>
        public void SetStyle(bool isHero, Color tint)
        {
            if (back != null)
                back.color = isHero ? Color.white : tint;

            if (label != null)
                label.color = isHero ? Color.black : Color.white;
        }

        /// <summary>
        /// Set the center label text.
        /// </summary>
        public void SetLabel(string text)
        {
            if (label != null)
                label.text = text ?? string.Empty;
        }

        /// <summary>
        /// Assign or hide the portrait. Mask is toggled with it.
        /// </summary>
        public void SetPortrait(Sprite sprite, bool enabled)
        {
            if (portrait == null || portraitMask == null)
                return;

            portraitMask.enabled = enabled;
            portrait.enabled = enabled;

            if (enabled)
                portrait.sprite = sprite;
        }
    }
}
