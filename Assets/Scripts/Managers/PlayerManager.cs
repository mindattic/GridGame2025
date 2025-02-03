using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //External properties
    protected IQueryable<ActorInstance> players => GameManager.instance.players;

    public void TriggerGlow()
    {
        players.Where(x => x.isActive && x.isAlive).ToList().ForEach(x => x.glow.TriggerGlow());
    }

}
