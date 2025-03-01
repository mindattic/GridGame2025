using Assets.Scripts.Actions;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{




    protected BoardInstance board => GameManager.instance.board;
    protected TurnManager turnManager => GameManager.instance.turnManager;
    protected ActionManager actionManager => GameManager.instance.actionManager;

    [SerializeField] public GameObject spellPrefab; // Assigned via inspector

    Dictionary<string, SpellInstance> spells = new Dictionary<string, SpellInstance>();

    // Spawns a SpellInstance configured by type.
    public IEnumerator Spawn(SpellSettings spell)
    {
        // Instantiate the SpellInstance prefab at the source's position.
        var prefab = Instantiate(spellPrefab, spell.source.position, Quaternion.identity);
        var instance = prefab.GetComponent<SpellInstance>();
        instance.name = $"Spell_{spell.friendlyName}_{Guid.NewGuid():N}";
        instance.parent = board.transform;
        spells.Add(instance.name, instance);
        yield return instance.Spawn(spell);
    }

    public void Despawn(string name)
    {

        Destroy(spells[name].gameObject);
        spells.Remove(name);
    }

    public void EnqueueHeal(ActorInstance source, ActorInstance target, bool castBeforeAttack = true)
    {
        var spell = new SpellSettings()
        {
            friendlyName = "Heal",
            source = source,
            target = target,
            path = SpellPath.BezierCurve,
            controlPoints = BezierCurveHelper.Gentle(source, target),
            trailKey = "GreenSparkle",
            vfxKey = "BuffLife",
            trigger = new Trigger(coroutine: target.Heal(10), isAsync: false)
        };

        var action = new CastSpellAction(spell);

        if (castBeforeAttack)
            actionManager.Insert(action);
        else
            actionManager.Add(action);
    }


    public void EnqueueFireball(ActorInstance source, ActorInstance target, bool castBeforeAttack = true)
    {
        var spell = new SpellSettings()
        {
            friendlyName = "Fireball",
            source = source,
            target = target,
            path = SpellPath.BezierCurve,
            controlPoints = BezierCurveHelper.Overshooting(source, target),
            trailKey = "Fireball",
            vfxKey = "PuffyExplosion",
            trigger = new Trigger(coroutine: target.FireDamage(10), isAsync: false)
        };

        var action = new CastSpellAction(spell);

        if (castBeforeAttack)
            actionManager.Insert(action);
        else
            actionManager.Add(action);
    }

}