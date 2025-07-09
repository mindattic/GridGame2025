using System.Collections.Generic;
using UnityEngine;
using game = GameManagerHelper;


public class AbilityButtonManager : MonoBehaviour
{
    #region Game Properies
    protected InputManager inputManager => GameManager.instance.inputManager;
    protected TargetLineManager targetLineManager => GameManager.instance.targetLineManager;

    private GameObject abilityButtonPrefab;
    private Transform abilityButtonContainer;
    #endregion

    public void Awake()
    {
        abilityButtonContainer = GameObject.Find("AbilityButtonContainer").transform;
        abilityButtonPrefab = PrefabRepo.Prefabs["AbilityButtonPrefab"];
    }

    private List<AbilityButton> buttons = new();

    public void Show(ActorInstance actor)
    {
        Hide();


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
            instance.Initialize(ability, () => OnClick(actor, ability));
            buttons.Add(instance);

        }
    }

    private void OnClick(ActorInstance actor, Ability ability)
    {
        if (ability.requiresTarget)
        {
            // switch into target mode
            inputManager.inputMode = InputMode.AbilityTarget;

            //// record the origin and callback
            //Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //worldPos.z = 0f;

            //targetLineManager.BeginTargeting(worldPos, target =>
            //{
            //    // restore gameplay mode
            //    inputManager.inputMode = InputMode.HeroTurn;
            //    ability.Activate(actor, target);
            //});
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
