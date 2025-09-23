using Assets.Helpers;
using Assets.Scripts.Libraries;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using g = Assets.Helpers.GameHelper;

public class AbilityButtonManager : MonoBehaviour
{
    private GameObject abilityButtonPrefab;
    private Transform abilityButtonContainer;

    public void Awake()
    {
        abilityButtonContainer = GameObject.Find("AbilityButtonContainer").transform;
        abilityButtonPrefab = PrefabLibrary.Prefabs["AbilityButtonPrefab"];
    }

    public List<AbilityButton> buttons = new();

    public void Show(ActorInstance actor)
    {
        Hide();

        if (actor.characterName == CharacterHelper.Cleric)
        {
            var abilities = new List<Ability>();
            var a1 = new Ability()
            {
                name = "Spark of Healing",
                type = AbilityType.TargetAlly,
                button = SpriteLibrary.AbilityButtons["Heal"]
            };
            abilities.Add(a1);

            foreach (var ability in abilities)
            {

                var go = Instantiate(abilityButtonPrefab, abilityButtonContainer);
                var instance = go.GetComponent<AbilityButton>();
                instance.name = $"AbilityButton_{ability.name.Replace(" ", "_")}";
                instance.GetComponent<Image>().sprite = ability.button;
                instance.GetComponentInChildren<TextMeshProUGUI>().text = "";
                buttons.Add(instance);
                instance.Initialize(ability, () => OnClick(actor, ability));
            }
        }
        else if (actor.characterName == CharacterHelper.Paladin)
        {
            var abilities = new List<Ability>();
            var shieldBash = new Ability()
            {
                name = "Shield Bash",
                type = AbilityType.TargetOpponent,
                button = SpriteLibrary.AbilityButtons.ContainsKey("ShieldBash") ? SpriteLibrary.AbilityButtons["ShieldBash"] : null
            };
            abilities.Add(shieldBash);

            foreach (var ability in abilities)
            {
                var go = Instantiate(abilityButtonPrefab, abilityButtonContainer);
                var instance = go.GetComponent<AbilityButton>();
                instance.name = $"AbilityButton_{ability.name.Replace(" ", "_")}";
                instance.GetComponent<Image>().sprite = ability.button;
                instance.GetComponentInChildren<TextMeshProUGUI>().text = "";
                buttons.Add(instance);
                instance.Initialize(ability, () => OnClick(actor, ability));
            }
        }

    }

    private void OnClick(ActorInstance actor, Ability ability)
    {
        // Paladin's Shield Bash uses LinearTarget mode, not the generic AbilityTarget flow
        var normalizedName = (ability?.name ?? string.Empty).Replace(" ", string.Empty).ToLowerInvariant();
        if (actor != null && actor.characterName == CharacterHelper.Paladin && normalizedName == "shieldbash")
        {
            g.InputManager.InputMode = InputMode.LinearTarget;
            g.TileManager.HighlightLinearPaths(actor.location);
            g.InputManager.BeginAbilityTargeting(actor); // cache acting hero for input handler
            g.InputManager.ShowCancelButton();
            // Important: ensure the current press is released so LinearTarget sees a fresh Began
            g.InputManager.RequireTouchRelease();
            return;
        }

        if (ability.requiresTarget)
        {
            // switch into target mode
            g.InputManager.InputMode = InputMode.AnyActorTarget;
            g.InputManager.ShowCancelButton();
            // Same gating for touch so the first tap on target registers as Began
            g.InputManager.RequireTouchRelease();
        }
        else
        {
            ability.Activate(actor, null);
        }
    }


    public void Hide()
    {
        foreach (var btn in buttons)
            Destroy(btn.gameObject);

        buttons.Clear();
    }
}
