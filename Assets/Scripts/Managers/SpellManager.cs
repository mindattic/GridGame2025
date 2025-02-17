using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MagicBallType { GreenSparkle, Fireball }

public class SpellManager : MonoBehaviour
{
    protected BoardInstance board => GameManager.instance.board;
    [SerializeField] public GameObject spellPrefab; // Assigned via inspector

    // Dictionary to keep track of active spells.
    Dictionary<string, SpellInstance> spells = new Dictionary<string, SpellInstance>();

    private void Awake()
    {
        // Optionally initialize here.
    }

    // Spawns a SpellInstance configured by type.
    public void Spawn(
        ActorInstance caster,
        ActorInstance target,
        MagicBallType type,
        System.Action onArrival)
    {
        // Decide parameters based on type.
        string trailKey = "";
        string vfxKey = "";
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 0);
        float duration = 1.0f;

        switch (type)
        {
            case MagicBallType.GreenSparkle:
                trailKey = "GreenSparkle";
                vfxKey = "BuffLife";
                AnimationCurve overshootCurve = new AnimationCurve(
                                                new Keyframe(0f, 0f),
                                                new Keyframe(0.5f, 1.2f),
                                                new Keyframe(1f, 1f)
                                            );
                curve = overshootCurve;
                // Optionally assign a designer-set curve here.
                duration = 1.2f;
                break;
            case MagicBallType.Fireball:
                trailKey = "Fireball";
                vfxKey = "PuffyExplosion";
                // Optionally assign a different curve here.
                duration = 0.8f;
                break;
        }

        // Instantiate the SpellInstance prefab at the caster's position.
        var prefab = Instantiate(spellPrefab, caster.position, Quaternion.identity);
        var instance = prefab.GetComponent<SpellInstance>();
        instance.name = $"Spell_{trailKey}_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        instance.caster = caster;
        instance.target = target;
        instance.trailKey = trailKey;
        instance.vfxKey = vfxKey;
        instance.pathCurve = curve;
        instance.travelDuration = duration;
        instance.onArrival = onArrival;
        spells.Add(instance.name, instance);
        instance.Spawn();
    }

    public void Despawn(string name)
    {
        Destroy(spells[name].gameObject);
        spells.Remove(name);
    }


}
