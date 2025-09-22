using Assets.Helper;
using Assets.Helpers;
using Assets.Scripts.Libraries;
using Assets.Scripts.Utilities;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using c = Assets.Helpers.CanvasHelper;
using g = Assets.Helpers.GameHelper;
using Label = TMPro.TextMeshProUGUI;

public class Card : MonoBehaviour
{
    // ----- Cached RectTransforms -----
    private RectTransform card;
    private RectTransform backdrop;
    private RectTransform portrait;
    private RectTransform title;
    private RectTransform details;

    // ----- CanvasGroups for fading -----
    private CanvasGroup backdropCG;
    private CanvasGroup titleCG;
    private CanvasGroup detailsCG;
    private CanvasGroup portraitCG;

    // ----- Animation state -----
    private Vector3 offscreenPosition;
    private Vector3 destination;
    private AnimationCurve slideInCurve;
    private float slideDuration;
    private float portraitSize;

    private const float PortraitWidthRatio = 1f;

    private void Awake()
    {
        card = GameObject.Find(GameObjectHelper.Game.Card.Root).GetComponent<RectTransform>();
        backdrop = GameObject.Find(GameObjectHelper.Game.Card.Backdrop).GetComponent<RectTransform>();
        portrait = GameObject.Find(GameObjectHelper.Game.Card.Portrait).GetComponent<RectTransform>();
        title = GameObject.Find(GameObjectHelper.Game.Card.Title).GetComponent<RectTransform>();
        details = GameObject.Find(GameObjectHelper.Game.Card.Details).GetComponent<RectTransform>();

        backdropCG = EnsureCanvasGroup(backdrop);
        titleCG = EnsureCanvasGroup(title);
        detailsCG = EnsureCanvasGroup(details);
        portraitCG = EnsureCanvasGroup(portrait);

        slideInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        slideDuration = 0.5f;

        RecomputeLayout();
        Clear();

        GameReady.Begin(this);
    }

    private void Start()
    {
        RecomputeLayout();
    }

    private void OnRectTransformDimensionsChange()
    {
        RecomputeLayout();
    }

    public void Assign()
    {
        if (!g.Actors.HasFocusedActor) return;

        var actorName = g.Actors.FocusedActor.characterName;
        var actorData = ActorLibrary.Get(actorName);

        // Ensure visuals enabled
        backdrop.gameObject.SetActive(true);
        portrait.gameObject.SetActive(true);

        // Detect current visibility states
        bool backdropVisible = backdrop.gameObject.activeInHierarchy && backdropCG.alpha > 0.9f;
        bool portraitVisibleAndPlaced =
            portrait.gameObject.activeInHierarchy &&
            portraitCG.alpha > 0.9f &&
            ApproximatelyVector2(portrait.anchoredPosition, destination);

        StopAllCoroutines();

        if (portraitVisibleAndPlaced)
        {
            // Delay sprite/title/details change until AFTER portrait is fully off-screen
            StartCoroutine(QuickSwapRoutine(actorData.Portrait, actorName, actorData.Details.Card, fadeText: !backdropVisible));
        }
        else
        {
            // First-time show or portrait off-screen: set content up front, then slide in
            portrait.GetComponent<Image>().sprite = actorData.Portrait;
            title.GetComponent<Label>().text = actorName;
            details.GetComponent<Label>().text = actorData.Details.Card;

            StartCoroutine(SlideInRoutine(fadeText: !backdropVisible));
        }
    }

    private void SlideIn()
    {
        StopAllCoroutines();
        StartCoroutine(SlideInRoutine(fadeText: true));
    }

    private IEnumerator SlideInRoutine(bool fadeText)
    {
        float elapsed = 0f;

        // Start state
        portrait.anchoredPosition = offscreenPosition;
        SetAlpha(portraitCG, 0f);

        // Backdrop always fades in from current to 1
        SetAlpha(backdropCG, 0f);

        // Conditionally fade title/details
        if (fadeText)
        {
            SetAlpha(titleCG, 0f);
            SetAlpha(detailsCG, 0f);
        }
        else
        {
            SetAlpha(titleCG, 1f);
            SetAlpha(detailsCG, 1f);
        }

        // Animate
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float eased = slideInCurve.Evaluate(t);

            portrait.anchoredPosition = Vector3.Lerp(offscreenPosition, destination, eased);

            // Always bring backdrop/portrait in
            SetAlpha(backdropCG, eased);
            SetAlpha(portraitCG, eased);

            if (fadeText)
            {
                float a = eased;
                SetAlpha(titleCG, a);
                SetAlpha(detailsCG, a);
            }

            yield return Wait.OneTick();
        }

        // Final state
        portrait.anchoredPosition = destination;
        SetAlpha(backdropCG, 1f);
        SetAlpha(portraitCG, 1f);
        SetAlpha(titleCG, 1f);
        SetAlpha(detailsCG, 1f);
    }

    public void SlideOut()
    {
        StopAllCoroutines();
        StartCoroutine(SlideOutRoutine());
    }

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
            SetAlpha(titleCG, a);
            SetAlpha(detailsCG, a);

            SetAlpha(backdropCG, a);
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

    public void Clear()
    {
        StopAllCoroutines();

        backdrop.gameObject.SetActive(false);
        portrait.gameObject.SetActive(false);

        title.GetComponent<Label>().text = "";
        details.GetComponent<Label>().text = "";

        portrait.anchoredPosition = offscreenPosition;

        Reset(backdropCG);
        Reset(titleCG);
        Reset(detailsCG);
        Reset(portraitCG);
    }

    public Vector3 PortraitWorldPosition()
    {
        return UnitConversionHelper.Canvas.ToWorld(portrait.transform);
    }

    public void BouncePortrait(float percentOfScreenHeight = 0.03f, float bounceDuration = 0.3333f)
    {
        float bounceDistance = Screen.height * percentOfScreenHeight;
        StartCoroutine(BouncePortraitRoutine(bounceDistance, bounceDuration));
    }

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

    private IEnumerator QuickSwapRoutine(Sprite newSprite, string newTitle, string newDetails, bool fadeText)
    {
        float quickOut = Mathf.Max(0.15f, slideDuration * 0.35f);
        float elapsedOut = 0f;

        // Start state
        portrait.anchoredPosition = destination;
        SetAlpha(backdropCG, 1f);
        SetAlpha(portraitCG, 1f);

        // Slide out quickly; optionally fade title/details
        while (elapsedOut < quickOut)
        {
            elapsedOut += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedOut / quickOut);
            float eased = slideInCurve.Evaluate(t);

            portrait.anchoredPosition = Vector3.Lerp(destination, offscreenPosition, eased);

            if (fadeText)
            {
                float aOut = 1f - eased;
                SetAlpha(titleCG, aOut);
                SetAlpha(detailsCG, aOut);
            }

            yield return Wait.OneTick();
        }

        // Hidden: swap content
        portrait.anchoredPosition = offscreenPosition;
        if (fadeText)
        {
            SetAlpha(titleCG, 0f);
            SetAlpha(detailsCG, 0f);
        }

        portrait.GetComponent<Image>().sprite = newSprite;
        title.GetComponent<Label>().text = newTitle;
        details.GetComponent<Label>().text = newDetails;

        // Slide back in; optionally fade title/details in
        float elapsedIn = 0f;
        while (elapsedIn < slideDuration)
        {
            elapsedIn += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedIn / slideDuration);
            float eased = slideInCurve.Evaluate(t);

            portrait.anchoredPosition = Vector3.Lerp(offscreenPosition, destination, eased);

            if (fadeText)
            {
                float aIn = eased;
                SetAlpha(titleCG, aIn);
                SetAlpha(detailsCG, aIn);
            }

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

    private void RecomputeLayout()
    {
        if (card == null || portrait == null) return;

        float basisWidth = card.rect.width > 0f ? card.rect.width : c.CanvasRect.rect.width;
        portraitSize = basisWidth * PortraitWidthRatio;

        portrait.sizeDelta = new Vector2(portraitSize, portraitSize);

        offscreenPosition = new Vector3(portraitSize, 0f, 0f);
        destination = new Vector3(-portraitSize * 0.25f, 0f, 0f);
    }

    private static CanvasGroup EnsureCanvasGroup(RectTransform target)
    {
        var cg = target.GetComponent<CanvasGroup>();
        if (cg == null) cg = target.gameObject.AddComponent<CanvasGroup>();
        return cg;
    }

    private static void SetAlpha(CanvasGroup cg, float a)
    {
        if (cg != null) cg.alpha = a;
    }

    private static void Reset(CanvasGroup cg)
    {
        if (cg == null) return;
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    private static bool ApproximatelyVector2(Vector2 a, Vector2 b, float tol = 0.5f)
    {
        return Mathf.Abs(a.x - b.x) <= tol && Mathf.Abs(a.y - b.y) <= tol;
    }

    // --------------------------------------------------------------------------------------------
    // UI: Arrow buttons to cycle focused hero
    // --------------------------------------------------------------------------------------------

    private void CycleHero(int direction)
    {
        // Block during ability targeting flows to avoid conflicting UI states
        if (g.InputManager != null)
        {
            var mode = g.InputManager.InputMode;
            if (mode == InputMode.AnyActorTarget || mode == InputMode.LinearTarget)
                return;
            // Ignore while dragging a selected hero
            if (g.InputManager.isDragging)
                return;
        }

        var heroes = g.Actors.Heroes.Where(h => h != null && h.IsPlaying).ToList();
        if (heroes.Count == 0) return;

        // Choose a baseline current hero
        var current = g.Actors.FocusedActor != null && g.Actors.FocusedActor.IsHero
            ? g.Actors.FocusedActor
            : (g.TurnManager != null && g.TurnManager.ActiveActor != null && g.TurnManager.ActiveActor.IsHero
                ? g.TurnManager.ActiveActor
                : heroes.First());

        int idx = heroes.IndexOf(current);
        if (idx < 0) idx = 0;

        int next = (idx + direction) % heroes.Count;
        if (next < 0) next += heroes.Count;

        var target = heroes[next];
        if (target == null) return;

        g.SelectedHeroManager?.Focus(target);
        g.AudioManager?.Play("Click");
    }

    // Bind these in the Inspector to Card/ArrowLeft and Card/ArrowRight buttons
    public void OnPreviousHeroArrowClick()
    {
        CycleHero(-1);
    }

    public void OnNextHeroArrowClick()
    {
        CycleHero(1);
    }
}
