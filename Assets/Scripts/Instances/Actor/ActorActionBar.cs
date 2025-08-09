using Assets.Helper;
using Assets.Scripts.Behaviors.Actor;
using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

// ActorActionBar is responsible for managing and updating the visual representation 
// of an actor's action points (AP) in the UI. It handles the fill and drain animations 
// for the action fill based on the actor's CurrentProfile and maximum AP.
public class ActorActionBar
{
    protected ActorFlags flags => instance.flags;
    protected ActorRenderers render => instance.render;
    protected ActorStats stats => instance.stats;

    private Vector3 initialScale => render.actionBarBack.transform.localScale;

    // Field to store the parent actor actors that this action fill is associated with.
    private ActorInstance instance;

    // Show sets up the ActionBar by linking it to its parent ActorInstance.
    public void Initialize(ActorInstance parentInstance)
    {
        this.instance = parentInstance;
    }

    // GetScale calculates the scaled width for the action fill elements based on a given AP value.
    // It scales the x-component proportionally to the fraction of AP relative to MaxAP and clamps it between 0 and the initial width.
    private Vector3 GetScale(float value)
    {
        return new Vector3(
            Mathf.Clamp(initialScale.x * (value / stats.MaxAP), 0, initialScale.x),
            initialScale.y,
            initialScale.z);
    }

    // Save refreshes the action fill UI to reflect the actor's CurrentProfile AP values.
    // It adjusts the fill and drain fill scales, updates the textarea display, triggers a weapon wiggle action,
    // and initiates the drain action.
    public void Update()
    {
        render.actionBarDrain.transform.localScale = GetScale(stats.PreviousAP);
        render.actionBarFill.transform.localScale = GetScale(stats.AP);
        render.actionBarText.text = $@"{stats.AP}/{stats.MaxAP}";

        // TriggerEvent visual feedback on the actor's weapon.
        instance.action.TriggerWeaponWiggle();
        // Bounce the drain action if needed.
        TriggerDrain();
    }

    // TriggerDrain starts the drain coroutine to action the reduction of the drain fill,
    // but only if the actor actors is active.
    private void TriggerDrain()
    {
        if (instance.isActive)
            instance.StartCoroutine(Drain());
    }

    // Drain is a coroutine that gradually reduces the displayed AP on the drain fill until it matches the CurrentProfile AP.
    // It waits for a brief interval before starting, then decreases stats.PreviousAP in increments,
    // updating the scale of the drain fill each tick.
    private IEnumerator Drain()
    {
        // Abort if no drain is required (i.e., CurrentProfile AP equals previous AP).
        if (stats.PreviousAP == stats.AP)
            yield break;

        // Local variable to hold the computed scale.
        Vector3 scale;

        // Wait for a pre-defined delay before beginning the drain action.
        yield return Wait.For(Intermission.Before.ActionBar.Drain);

        // Gradually decrease PreviousAP until it matches the CurrentProfile AP.
        while (stats.AP < stats.PreviousAP)
        {
            stats.PreviousAP -= Increment.ActionBar.Drain;
            scale = GetScale(stats.PreviousAP);
            render.actionBarDrain.transform.localScale = scale;
            yield return Wait.OneTick();
        }

        // After draining, synchronize PreviousAP with the CurrentProfile AP and update the health fill drain element.
        stats.PreviousAP = stats.AP;
        scale = GetScale(stats.PreviousAP);
        render.healthBarDrain.transform.localScale = scale;
    }

    // TriggerFill starts the coroutine that fills the action fill (increasing AP) if conditions are met.
    public void TriggerFill()
    {
        if (instance.isActive)
            instance.StartCoroutine(Fill());
    }

    // Fill is a coroutine that incrementally increases the actor's AP based on its Intelligence stat.
    // It continues to increase AP until the actor reaches max AP or one of the abort conditions occurs.
    private IEnumerator Fill()
    {
        // Abort the fill process if:
        // - The enemy is stunned,
        // - No hero is selected,
        // - The actor is not an enemy,
        // - The actor is not playing,
        // - The actor already has max AP, or
        // - The actor is currently gaining AP.
        if (g.DebugManager.isEnemyStunned || !g.Actors.HasSelectedHero|| !instance.isEnemy || !instance.isPlaying || instance.hasMaxAP || flags.isGainingAP)
            yield break;

        // Before starting, mark that the actor is gaining AP and calculate the increment amount.
        flags.isGainingAP = true;
        float amount = stats.Intelligence * 0.1f;

        // During: Gradually increase AP until max AP is reached.
        while (g.Actors.HasSelectedHero && instance.isEnemy && instance.isPlaying && !instance.hasMaxAP)
        {
            stats.AP += amount;
            stats.AP = Mathf.Clamp(stats.AP, 0, stats.MaxAP);
            stats.PreviousAP = stats.AP;
            Update();
            yield return Wait.OneTick();
        }

        // After: Finalize the AP values and update the UI.
        stats.PreviousAP = stats.AP;
        Update();
        flags.isGainingAP = false;
    }

    // Reset sets the actor's AP values to zero and refreshes the action fill UI.
    public void Reset()
    {
        stats.AP = 0;
        stats.PreviousAP = 0;
        Update();
    }

    // AddInitiative provides a small initial AP value based on the actor's Intelligence stat.
    // This is used to seed the initiative system, allowing for a randomized start.
    public void AddInitiative()
    {
        // TODO: Consider incorporating Stats.Luck for more nuanced randomization.
        float amount = stats.Intelligence * 0.01f;
        stats.AP = amount;
        stats.PreviousAP = amount;
        Update();
    }
}
