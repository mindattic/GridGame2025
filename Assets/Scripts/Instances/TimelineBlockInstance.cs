using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.Scripts.Canvas.Timeline
{
    [DisallowMultipleComponent]
    public sealed class TimelineBlockInstance : MonoBehaviour
    {
        [Header("Parts")]
        [SerializeField] private Image back;
        [SerializeField] private Image portrait;
        [SerializeField] private Mask portraitMask;
        [SerializeField] private TMP_Text label;

        public RectTransform Rect { get; private set; }

        private void Awake() => Rect = GetComponent<RectTransform>();

        public void SetSize(float width, float height)
        {
            Rect.sizeDelta = new Vector2(width, height);
        }

        public void SetStyle(bool isHero, Color tint)
        {
            back.color = isHero ? Color.white : tint;
            if (label) label.color = isHero ? Color.black : Color.white;
        }

        public void SetLabel(string text)
        {
            if (label) label.text = text;
        }

        public void SetPortrait(Sprite sprite, bool enabled)
        {
            if (!portrait || !portraitMask) return;

            portraitMask.enabled = enabled;
            portrait.enabled = enabled;
            if (enabled) portrait.sprite = sprite;
        }
    }
}
