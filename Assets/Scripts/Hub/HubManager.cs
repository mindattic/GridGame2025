using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using scene = Assets.Helpers.SceneHelper;
using Assets.Helper;

/// <summary>
/// HubManager coordinates navigation and section activation on the Hub.
/// It owns references to all section panels and their controllers and exposes entry points
/// for transitioning to other scenes (overworld, battle). Only one section panel is active
/// at a time. TODO hooks integrate with PartyManager, save/load, currencies, and scene flow.
/// </summary>
public class HubManager : MonoBehaviour
{
    // Navigation buttons (resolved at runtime; no inspector assignment required)
    private Button partyButton;
    private Button shopButton;
    private Button medicalButton;
    private Button residenceButton;
    private Button blacksmithButton;
    private Button overworldButton;
    private Button battleButton;

    // Section root panels (resolved at runtime)
    private RectTransform partyPanel;
    private RectTransform shopPanel;
    private RectTransform medicalPanel;
    private RectTransform residencePanel;
    private RectTransform blacksmithPanel;

    // Controllers
    private PartySectionController partyController;
    private ShopSectionController shopController;
    private MedicalSectionController medicalController;
    private ResidenceSectionController residenceController;
    private BlacksmithSectionController blacksmithController;

    // Track active section.
    private RectTransform activePanel;

    private void Awake()
    {
        ResolveSceneObjects();
        InitializeSections();

        // Ensure a clean start state: disable all panels then show Party.
        GoToPartySection();
    }

    private void OnDestroy()
    {
        // Unwire to prevent leaking listeners if Hub scene is unloaded.
        if (partyButton != null) partyButton.onClick.RemoveListener(GoToPartySection);
        if (shopButton != null) shopButton.onClick.RemoveListener(GoToShopSection);
        if (medicalButton != null) medicalButton.onClick.RemoveListener(GoToMedicalSection);
        if (residenceButton != null) residenceButton.onClick.RemoveListener(GoToResidenceSection);
        if (blacksmithButton != null) blacksmithButton.onClick.RemoveListener(GoToBlacksmithSection);
        if (overworldButton != null) overworldButton.onClick.RemoveListener(GoToOverworld);
        if (battleButton != null) battleButton.onClick.RemoveListener(GoToBattle);
    }

    // Fade in overlay once scene is fully started so Hub appears smoothly.
    private void Start()
    {
        scene.FadeIn();
    }

    private void ResolveSceneObjects()
    {
        // Buttons
        partyButton = GameObject.Find(GameObjectHelper.Hub.PartyButton)?.GetComponent<Button>();
        shopButton = GameObject.Find(GameObjectHelper.Hub.ShopButton)?.GetComponent<Button>();
        medicalButton = GameObject.Find(GameObjectHelper.Hub.MedicalButton)?.GetComponent<Button>();
        residenceButton = GameObject.Find(GameObjectHelper.Hub.ResidenceButton)?.GetComponent<Button>();
        blacksmithButton = GameObject.Find(GameObjectHelper.Hub.BlacksmithButton)?.GetComponent<Button>();
        overworldButton = GameObject.Find(GameObjectHelper.Hub.OverworldButton)?.GetComponent<Button>();
        battleButton = GameObject.Find(GameObjectHelper.Hub.BattleButton)?.GetComponent<Button>();

        // Panels
        partyPanel = GameObject.Find(GameObjectHelper.Hub.PartyPanel)?.GetComponent<RectTransform>();
        shopPanel = GameObject.Find(GameObjectHelper.Hub.ShopPanel)?.GetComponent<RectTransform>();
        medicalPanel = GameObject.Find(GameObjectHelper.Hub.MedicalPanel)?.GetComponent<RectTransform>();
        residencePanel = GameObject.Find(GameObjectHelper.Hub.ResidencePanel)?.GetComponent<RectTransform>();
        blacksmithPanel = GameObject.Find(GameObjectHelper.Hub.BlacksmithPanel)?.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Collect all section panels (non-null) for iteration.
    /// </summary>
    private IEnumerable<RectTransform> AllPanels()
    {
        if (partyPanel != null) yield return partyPanel;
        if (shopPanel != null) yield return shopPanel;
        if (medicalPanel != null) yield return medicalPanel;
        if (residencePanel != null) yield return residencePanel;
        if (blacksmithPanel != null) yield return blacksmithPanel;
    }

    /// <summary>
    /// Ensures each section controller is present and initialized.
    /// Adds controllers if missing so runtime code can rely on them.
    /// </summary>
    private void InitializeSections()
    {
        if (partyPanel != null) partyController = EnsureController<PartySectionController>(partyPanel);
        if (shopPanel != null) shopController = EnsureController<ShopSectionController>(shopPanel);
        if (medicalPanel != null) medicalController = EnsureController<MedicalSectionController>(medicalPanel);
        if (residencePanel != null) residenceController = EnsureController<ResidenceSectionController>(residencePanel);
        if (blacksmithPanel != null) blacksmithController = EnsureController<BlacksmithSectionController>(blacksmithPanel);

        partyController?.Initialize(this);
        shopController?.Initialize(this);
        medicalController?.Initialize(this);
        residenceController?.Initialize(this);
        blacksmithController?.Initialize(this);
    }

    /// <summary>
    /// Generic helper that guarantees a controller component exists on a panel.
    /// </summary>
    private T EnsureController<T>(RectTransform panel) where T : Component
    {
        var existing = panel.GetComponent<T>();
        if (existing != null) return existing;
        return panel.gameObject.AddComponent<T>();
    }

    /// <summary>
    /// Core section activation logic. Deactivates all other panels and activates the requested one.
    /// </summary>
    private void Activate(RectTransform panel)
    {
        if (panel == null) return;
        foreach (var p in AllPanels())
        {
            if (p == null) continue;
            bool enable = p == panel;
            if (p.gameObject.activeSelf != enable)
                p.gameObject.SetActive(enable);
        }
        activePanel = panel;
    }

    /// <summary>Switches to Party section (default landing view).</summary>
    public void GoToPartySection()
    {
        Activate(partyPanel);
        partyController?.OnActivated();
    }

    /// <summary>Switches to Shop section.</summary>
    public void GoToShopSection()
    {
        Activate(shopPanel);
        shopController?.OnActivated();
    }

    /// <summary>Switches to Medical section.</summary>
    public void GoToMedicalSection()
    {
        Activate(medicalPanel);
        medicalController?.OnActivated();
    }

    /// <summary>Switches to Residence section.</summary>
    public void GoToResidenceSection()
    {
        Activate(residencePanel);
        residenceController?.OnActivated();
    }

    /// <summary>Switches to Blacksmith section.</summary>
    public void GoToBlacksmithSection()
    {
        Activate(blacksmithPanel);
        blacksmithController?.OnActivated();
    }

    /// <summary>
    /// Transition to overworld using the centralized fade + loading flow.
    /// </summary>
    public void GoToOverworld()
    {
        scene.Fade.ToOverworld();
    }

    /// <summary>
    /// Transition to battle using the centralized fade + loading flow.
    /// </summary>
    public void GoToBattle()
    {
        scene.Fade.ToGame();
    }
}
