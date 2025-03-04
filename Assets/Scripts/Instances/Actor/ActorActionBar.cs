using Assets.Scripts.Behaviors.Actor;
using System.Collections;
using UnityEngine;

// ActorActionBar is responsible for managing and updating the visual representation 
// of an actor's action points (AP) in the UI. It handles the fill and drain animations 
// for the action bar based on the actor's current and maximum AP.
public class ActorActionBar
{
    // Quick Reference Properties:
    // These properties provide direct access to relevant game managers and the current actor's modules.
    protected DebugManager debugManager => GameManager.instance.debugManager;
    // Retrieves the flags module from the current actor instance to check various state flags.
    protected ActorFlags flags => instance.flags;
    // Provides access to the currently selected player (if any) from the GameManager.
    protected ActorInstance selectedPlayer => GameManager.instance.selectedPlayer;
    // Retrieves the rendering module, which manages all UI elements related to the actor.
    protected ActorRenderers render => instance.render;
    // Retrieves the stats module that stores the actor's AP, MaxAP, Speed, etc.
    protected ActorStats stats => instance.stats;
    // Indicates whether a player is selected.
    protected bool hasSelectedPlayer => selectedPlayer != null;

    // Internal property:
    // Cache the initial scale of the action bar's background element, used for calculating fill/drain scale.
    private Vector3 initialScale => render.actionBarBack.transform.localScale;

    // Field to store the parent actor instance that this action bar is associated with.
    private ActorInstance instance;

    // Initialize sets up the ActionBar by linking it to its parent ActorInstance.
    public void Initialize(ActorInstance parentInstance)
    {
        this.instance = parentInstance;
    }

    // GetScale calculates the scaled width for the action bar elements based on a given AP value.
    // It scales the x-component proportionally to the fraction of AP relative to MaxAP and clamps it between 0 and the initial width.
    private Vector3 GetScale(float value)
    {
        return new Vector3(
            Mathf.Clamp(initialScale.x * (value / stats.MaxAP), 0, initialScale.x),
            initialScale.y,
            initialScale.z);
    }

    // Update refreshes the action bar UI to reflect the actor's current AP values.
    // It adjusts the fill and drain bar scales, updates the text display, triggers a weapon wiggle animation,
    // and initiates the drain animation.
    public void Update()
    {
        render.actionBarDrain.transform.localScale = GetScale(stats.PreviousAP);
        render.actionBarFill.transform.localScale = GetScale(stats.AP);
        render.actionBarText.text = $@"{stats.AP}/{stats.MaxAP}";

        // Trigger visual feedback on the actor's weapon.
        instance.action.TriggerWeaponWiggle();
        // Start the drain animation if needed.
        TriggerDrain();
    }

    // TriggerDrain starts the drain coroutine to animate the reduction of the drain bar,
    // but only if the actor instance is active.
    private void TriggerDrain()
    {
        if (instance.isActive)
            instance.StartCoroutine(Drain());
    }

    // Drain is a coroutine that gradually reduces the displayed AP on the drain bar until it matches the current AP.
    // It waits for a brief interval before starting, then decreases stats.PreviousAP in increments,
    // updating the scale of the drain bar each tick.
    private IEnumerator Drain()
    {
        // Abort if no drain is required (i.e., current AP equals previous AP).
        if (stats.PreviousAP == stats.AP)
            yield break;

        // Local variable to hold the computed scale.
        Vector3 scale;

        // Wait for a pre-defined delay before beginning the drain animation.
        yield return Wait.For(Intermission.Before.ActionBar.Drain);

        // Gradually decrease PreviousAP until it matches the current AP.
        while (stats.AP < stats.PreviousAP)
        {
            stats.PreviousAP -= Increment.ActionBar.Drain;
            scale = GetScale(stats.PreviousAP);
            render.actionBarDrain.transform.localScale = scale;
            yield return Wait.OneTick();
        }

        // After draining, synchronize PreviousAP with the current AP and update the health bar drain element.
        stats.PreviousAP = stats.AP;
        scale = GetScale(stats.PreviousAP);
        render.healthBarDrain.transform.localScale = scale;
    }

    // TriggerFill starts the coroutine that fills the action bar (increasing AP) if conditions are met.
    public void TriggerFill()
    {
        if (instance.isActive)
            instance.StartCoroutine(Fill());
    }

    // Fill is a coroutine that incrementally increases the actor's AP based on its Speed stat.
    // It continues to increase AP until the actor reaches max AP or one of the abort conditions occurs.
    private IEnumerator Fill()
    {
        // Abort the fill process if:
        // - The enemy is stunned,
        // - No player is selected,
        // - The actor is not an enemy,
        // - The actor is not playing,
        // - The actor already has max AP, or
        // - The actor is currently gaining AP.
        if (debugManager.isEnemyStunned || !hasSelectedPlayer || !instance.isEnemy || !instance.isPlaying || instance.hasMaxAP || flags.isGainingAP)
            yield break;

        // Before starting, mark that the actor is gaining AP and calculate the increment amount.
        flags.isGainingAP = true;
        float amount = stats.Speed * 0.1f;

        // During: Gradually increase AP until max AP is reached.
        while (hasSelectedPlayer && instance.isEnemy && instance.isPlaying && !instance.hasMaxAP)
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

    // Reset sets the actor's AP values to zero and refreshes the action bar UI.
    public void Reset()
    {
        stats.AP = 0;
        stats.PreviousAP = 0;
        Update();
    }

    // AddInitiative provides a small initial AP value based on the actor's Speed stat.
    // This is used to seed the initiative system, allowing for a randomized start.
    public void AddInitiative()
    {
        // TODO: Consider incorporating Stats.Luck for more nuanced randomization.
        float amount = stats.Speed * 0.01f;
        stats.AP = amount;
        stats.PreviousAP = amount;
        Update();
    }
}
