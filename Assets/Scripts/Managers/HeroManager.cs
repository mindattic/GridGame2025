using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    //Quick Reference Properties
    protected IEnumerable<ActorInstance> heroes => GameManager.instance.heroes;

    public void TriggerGlow()
    {
        heroes.Where(x => x.isPlaying).ToList().ForEach(x => x.glow.TriggerGlow());
    }

}
