using System.Collections;
using UnityEngine;
using game = GameManagerHelper;

/// <summary>
/// A TriggerEvent that spawns a specified VFX at a given position, 
/// then executes an optional chained TriggerEvent.
/// </summary>
public class SpawnVFXTriggerEvent : TriggerEvent
{
    private VFXAsset vfx;
    private Vector3 position;
    private TriggerEvent chainedTrigger;

    /// <summary>
    /// Constructor for SpawnVFXTriggerEvent.
    /// </summary>
    /// <param name="vfx">The VFXAsset to spawn.</param>
    /// <param name="position">The position at which to spawn the VFX.</param>
    /// <param name="chainedTrigger">An optional TriggerEvent to execute after spawning the VFX.</param>
    public SpawnVFXTriggerEvent(VFXAsset vfx, Vector3 position, TriggerEvent chainedTrigger = null)
    {
        this.vfx = vfx;
        this.position = position;
        this.chainedTrigger = chainedTrigger;
    }

    public override IEnumerator Run()
    {
        GameManager.instance.vfxManager.Spawn(vfx, position);

        if (chainedTrigger != null)
            yield return chainedTrigger.Run();

        HasExecuted = true;
    }
}
