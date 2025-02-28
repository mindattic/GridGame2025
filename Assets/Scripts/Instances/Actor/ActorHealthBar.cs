using Assets.Scripts.Behaviors.Actor;
using Assets.Scripts.Instances.Actor;
using System.Collections;
using UnityEngine;

public class ActorHealthBar
{
    //External properties
    protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
    protected ActorRenderers render => instance.render;
    protected ActorStats stats => instance.stats;

    //Internal properties
    public bool isEmpty => !isDraining && stats.PreviousHP < 1;

    //Fields
    private ActorInstance instance;
    public bool isDraining;
    

    public void Initialize(ActorInstance parentInstance)
    {
        this.instance = parentInstance;
    }

    //Properties
    private Vector3 initialScale => render.healthBarBack.transform.localScale;

    private Vector3 GetScale(float value)
    {
        return new Vector3(
            Mathf.Clamp(initialScale.x * (value / stats.MaxHP), 0, initialScale.x),
            initialScale.y,
            initialScale.z);
    }

    public void Update()
    {
        render.healthBarDrain.transform.localScale = GetScale(stats.PreviousHP);
        render.healthBarFill.transform.localScale = GetScale(stats.HP);
        render.healthBarText.text = $@"{stats.HP}/{stats.MaxHP}";

        if (instance.isActive)
            instance.StartCoroutine(Drain());
    }

    private IEnumerator Drain()
    {
        //EnqueueAttacks abort conditions
        if (stats.PreviousHP == stats.HP)
            yield break;

        //Before:
        Vector3 scale;
        isDraining = true;

        //During:
        yield return Wait.For(Intermission.Before.HealthBar.Drain);

        while (stats.HP < stats.PreviousHP)
        {
            stats.PreviousHP -= Increment.HealthBar.Drain;
            stats.PreviousHP = Mathf.Clamp(stats.PreviousHP, stats.HP, stats.MaxHP);
            scale = GetScale(stats.PreviousHP);
            render.healthBarDrain.transform.localScale = scale;
            yield return Wait.OneTick();
        }

        //After:
        stats.PreviousHP = stats.HP;
        scale = GetScale(stats.PreviousHP);
        render.healthBarDrain.transform.localScale = scale;
        isDraining = false;
    }


}
