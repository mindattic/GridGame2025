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
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        //Elastic
        public float overshootIntensity = 1.3f;  // Overshoot intensity
        public float dampingFactor = 3.5f;  // Damping factor (higher = stops oscillating faster)
        public float oscillationFrequency = 8f; // Frequency of oscillation
        public float smoothingFactor = 0.9f; //0.9f means 90% of the new position is determined by the target position, and 10% is preserved from the previous frame.

        //Bezier
        public float launchAngle = 180f;
        public float curveDeviation = 30f;
        public float launchDistanceFactor = 0.5f;
        public float curveHeightFactor = 1.5f;
    }

}
