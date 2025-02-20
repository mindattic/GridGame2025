using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    protected BoardInstance board => GameManager.instance.board;
    [SerializeField] public GameObject spellPrefab; // Assigned via inspector



    [SerializeField]


    // Dictionary to keep track of active spells.
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
}
