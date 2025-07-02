using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;


public enum AbilityType
{
    Passive,
    TargetAlly,
    TargetOpponent,
    TargetAny,
    Self
}

[System.Serializable]
public class AbilityData
{
    public string name;
    public AbilityType type;
    public UnityEngine.Events.UnityEvent<ActorInstance> onTargetSelected;
}


public class AbilityButtonManager : MonoBehaviour
{
  
    protected InputManager inputManager => GameManager.instance.inputManager;
    protected TargetLineManager targetLineManager => GameManager.instance.targetLineManager;

    private GameObject abilityButtonPrefab;
    private Transform abilityButtonContainer;

    public void Awake()
    {
        abilityButtonContainer = GameObject.Find("AbilityButtonContainer").transform;
        abilityButtonPrefab = PrefabRepo.Prefabs["AbilityButtonPrefab"];
    }


    
    private List<AbilityButton> spawnedButtons = new();

    public void ShowAbilityButtons(ActorInstance actor)
    {
        ClearButtons();

        var abilities = new List<Ability>();
        var a1 = new Ability()
        {
            name = "Spark of Healing",
            type = AbilityType.TargetAlly
        };
        abilities.Add(a1);

        foreach (var ability in abilities)
        {

            var buttonGO = Instantiate(abilityButtonPrefab, abilityButtonContainer);
            var instance = buttonGO.GetComponent<AbilityButton>();
            instance.name = $"AbilityButton_{ability.name.Replace(" ", "_")}";
            instance.Initialize(ability, () => OnAbilityClicked(actor, ability));
            spawnedButtons.Add(instance);

        }
    }

    private void OnAbilityClicked(ActorInstance actor, Ability ability)
    {
        if (ability.requiresTarget)
        {
            // switch into target mode
            inputManager.inputMode = InputMode.AbilityTarget;

            // record the origin and callback
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;

            targetLineManager.BeginTargeting(worldPos, target =>
            {
                // restore gameplay mode
                inputManager.inputMode = InputMode.Gameplay;
                ability.Activate(actor, target);
            });
        }
        else
        {
            ability.Activate(actor, null);
        }
    }


    public void ClearButtons()
    {
        foreach (var btn in spawnedButtons)
            Destroy(btn.gameObject);

        spawnedButtons.Clear();
    }
}
