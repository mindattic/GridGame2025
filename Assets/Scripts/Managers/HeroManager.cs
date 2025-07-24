using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameManagerHelper;

public class HeroManager : MonoBehaviour
{

    public void TriggerGlow()
    {
        g.Actors.Heroes.Where(x => x.isPlaying).ToList().ForEach(x => x.glow.TriggerGlow());
    }

}
