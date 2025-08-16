using Assets.Helper;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using c = Assets.Helpers.CanvasHelper;
using g = Assets.Helpers.GameHelper;
using Label = TMPro.TextMeshProUGUI;

// The Card class manages the UI card display that shows details about a focused actor.
// Responsibilities:
// 1) Initialize references and base layout values.
// 2) Assign the focused actor data and show the card.
// 3) Animate portrait slide in with fade in on backdrop, title, details, and portrait.
// 4) Animate portrait slide out with fade out on backdrop, title, details, and portrait.
// 5) On reselect while visible, quickly slide out the current portrait, keep backdrop visible,
//    swap data, then slide the new portrait in with Title and Details fading out then in.
// 6) Provide simple feedback animations like BouncePortrait.
// Notes:
// - Uses CanvasGroup for fading. If a CanvasGroup is missing on a target, one is added at runtime.
public class Card : MonoBehaviour
{
    RectTransform card;
    RectTransform backdrop;
    RectTransform portrait;
    RectTransform title;
    RectTransform details;

    CanvasGroup backdropCG;
    CanvasGroup titleCG;
    CanvasGroup detailsCG;
    CanvasGroup portraitCG;

    Vector3 offscreenPosition;
    Vector3 destination;
    AnimationCurve slideInCurve;
    float slideDuration;
    float portraitSize;

    // Awake caches references and ensures CanvasGroups exist for fade control.
    // Also precomputes positions before calling Clear so Clear uses correct offscreen values.
    private void Awake()
    {
        card = GameObject.Find(GameObjectHelper.Game.Card.Root).GetComponent<RectTransform>();
        backdrop = GameObject.Find(GameObjectHelper.Game.Card.Backdrop).GetComponent<RectTransform>();
        portrait = GameObject.Find(GameObjectHelper.Game.Card.Portrait).GetComponent<RectTransform>();
        title = GameObject.Find(GameObjectHelper.Game.Card.Title).GetComponent<RectTransform>();
        details = GameObject.Find(GameObjectHelper.Game.Card.Details).GetComponent<RectTransform>();

        backdropCG = backdrop.GetComponent<CanvasGroup>();
        titleCG = title.GetComponent<CanvasGroup>();
        detailsCG = details.GetComponent<CanvasGroup>();
        portraitCG = portrait.GetComponent<CanvasGroup>();

        portraitSize = c.CanvasRect.rect.width;
        slideInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        slideDuration = 0.5f;

        // Precompute offscreen and destination so Clear() can rely on them here in Awake.
        offscreenPosition = new Vector3(portraitSize, 0f);
        destination = new Vector3(-portraitSize / 4f, 0f);

        Clear();
    }

    // Start computes size again for safety; positions are already set in Awake for Clear().
    private void Start()
    {
        portrait.sizeDelta = new Vector2(portraitSize, portraitSize);

        // Keep in sync if canvas size changed between Awake and Start.
        offscreenPosition = new Vector3(portraitSize, 0f);
        destination = new Vector3(-portraitSize / 4f, 0f);
    }

    // Assign populates the card from the focused actor and starts the appropriate animation.
    public void Assign()
    {
        if (!g.Actors.HasFocusedActor)
            return;

        var actorData = ActorLibrary.Get(g.Actors.FocusedActor.characterName);

        // Precompute text and sprite for the new actor.
        var HP = $"{g.Actors.FocusedActor.Stats.HP,2}/{g.Actors.FocusedActor.Stats.MaxHP,-3}";
        var STR = $"{g.Actors.FocusedActor.Stats.Strength,4}";
        var VIT = $"{g.Actors.FocusedActor.Stats.Vitality,4}";
        var AGI = $"{g.Actors.FocusedActor.Stats.Agility,4}";
        var STA = $"{g.Actors.FocusedActor.Stats.Stamina,4}";
        var INT = $"{g.Actors.FocusedActor.Stats.Intelligence,4}";
        var WIS = $"{g.Actors.FocusedActor.Stats.Wisdom,4}";
        var LCK = $"{g.Actors.FocusedActor.Stats.Luck,4}";

        var stats =
            $"HP       STR  VIT  AGI  STA  INT  WIS  LCK{Environment.NewLine}" +
            $"{HP}   {STR}{VIT}{AGI}{STA}{INT}{WIS}{LCK}{Environment.NewLine}";

        string newTitle = g.Actors.FocusedActor.characterName;
        string newDetails = stats + actorData.Details.Card;
        Sprite newPortrait = actorData.Portrait;

        // Ensure base visuals are enabled.
        backdrop.gameObject.SetActive(true);
        portrait.gameObject.SetActive(true);

        // Decide whether to run a quick swap or a normal show.
        bool alreadyVisible =
            backdrop.gameObject.activeInHierarchy &&
            portrait.gameObject.activeInHierarchy &&
            backdropCG.alpha > 0.9f &&
            portraitCG.alpha > 0.9f &&
            ApproximatelyVector2(portrait.anchoredPosition, destination);

        StopAllCoroutines();

        if (alreadyVisible)
        {
            StartCoroutine(QuickSwapRoutine(newPortrait, newTitle, newDetails));
        }
        else
        {
            // First show path.
            portrait.GetComponent<Image>().sprite = newPortrait;
            title.GetComponent<Label>().text = newTitle;
            details.GetComponent<Label>().text = newDetails;
            StartCoroutine(SlideInRoutine());
        }
    }

    // SlideIn starts the in animation that moves the portrait from offscreen to destination
    // while fading in backdrop, title, details, and portrait.
    private void SlideIn()
    {
        StopAllCoroutines();
        StartCoroutine(SlideInRoutine());
    }

    // SlideInRoutine animates the portrait position and fades UI elements in over slideDuration.
    private IEnumerator SlideInRoutine()
    {
        float elapsed = 0f;

        portrait.anchoredPosition = offscreenPosition;
        SetAlpha(backdropCG, 0f);
        SetAlpha(titleCG, 0f);
        SetAlpha(detailsCG, 0f);
        SetAlpha(portraitCG, 0f);

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float eased = slideInCurve.Evaluate(t);

            portrait.anchoredPosition = Vector3.Lerp(offscreenPosition, destination, eased);

            float a = eased;
            SetAlpha(backdropCG, a);
            SetAlpha(titleCG, a);
            SetAlpha(detailsCG, a);
            SetAlpha(portraitCG, a);

            yield return Wait.OneTick();
        }

        portrait.anchoredPosition = destination;
        SetAlpha(backdropCG, 1f);
        SetAlpha(titleCG, 1f);
        SetAlpha(detailsCG, 1f);
        SetAlpha(portraitCG, 1f);
    }

    // Public wrapper to hide the card with animation.
    public void SlideOut()
    {
        StopAllCoroutines();
        StartCoroutine(SlideOutRoutine());
    }

    // SlideOutRoutine animates portrait out to offscreen and fades elements to transparent.
    private IEnumerator SlideOutRoutine()
    {
        float elapsed = 0f;

        portrait.anchoredPosition = destination;
        SetAlpha(backdropCG, 1f);
        SetAlpha(titleCG, 1f);
        SetAlpha(detailsCG, 1f);
        SetAlpha(portraitCG, 1f);

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float eased = slideInCurve.Evaluate(t);

            portrait.anchoredPosition = Vector3.Lerp(destination, offscreenPosition, eased);

            float a = 1f - eased;
            SetAlpha(backdropCG, a);
            SetAlpha(titleCG, a);
            SetAlpha(detailsCG, a);
            SetAlpha(portraitCG, a);

            yield return Wait.OneTick();
        }

        portrait.anchoredPosition = offscreenPosition;
        SetAlpha(backdropCG, 0f);
        SetAlpha(titleCG, 0f);
        SetAlpha(detailsCG, 0f);
        SetAlpha(portraitCG, 0f);

        backdrop.gameObject.SetActive(false);
        portrait.gameObject.SetActive(false);
    }

    // Clear cancels any animations and restores a strict baseline with no mid-transition leftovers.
    // Baseline rules:
    // - All coroutines stopped.
    // - Backdrop and Portrait disabled.
    // - Title and Details text cleared.
    // - Portrait anchored at offscreenPosition.
    // - All CanvasGroups alpha = 0, raycasts disabled, not interactable.
    public void Clear()
    {
        StopAllCoroutines();

        // Disable visuals that should not be visible when cleared.
        backdrop.gameObject.SetActive(false);
        portrait.gameObject.SetActive(false);

        // Reset text fields.
        title.GetComponent<Label>().text = "";
        details.GetComponent<Label>().text = "";

        // Reset transform state.
        portrait.anchoredPosition = offscreenPosition;

        // Hard-reset all alphas and interaction on groups.
        Reset(backdropCG);
        Reset(titleCG);
        Reset(detailsCG);
        Reset(portraitCG);
    }

    // PortraitWorldPosition exposes the portrait position in world space for external use.
    public Vector3 PortraitWorldPosition()
    {
        return ScreenHelper.Convert.CanvasToWorldPosition(portrait.transform);
    }

    // BouncePortrait performs a quick up and down motion for feedback effects.
    public void BouncePortrait(float percentOfScreenHeight = 0.03f, float bounceDuration = 0.3333f)
    {
        float bounceDistance = Screen.height * percentOfScreenHeight;
        StartCoroutine(BouncePortraitRoutune(bounceDistance, bounceDuration));
    }

    // BouncePortraitRoutune animates upward then returns to the original position.
    private IEnumerator BouncePortraitRoutune(float bounceDistance, float bounceDuration)
    {
        Vector2 originalPos = portrait.anchoredPosition;
        Vector2 upPos = originalPos + Vector2.up * bounceDistance;
        float halfDuration = bounceDuration / 2f;
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration;
            portrait.anchoredPosition = Vector2.Lerp(originalPos, upPos, Mathf.SmoothStep(0f, 1f, t));
            elapsed += Time.deltaTime;
            yield return Wait.None();
        }
        portrait.anchoredPosition = upPos;

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration;
            portrait.anchoredPosition = Vector2.Lerp(upPos, originalPos, Mathf.SmoothStep(0f, 1f, t));
            elapsed += Time.deltaTime;
            yield return Wait.None();
        }
        portrait.anchoredPosition = originalPos;
    }

    // QuickSwapRoutine performs a fast slide out of the current portrait, keeps backdrop visible,
    // updates the data, then slides in the new portrait while Title and Details fade out then in.
    private IEnumerator QuickSwapRoutine(Sprite newSprite, string newTitle, string newDetails)
    {
        float quickOut = Mathf.Max(0.15f, slideDuration * 0.35f);
        float elapsedOut = 0f;

        // Ensure starting state.
        portrait.anchoredPosition = destination;
        SetAlpha(backdropCG, 1f);

        while (elapsedOut < quickOut)
        {
            elapsedOut += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedOut / quickOut);
            float eased = slideInCurve.Evaluate(t);

            portrait.anchoredPosition = Vector3.Lerp(destination, offscreenPosition, eased);

            float aOut = 1f - eased;
            SetAlpha(titleCG, aOut);
            SetAlpha(detailsCG, aOut);

            SetAlpha(backdropCG, 1f);
            SetAlpha(portraitCG, 1f);

            yield return Wait.OneTick();
        }

        // Fully hidden outgoing.
        portrait.anchoredPosition = offscreenPosition;
        SetAlpha(titleCG, 0f);
        SetAlpha(detailsCG, 0f);

        // Update content while hidden.
        portrait.GetComponent<Image>().sprite = newSprite;
        title.GetComponent<Label>().text = newTitle;
        details.GetComponent<Label>().text = newDetails;

        // Prepare for slide in.
        SetAlpha(backdropCG, 1f);
        SetAlpha(portraitCG, 1f);
        portrait.anchoredPosition = offscreenPosition;

        float elapsedIn = 0f;
        while (elapsedIn < slideDuration)
        {
            elapsedIn += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedIn / slideDuration);
            float eased = slideInCurve.Evaluate(t);

            portrait.anchoredPosition = Vector3.Lerp(offscreenPosition, destination, eased);

            float aIn = eased;
            SetAlpha(titleCG, aIn);
            SetAlpha(detailsCG, aIn);

            SetAlpha(backdropCG, 1f);
            SetAlpha(portraitCG, 1f);

            yield return Wait.OneTick();
        }

        portrait.anchoredPosition = destination;
        SetAlpha(titleCG, 1f);
        SetAlpha(detailsCG, 1f);
        SetAlpha(backdropCG, 1f);
        SetAlpha(portraitCG, 1f);
    }

    // EnsureCanvasGroup attaches or returns a CanvasGroup on a RectTransform.
    private static CanvasGroup EnsureCanvasGroup(RectTransform target)
    {
        var cg = target.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = target.gameObject.AddComponent<CanvasGroup>();
        return cg;
    }

    // SetAlpha updates a CanvasGroup alpha safely.
    private static void SetAlpha(CanvasGroup cg, float a)
    {
        if (cg != null) cg.alpha = a;
    }

    // Forces a CanvasGroup to a non-interactive fully transparent baseline.
    private static void Reset(CanvasGroup cg)
    {
        if (cg == null) return;
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    // ApproximatelyVector2 compares Vector2 positions with a small tolerance.
    private static bool ApproximatelyVector2(Vector2 a, Vector2 b, float tol = 0.5f)
    {
        return Mathf.Abs(a.x - b.x) <= tol && Mathf.Abs(a.y - b.y) <= tol;
    }
}
