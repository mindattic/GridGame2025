using Assets.Scripts.Models;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Helper;
using UnityEngine.EventSystems;
using g = Assets.Helpers.GameHelper;

namespace Assets.Scripts.Canvas.Timeline
{
    /// <summary>
    /// Visual instance for a timeline block.
    /// - Square block footprint.
    /// - Mask crops the portrait.
    /// - Portrait is offset so only the top half is visible.
    /// New prefab: Mask (with Image+Mask) -> Portrait (Image), Label (TMP_Text)
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TimelineBlockInstance : MonoBehaviour, IPointerClickHandler
    {
        [Header("Parts")]
        private GameObject maskRoot;    // Root GO for Mask and Portrait (optional; fallback to portraitMask.gameObject)
        private Mask portraitMask;      // On child 'Mask'
        private Image maskImage;        // Image on 'Mask' (used for backs when needed)
        private Image portrait;         // Child of 'Mask'
        private TMP_Text label;         // Sibling of 'Mask'
        private Image backImage;        // Optional separate Back Image (preferred for divider art)
        private GameObject activeIndicatorGO; // current-turn indicator
        private GameObject focusIndicatorGO;  // selection/focus highlight

        [Header("Data")]
        public ActorInstance Owner; // actor this block belongs to (null for dividers)

        public RectTransform Rect { get; private set; }

        private float? portraitYOffsetOverride;
        private bool useThumbnailSettings;  // whether to apply thumbnail settings
        private ThumbnailSettings appliedThumbnail;
        private CanvasThumbnailSettings canvasCrop;

        private void Awake()
        {
            Rect = GetComponent<RectTransform>();

            // Back
            var backTr = transform.Find(GameObjectHelper.TimelineBlock.Back);
            if (backTr != null) backImage = backTr.GetComponent<Image>();

            // Mask
            var maskTr = transform.Find(GameObjectHelper.TimelineBlock.Mask);
            if (maskTr != null)
            {
                maskRoot = maskTr.gameObject;
                portraitMask = maskTr.GetComponent<Mask>();
                maskImage = maskTr.GetComponent<Image>();
            }

            // Portrait under Mask
            var portraitTr = transform.Find(GameObjectHelper.TimelineBlock.Portrait);
            if (portraitTr != null) portrait = portraitTr.GetComponent<Image>();

            // Label
            var labelTr = transform.Find(GameObjectHelper.TimelineBlock.Label);
            if (labelTr != null) label = labelTr.GetComponent<TMP_Text>();

            // Indicators
            var actTr = transform.Find(GameObjectHelper.TimelineBlock.ActiveIndicator);
            if (actTr != null) activeIndicatorGO = actTr.gameObject;
            if (activeIndicatorGO != null) activeIndicatorGO.SetActive(false);

            var focTr = transform.Find(GameObjectHelper.TimelineBlock.FocusIndicator);
            if (focTr != null) focusIndicatorGO = focTr.gameObject;
            if (focusIndicatorGO != null) focusIndicatorGO.SetActive(false);

            if (maskRoot == null && portraitMask != null)
                maskRoot = portraitMask.gameObject;
            if (portrait != null) portrait.preserveAspect = false; // ensure rect size drives the visual

            EnforceSquare();
            ConfigureMask();
        }

        public void SetOwner(ActorInstance actor)
        {
            Owner = actor;
        }

        // Active indicator (current turn)
        public void ShowActiveIndicator(bool on = true)
        { if (activeIndicatorGO != null) activeIndicatorGO.SetActive(on); }
        public void HideActiveIndicator() => ShowActiveIndicator(false);
        public void SetCurrent(bool on) => ShowActiveIndicator(on);

        // Focus indicator (selected for inspection)
        public void ShowFocusIndicator(bool on = true)
        { if (focusIndicatorGO != null) focusIndicatorGO.SetActive(on); }
        public void HideFocusIndicator() => ShowFocusIndicator(false);
        public void SetSelected(bool selected) => ShowFocusIndicator(selected);

        private void AutoWireFromHierarchyUsingHelper()
        {
          
        }

        /// <summary>
        /// Set label and portrait in one call (no background tint in new layout).
        /// </summary>
        public void Set(string text, Sprite portraitSprite)
        {
            // Ensure mask visuals are enabled for normal blocks
            if (maskRoot != null) maskRoot.SetActive(true);
            if (backImage != null)
            {
                backImage.enabled = true;
                // keep existing sprite assigned by prefab unless explicitly changed
            }

            SetLabel("");
            SetPortraitTopHalf(portraitSprite);
            EnforceSquare();
            ConfigureMask();
        }

        /// <summary>
        /// Configure as a divider: hide portrait, set the mask image sprite for the divider art, and set label.
        /// </summary>
        public void SetDivider(Sprite dividerSprite, string text)
        {
            ClearPortrait();

            // Disable mask root so divider is not clipped
            if (maskRoot != null) maskRoot.SetActive(false);
            else DisableMask();

            // Prefer Back Image for divider; fallback to mask image
            if (backImage != null)
            {
                backImage.sprite = dividerSprite;
                backImage.enabled = dividerSprite != null;
                var c = backImage.color; c.a = 1f; backImage.color = c;
            }
            else if (maskImage != null)
            {
                maskImage.sprite = dividerSprite;
                maskImage.enabled = dividerSprite != null;
                var c = maskImage.color; c.a = 1f; maskImage.color = c;
            }

            SetLabel(text);
            EnforceSquare();
        }

        /// <summary>
        /// Assign the back sprite and show the back image.
        /// </summary>
        public void SetBackSprite(Sprite sprite, Color color)
        {
            backImage.sprite = sprite;
            backImage.enabled = sprite != null;
            backImage.color = color;

            maskImage.sprite = sprite;
            maskImage.enabled = sprite != null;
            maskImage.color = color;
        }

        /// <summary>
        /// Tint the background for enemy blocks (via mask image color).
        /// </summary>
        public void TintBackForEnemy()
        {
            // Tint the back image if present; else tint mask image
            if (backImage != null)
            {
                backImage.color = ColorHelper.Solid.GunMetal;
                backImage.enabled = true;
            }
            else if (maskImage != null)
            {
                maskImage.color = ColorHelper.Solid.GunMetal;
                maskImage.enabled = true;
            }
        }

        /// <summary>
        /// Tint the background for hero blocks (via mask image color).
        /// </summary>
        public void TintBackForHero()
        {
            // Tint the back image if present; else tint mask image
            if (backImage != null)
            {
                backImage.color = ColorHelper.Solid.White;
                backImage.enabled = true;
            }
            else if (maskImage != null)
            {
                maskImage.color = ColorHelper.Solid.White;
                maskImage.enabled = true;
            }
        }

        /// <summary>
        /// Allows callers to override the portrait's Y offset in pixels (default is -blockSize * 0.5).
        /// Pass 0 to center the portrait within the mask.
        /// </summary>
        public void SetPortraitYOffset(float y)
        {
            portraitYOffsetOverride = y;
            FitPortraitRect();
        }

        /// <summary>
        /// Apply an actor's ThumbnailSettings to crop/zoom the portrait in the mask.
        /// </summary>
        public void ApplyThumbnailSettings(ThumbnailSettings settings)
        {
            if (settings == null)
            {
                useThumbnailSettings = false;
                appliedThumbnail = null;
            }
            else
            {
                // Store a copy so later mutations on the source do not affect our layout unexpectedly.
                appliedThumbnail = new ThumbnailSettings(settings);
                useThumbnailSettings = true;
            }

            // Clear legacy Y-offset override so it doesn't conflict.
            portraitYOffsetOverride = null;
            FitPortraitRect();
        }

        /// <summary>
        /// Apply a canvas crop window (top-center), e.g., built from ActorData.CanvasThumbnailSettings.
        /// </summary>
        public void ApplyCanvasCrop(CanvasThumbnailSettings crop)
        {
            canvasCrop = crop != null ? new CanvasThumbnailSettings(crop) : CanvasThumbnailSettings.Default;
            FitPortraitRect();
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

        public void SetLabel(string text)
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

        private void DisableMask()
        {
            if (portraitMask != null) portraitMask.enabled = false;
            if (portrait != null) portrait.enabled = false;
        }

        private void FitPortraitRect()
        {
            if (portrait == null || Rect == null) return;

            float s = Mathf.Min(Rect.rect.width, Rect.rect.height);
            var pr = portrait.rectTransform;

            pr.anchorMin = new Vector2(0.5f, 0.5f);
            pr.anchorMax = new Vector2(0.5f, 0.5f);
            pr.pivot = new Vector2(0.5f, 0.5f);

            if (canvasCrop != null)
            {
                // Use canvas crop settings directly
                float w = Mathf.Max(1f, canvasCrop.Width);
                float h = Mathf.Max(1f, canvasCrop.Height);
                pr.sizeDelta = new Vector2(w, h);

                float px = canvasCrop.X;
                float py = canvasCrop.Y;
                pr.anchoredPosition = new Vector2(px, py);

                // Apply UI scale from settings (default 4,4)
                pr.localScale = new Vector3(canvasCrop.Scale.x, canvasCrop.Scale.y, 1f);
            }
            else if (useThumbnailSettings && appliedThumbnail != null)
            {
                float w = Mathf.Max(1f, s * appliedThumbnail.Scale.x);
                float h = Mathf.Max(1f, s * appliedThumbnail.Scale.y);
                pr.sizeDelta = new Vector2(w, h);

                float px = appliedThumbnail.Offset.x * s;
                float py = appliedThumbnail.Offset.y * s;
                pr.anchoredPosition = new Vector2(px, py);

                pr.localScale = Vector3.one;
            }
            else
            {
                // Default to CanvasThumbnailSettings defaults
                var def = CanvasThumbnailSettings.Default;
                pr.sizeDelta = new Vector2(def.Width, def.Height);
                float y = portraitYOffsetOverride.HasValue ? portraitYOffsetOverride.Value : def.Y;
                pr.anchoredPosition = new Vector2(def.X, y);
                pr.localScale = new Vector3(def.Scale.x, def.Scale.y, 1f);
            }

            pr.localRotation = Quaternion.identity;
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

        public void ClearPortrait()
        {
            if (portrait != null)
            {
                portrait.sprite = null;
                portrait.enabled = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Owner == null) return;
            // Focus this actor via SelectionManager, which also refreshes timeline selections
            g.SelectionManager?.Select(Owner);
        }
    }
}
