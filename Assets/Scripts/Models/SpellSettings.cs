using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{

    public class SpellSettings
    {
        public string friendlyName;
        public ActorInstance source;
        public ActorInstance target;
        public string trailKey;   // e.g., "GreenSparkle" or "Fireball"
        public string vfxKey;    // e.g., "BuffLife" or "PuffyExplosion"
        public SpellPath path = SpellPath.AnimationCurve;
        public float duration = 1.0f;
        public Trigger trigger;

        //AnimationCurve
        public AnimationCurve travelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve waveCurve = AnimationCurve.EaseInOut(0, 0, 1, 0); //Straight: new Keyframe(0, 0), new Keyframe(1, 0));


        //Bezier
        public float travelModifier;
        public float waveModifier;
        public float launchAngle = 180f;
        public float curveDeviation = 30f;
        public float launchDistanceFactor = 0.5f;
        public float curveHeightFactor = 1.5f;
        public List<Vector3> controlPoints;
    }

}
