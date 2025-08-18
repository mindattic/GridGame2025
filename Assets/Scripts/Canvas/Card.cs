using Assets.Helper;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using c = Assets.Helpers.CanvasHelper;
using g = Assets.Helpers.GameHelper;
using Label = TMPro.TextMeshProUGUI;

/// <summary>
/// Card UI controller.
/// Shows info for the focused actor and animates a portrait sliding in/out,
/// with fades on backdrop, title, details, and portrait.
/// Portrait size is always half of the Card container width.
/// </summary>
public class Card : MonoBehaviour
{
    // ----- Cached RectTransforms -----
    private RectTransform card;        // Root card container
    private RectTransform backdrop;    // Dim background behind the portrait and text
    private RectTransform portrait;    // Actor portrait image rect
    private RectTransform title;       // Title text rect
    private RectTransform details;     // Details text rect

    // ----- CanvasGroups for fading -----
    private CanvasGroup backdropCG;
    private CanvasGroup titleCG;
    private CanvasGroup detailsCG;
    private CanvasGroup portraitCG;

    // ----- Animation state -----
    private Vector3 offscreenPosition; // Where the portrait starts when hidden (to the right)
    private Vector3 destination;       // Where the portrait rests when shown
    private AnimationCurve slideInCurve;
    private float slideDuration;
    private float portraitSize;        // Square size for the portrait (half of card width)

    // Ratio used to size the portrait relative to the card width.
    private const float PortraitWidthRatio = 1f;

    // Awake sets up references, ensures groups, computes sizes, then clears the UI.
    private void Awake()
    {
        // Find core objects
        card = GameObject.Find(GameObjectHelper.Game.Card.Root).GetComponent<RectTransform>();
        backdrop = GameObject.Find(GameObjectHelper.Game.Card.Backdrop).GetComponent<RectTransform>();
        portrait = GameObject.Find(GameObjectHelper.Game.Card.Portrait).GetComponent<RectTransform>();
        title = GameObject.Find(GameObjectHelper.Game.Card.Title).GetComponent<RectTransform>();
        details = GameObject.Find(GameObjectHelper.Game.Card.Details).GetComponent<RectTransform>();

        // Ensure CanvasGroups exist on each target
        backdropCG = EnsureCanvasGroup(backdrop);
        titleCG = EnsureCanvasGroup(title);
        detailsCG = EnsureCanvasGroup(details);
        portraitCG = EnsureCanvasGroup(portrait);

        // Basic animation config
        slideInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        slideDuration = 0.5f;

        // Compute initial sizes and positions
        RecomputeLayout();

        // Reset to a known baseline
        Clear();
    }

    // Start reapplies size/positions in case the layout resolved after Awake.
    private void Start()
    {
        RecomputeLayout();
    }

    // React to layout or resolution changes at runtime and in editor.
    private void OnRectTransformDimensionsChange()
    {
        // If any parent layout changed the card width, update portrait sizing/positions.
        RecomputeLayout();
    }

    // Assign populates the card with the focused actor and plays the right animation.
    public void Assign()
    {
        if (!g.Actors.HasFocusedActor) return;

        var actorName = g.Actors.FocusedActor.characterName;
        var actorData = ActorLibrary.Get(actorName);

        // Update content fields
        portrait.GetComponent<Image>().sprite = actorData.Portrait;
        title.GetComponent<Label>().text = actorName;
        details.GetComponent<Label>().text = actorData.Details.Card;

        // Ensure base visuals are enabled
        backdrop.gameObject.SetActive(true);
        portrait.gameObject.SetActive(true);

        // Decide which animation path to use
        bool alreadyVisible =
            backdrop.gameObject.activeInHierarchy &&
            portrait.gameObject.activeInHierarchy &&
            backdropCG.alpha > 0.9f &&
            portraitCG.alpha > 0.9f &&
            ApproximatelyVector2(portrait.anchoredPosition, destination);

        StopAllCoroutines();

        if (alreadyVisible)
        {
            // Quick swap when card is already shown
            StartCoroutine(QuickSwapRoutine(actorData.Portrait, actorName, actorData.Details.Card));
        }
        else
        {
            // First-time show
            StartCoroutine(SlideInRoutine());
        }
    }

    // Public wrapper to start the slide-in animation.
    private void SlideIn()
    {
        StopAllCoroutines();
        StartCoroutine(SlideInRoutine());
    }

    // Slide-in animation: move portrait from right to destination and fade elements in.
    private IEnumerator SlideInRoutine()
    {
        float elapsed = 0f;

        // Start state
        portrait.anchoredPosition = offscreenPosition;
        SetAlpha(backdropCG, 0f);
        SetAlpha(titleCG, 0f);
        SetAlpha(detailsCG, 0f);
        SetAlpha(portraitCG, 0f);

        // Animate
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

        // Final state
        portrait.anchoredPosition = destination;
        SetAlpha(backdropCG, 1f);
        SetAlpha(titleCG, 1f);
        SetAlpha(detailsCG, 1f);
        SetAlpha(portraitCG, 1f);
    }

    // Public API to hide the card with animation.
    public void SlideOut()
    {
        StopAllCoroutines();
        StartCoroutine(SlideOutRoutine());
    }

    // Slide-out animation: move portrait offscreen to the right and fade elements out.
    private IEnumerator SlideOutRoutine()
    {
        float elapsed = 0f;

        // Start state
        portrait.anchoredPosition = destination;
        SetAlpha(backdropCG, 1f);
        SetAlpha(titleCG, 1f);
        SetAlpha(detailsCG, 1f);
        SetAlpha(portraitCG, 1f);

        // Animate
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

        // Final state
        portrait.anchoredPosition = offscreenPosition;
        SetAlpha(backdropCG, 0f);
        SetAlpha(titleCG, 0f);
        SetAlpha(detailsCG, 0f);
        SetAlpha(portraitCG, 0f);

        backdrop.gameObject.SetActive(false);
        portrait.gameObject.SetActive(false);
    }

    // Clear returns the UI to a strict baseline with no visible elements.
    public void Clear()
    {
        StopAllCoroutines();

        // Disable visuals
        backdrop.gameObject.SetActive(false);
        portrait.gameObject.SetActive(false);

        // Clear text
        title.GetComponent<Label>().text = "";
        details.GetComponent<Label>().text = "";

        // Reset transform state
        portrait.anchoredPosition = offscreenPosition;

        // Reset fades and interaction
        Reset(backdropCG);
        Reset(titleCG);
        Reset(detailsCG);
        Reset(portraitCG);
    }

    // Converts the portrait RectTransform position to world space.
    public Vector3 PortraitWorldPosition()
    {
        return ScreenHelper.Convert.CanvasToWorldPosition(portrait.transform);
    }

    // Quick bounce for feedback on the portrait.
    public void BouncePortrait(float percentOfScreenHeight = 0.03f, float bounceDuration = 0.3333f)
    {
        float bounceDistance = Screen.height * percentOfScreenHeight;
        StartCoroutine(BouncePortraitRoutine(bounceDistance, bounceDuration));
    }

    // Bounce animation up and back down.
    private IEnumerator BouncePortraitRoutine(float bounceDistance, float bounceDuration)
    {
        Vector2 originalPos = portrait.anchoredPosition;
        Vector2 upPos = originalPos + Vector2.up * bounceDistance;
        float half = bounceDuration * 0.5f;
        float elapsed = 0f;

        while (elapsed < half)
        {
            float t = elapsed / half;
            portrait.anchoredPosition = Vector2.Lerp(originalPos, upPos, Mathf.SmoothStep(0f, 1f, t));
            elapsed += Time.deltaTime;
            yield return Wait.None();
        }
        portrait.anchoredPosition = upPos;

        elapsed = 0f;
        while (elapsed < half)
        {
            float t = elapsed / half;
            portrait.anchoredPosition = Vector2.Lerp(upPos, originalPos, Mathf.SmoothStep(0f, 1f, t));
            elapsed += Time.deltaTime;
            yield return Wait.None();
        }
        portrait.anchoredPosition = originalPos;
    }

    // Fast swap when a new actor is selected while the card is already visible.
    private IEnumerator QuickSwapRoutine(Sprite newSprite, string newTitle, string newDetails)
    {
        float quickOut = Mathf.Max(0.15f, slideDuration * 0.35f);
        float elapsedOut = 0f;

        // Start state
        portrait.anchoredPosition = destination;
        SetAlpha(backdropCG, 1f);

        // Slide out quickly while fading title/details
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

        // Hidden: swap content
        portrait.anchoredPosition = offscreenPosition;
        SetAlpha(titleCG, 0f);
        SetAlpha(detailsCG, 0f);

        portrait.GetComponent<Image>().sprite = newSprite;
        title.GetComponent<Label>().text = newTitle;
        details.GetComponent<Label>().text = newDetails;

        // Slide back in with title/details fading in
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

        // Final state
        portrait.anchoredPosition = destination;
        SetAlpha(titleCG, 1f);
        SetAlpha(detailsCG, 1f);
        SetAlpha(backdropCG, 1f);
        SetAlpha(portraitCG, 1f);
    }

    // ----- Helpers -----

    /// <summary>
    /// Compute portrait size and key positions from the current card width.
    /// Portrait is a square sized to half of the card width.
    /// Also applies the size to the portrait rect so layout is in sync.
    /// </summary>
    private void RecomputeLayout()
    {
        if (card == null || portrait == null) return;

        // If card width is not resolved yet, fall back to canvas width
        float basisWidth = card.rect.width > 0f ? card.rect.width : c.CanvasRect.rect.width;

        // Portrait is half of the available width
        portraitSize = basisWidth * PortraitWidthRatio;

        // Apply the actual size to the portrait rect (square)
        portrait.sizeDelta = new Vector2(portraitSize, portraitSize);

        // Positions that drive the slide animations
        offscreenPosition = new Vector3(portraitSize, 0f, 0f);
        destination = new Vector3(-portraitSize * 0.25f, 0f, 0f); // keep your original offset logic
    }

    // Ensure a CanvasGroup exists on a RectTransform.
    private static CanvasGroup EnsureCanvasGroup(RectTransform target)
    {
        var cg = target.GetComponent<CanvasGroup>();
        if (cg == null) cg = target.gameObject.AddComponent<CanvasGroup>();
        return cg;
    }

    // Set CanvasGroup alpha safely.
    private static void SetAlpha(CanvasGroup cg, float a)
    {
        if (cg != null) cg.alpha = a;
    }

    // Reset a CanvasGroup to transparent and non-interactive.
    private static void Reset(CanvasGroup cg)
    {
        if (cg == null) return;
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    // Approximate Vector2 equality with tolerance.
    private static bool ApproximatelyVector2(Vector2 a, Vector2 b, float tol = 0.5f)
    {
        return Mathf.Abs(a.x - b.x) <= tol && Mathf.Abs(a.y - b.y) <= tol;
    }
}
