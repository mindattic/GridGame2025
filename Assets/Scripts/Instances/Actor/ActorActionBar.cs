using Assets.Scripts.Behaviors.Actor;
using Assets.Scripts.Instances.Actor;
using System.Collections;
using UnityEngine;

public class ActorActionBar
{
    //External properties
    protected DebugManager debugManager => GameManager.instance.debugManager;
    protected ActorFlags flags => instance.flags;
    protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
    protected ActorRenderers render => instance.render;
    protected ActorStats stats => instance.stats;
    protected bool hasSelectedPlayer => selectedPlayer != null;

    //Internal properties
    private Vector3 initialScale => render.actionBarBack.transform.localScale;

    //Fields
    private ActorInstance instance;

    public void Initialize(ActorInstance parentInstance)
    {
        this.instance = parentInstance;
    }

   
    private Vector3 GetScale(float value)
    {
        return new Vector3(
            Mathf.Clamp(initialScale.x * (value / stats.MaxAP), 0, initialScale.x),
            initialScale.y,
            initialScale.z);
    }

    public void Update()
    {
        render.actionBarDrain.transform.localScale = GetScale(stats.PreviousAP);
        render.actionBarFill.transform.localScale = GetScale(stats.AP);
        render.actionBarText.text = $@"{stats.AP}/{stats.MaxAP}";


        instance.action.TriggerWeaponWiggle();
        TriggerDrain();
    }

    private void TriggerDrain()
    {
        if (instance.isActive && instance.isAlive)
            instance.StartCoroutine(Drain());
    }

    private IEnumerator Drain()
    {
        //Check abort conditions
        if (stats.PreviousAP == stats.AP)
            yield break;

        //Before:
        Vector3 scale;

        //During:
        yield return Wait.For(Intermission.Before.ActionBar.Drain);

        while (stats.AP < stats.PreviousAP)
        {
            stats.PreviousAP -= Increment.ActionBar.Drain;
            scale = GetScale(stats.PreviousAP);
            render.actionBarDrain.transform.localScale = scale;
            yield return Wait.OneTick();
        }

        //After:
        stats.PreviousAP = stats.AP;
        scale = GetScale(stats.PreviousAP);
        render.healthBarDrain.transform.localScale = scale;
    }

    public void TriggerFill()
    {
        if (instance.isActive && instance.isAlive)
            instance.StartCoroutine(Fill());
    }

    private IEnumerator Fill()
    {
        //Check abort conditions
        if (debugManager.isEnemyStunned || !hasSelectedPlayer || !instance.isEnemy || !instance.isActive || !instance.isAlive || instance.hasMaxAP || flags.isGainingAP)
            yield break;

        //Before:
        flags.isGainingAP = true;
        float amount = stats.Speed * 0.1f;

        //During:
        while (hasSelectedPlayer && instance.isEnemy && instance.isActive && instance.isAlive && !instance.hasMaxAP)
        {
            stats.AP += amount;
            stats.AP = Mathf.Clamp(stats.AP, 0, stats.MaxAP);
            stats.PreviousAP = stats.AP;
            Update();
            yield return Wait.OneTick();
        }

        //After:
        stats.PreviousAP = stats.AP;
        Update();
        flags.isGainingAP = false;
    }

    public void Reset()
    {
        stats.AP = 0;
        stats.PreviousAP = 0;
        Update();
    }


    public void AddInitiative()
    {
        //TODO: SpawnActor randomization based on Stats.Luck...
        float amount = stats.Speed * 0.01f;
        stats.AP = amount;
        stats.PreviousAP = amount;
        Update();
    }







}
