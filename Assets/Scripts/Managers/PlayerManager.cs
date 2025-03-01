using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //Quick Reference Properties
    protected IQueryable<ActorInstance> players => GameManager.instance.players;

    public void TriggerGlow()
    {
        players.Where(x => x.isPlaying).ToList().ForEach(x => x.glow.TriggerGlow());
    }

}
