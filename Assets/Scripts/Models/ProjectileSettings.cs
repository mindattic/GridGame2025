using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{
    /// <summary>
    /// Holds all configuration for spawning and animating a projectile.
    /// The 'routine' field is an IEnumerator to be started by the caller
    /// using FireAndForget or yielded via StartCoroutine when appropriate.
    /// </summary>
    public class ProjectileSettings
    {
        public string friendlyName;
        public Vector3 startPosition;
        public ActorInstance target;
        public string trailKey;
        public string vfxKey;
        public ProjectilePath path = ProjectilePath.AnimationCurve;
        public float duration = 1.0f;

        public IEnumerator routine;

        public AnimationCurve travelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve waveCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);

        public float travelModifier;
        public float waveModifier;
        public float launchAngle = 180f;
        public float curveDeviation = 30f;
        public float launchDistanceFactor = 0.5f;
        public float curveHeightFactor = 1.5f;
        public List<Vector3> controlPoints;
    }
}
