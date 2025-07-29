using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;


public class AbilityButtonManager : MonoBehaviour
{
    private GameObject abilityButtonPrefab;
    private Transform abilityButtonContainer;

    public void Awake()
    {
        abilityButtonContainer = GameObject.Find("AbilityButtonContainer").transform;
        abilityButtonPrefab = PrefabRepo.Prefabs["AbilityButtonPrefab"];
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
                type = AbilityType.TargetAlly
            };
            abilities.Add(a1);

            foreach (var ability in abilities)
            {

                var prefab = Instantiate(abilityButtonPrefab, abilityButtonContainer);
                var instance = prefab.GetComponent<AbilityButton>();
                instance.name = $"AbilityButton_{ability.name.Replace(" ", "_")}";
                buttons.Add(instance);
                instance.Initialize(ability, () => OnClick(actor, ability));
            }
        }

    }

    private void OnClick(ActorInstance actor, Ability ability)
    {
        if (ability.requiresTarget)
        {
            // switch into targetActor mode
            g.InputManager.inputMode = InputMode.AbilityTarget;
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
