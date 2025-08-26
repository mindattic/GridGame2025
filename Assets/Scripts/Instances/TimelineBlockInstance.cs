// --- File: Assets/Scripts/Canvas/Timeline/TimelineBlockInstance.cs ---
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.Scripts.Canvas.Timeline
{
    /// <summary>
    /// Visual instance for a timeline block.
    /// - Square block footprint.
    /// - Mask crops the portrait.
    /// - Portrait is offset so only the top half is visible.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TimelineBlockInstance : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField] private Image back;
        [SerializeField] private Image portrait;
        [SerializeField] private Mask portraitMask;
        [SerializeField] private TMP_Text label;

        public RectTransform Rect { get; private set; }

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
            EnforceSquare();
            ConfigureMask();
        }

        /// <summary>
        /// Set label, tint, and portrait in one call.
        /// </summary>
        public void Set(string text, Color tint, Sprite portraitSprite)
        {
            SetLabel(text);
            SetStyle(tint);
            SetPortraitTopHalf(portraitSprite);
            EnforceSquare();
            ConfigureMask();
        }

        /// <summary>
        /// Resize the block.
        /// </summary>
        public void Resize(float width, float height)
        {
            if (Rect != null) Rect.sizeDelta = new Vector2(width, height);
            ConfigureMask();
            FitPortraitRect();
        }

        /// <summary>
        /// Ensure the mask is square of the given size.
        /// </summary>
        public void SetSquareMask(float size)
        {
            if (Rect != null) Rect.sizeDelta = new Vector2(size, size);
            ConfigureMask();
            FitPortraitRect();
        }

        private void SetStyle(Color tint)
        {
            if (back != null) back.color = tint;
            if (label != null) label.color = Color.white;
        }

        private void SetLabel(string text)
        {
            if (label != null) label.text = text;
        }

        private void EnforceSquare()
        {
            if (Rect == null) return;

            float h = Mathf.Max(1f, Rect.rect.height);
            Rect.sizeDelta = new Vector2(h, h);

            Rect.anchorMin = new Vector2(0f, 0.5f);
            Rect.anchorMax = new Vector2(0f, 0.5f);
            Rect.pivot = new Vector2(0f, 0.5f);
        }

        private void ConfigureMask()
        {
            if (portraitMask == null) return;

            var mr = portraitMask.GetComponent<RectTransform>();
            if (mr == null || Rect == null) return;

            mr.anchorMin = new Vector2(0.5f, 0.5f);
            mr.anchorMax = new Vector2(0.5f, 0.5f);
            mr.pivot = new Vector2(0.5f, 0.5f);
            mr.sizeDelta = Rect.sizeDelta;
            mr.anchoredPosition = Vector2.zero;
            portraitMask.enabled = true;
        }

        private void FitPortraitRect()
        {
            if (portrait == null || Rect == null) return;

            float s = Mathf.Min(Rect.rect.width, Rect.rect.height);
            var pr = portrait.rectTransform;

            pr.anchorMin = new Vector2(0.5f, 0.5f);
            pr.anchorMax = new Vector2(0.5f, 0.5f);
            pr.pivot = new Vector2(0.5f, 0.5f);

            // Double height and shift down so the top half shows inside the square mask.
            pr.sizeDelta = new Vector2(s, s * 2f);
            pr.anchoredPosition = new Vector2(0f, -s * 0.5f);

            pr.localRotation = Quaternion.identity;
            pr.localScale = Vector3.one;
        }

        /// <summary>
        /// Assign the portrait sprite and crop to the top half using mask and offset.
        /// </summary>
        private void SetPortraitTopHalf(Sprite sprite)
        {
            bool enabled = sprite != null;

            if (portrait != null)
            {
                portrait.enabled = enabled;
                portrait.sprite = sprite;
            }

            if (portraitMask != null) portraitMask.enabled = enabled;

            FitPortraitRect();
        }
    }
}
