using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class HeroManager : MonoBehaviour
{

    public void Glow()
    {
        g.Actors.Heroes.Where(x => x.IsPlaying).ToList().ForEach(x => x.Glow.Glow());
    }

}
