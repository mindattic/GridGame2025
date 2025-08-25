using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.Scripts.Canvas.Timeline
{
    /// <summary>
    /// TimelineBlockInstance controls visuals for a single rectangle in the timeline.
    /// Supports hero blocks and enemy blocks with optional masked portrait.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TimelineBlockInstance : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField] private Image back;            // Solid rectangle
        [SerializeField] private Image portrait;        // Masked portrait image
        [SerializeField] private Mask portraitMask;     // UI Mask on the portrait parent
        [SerializeField] private TMP_Text label;        // Center label text

        public RectTransform Rect { get; private set; }

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Set the visual size in pixels for this block.
        /// Height is the viewport height to create a continuous strip.
        /// </summary>
        public void SetSize(float width, float height)
        {
            Rect.sizeDelta = new Vector2(width, height);
        }

        /// <summary>
        /// Apply style for hero or enemy.
        /// Hero: white block with no portrait.
        /// Enemy: tinted block with portrait overlay.
        /// </summary>
        public void SetStyle(bool isHero, Color tint)
        {
            back.color = isHero ? Color.white : tint;
            if (label != null)
                label.color = isHero ? Color.black : Color.white;
        }

        /// <summary>
        /// Set the label text shown inside the block.
        /// </summary>
        public void SetLabel(string text)
        {
            if (label != null)
                label.text = text;
        }

        /// <summary>
        /// Assign an enemy portrait and enable or disable the mask group.
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
