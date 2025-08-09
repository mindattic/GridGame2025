using System.Collections.Generic;
using UnityEngine;

namespace Assets.Helper
{


    public static class BezierCurveHelper
    {
        /// <summary>
        /// Generates control points for a gentle S-curve move.
        /// Ensures the perpendicular wave follows the travel direction properly.
        /// </summary>
        public static List<Vector3> Gentle(Vector3 startPosition, ActorInstance target, float travelModifier = 1f, float waveModifier = 1.2f)
        {
            List<Vector3> controlPoints = new List<Vector3>();
            Vector3 start = startPosition;
            Vector3 end = target.position;

            float distance = Vector3.Distance(start, end);
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(Vector3.up, direction).normalized; // Ensure perpendicular aligns with direction

            // Alternate the wave direction properly
            float sideModifier1 = RNG.Boolean ? 1f : -1f;
            float sideModifier2 = -sideModifier1; // Ensure the second control point inverts the curve correctly

            Vector3 control1 = start
                + direction * (distance * 0.3f * travelModifier)  // Seek forward
                + perpendicular * (distance * 0.3f * sideModifier1 * waveModifier) // First curve direction
                + Vector3.up * (distance * 0.2f * sideModifier1 * waveModifier); // **Now alternates up/down**

            Vector3 control2 = end
                - direction * (distance * 0.3f * travelModifier)  // Seek slightly backward
                + perpendicular * (distance * 0.3f * sideModifier2 * waveModifier) // Reverse the curve direction
                + Vector3.up * (distance * 0.1f * sideModifier2 * waveModifier); // **Now alternates up/down**

            controlPoints.Add(start);
            controlPoints.Add(control1);
            controlPoints.Add(control2);
            controlPoints.Add(end);

            return controlPoints;
        }


        /// <summary>
        /// Generates control points for an overshooting arc.
        /// The projectile overshoots the target before curving back.
        /// </summary>
        public static List<Vector3> Overshooting(Vector3 startPosition, ActorInstance target, float travelModifier = 1.6f, float waveModifier = 0.2f, bool overshoot = true)
        {
            List<Vector3> controlPoints = new List<Vector3>();
            Vector3 start = startPosition;
            Vector3 end = target.position;

            float distance = Vector3.Distance(start, end);
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

            float verticalModifier = RNG.Boolean ? 1f : -1f;

            Vector3 control1 = start
                + direction * (distance * 0.5f * travelModifier)
                + perpendicular * (distance * 0.3f * waveModifier)
                + Vector3.up * (distance * 0.7f * verticalModifier * waveModifier);

            Vector3 control2 = end
                + direction * (distance * 0.3f * travelModifier)
                - perpendicular * (distance * 0.3f * waveModifier)
                + Vector3.up * (distance * 0.5f * verticalModifier * waveModifier);

            if (overshoot)
            {
                control2 += direction * (distance * 0.2f);
            }

            controlPoints.Add(start);
            controlPoints.Add(control1);
            controlPoints.Add(control2);
            controlPoints.Add(end);

            return controlPoints;
        }

        public static List<Vector3> OvershootingWave(Vector3 startPosition, ActorInstance target, float travelModifier = 1.6f, float waveModifier = 0.2f, bool overshoot = true)
        {
            List<Vector3> controlPoints = new List<Vector3>();
            Vector3 start = startPosition;
            Vector3 end = target.position;

            float distance = Vector3.Distance(start, end);
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

            float verticalModifier1 = RNG.Boolean ? 1f : -1f;
            float verticalModifier2 = -verticalModifier1; // Reverse the wave direction

            Vector3 control1 = start
                + direction * (distance * 0.5f * travelModifier)
                + perpendicular * (distance * 0.3f * waveModifier)
                + Vector3.up * (distance * 0.7f * verticalModifier1 * waveModifier); // Alternating up/down

            Vector3 control2 = end
                + direction * (distance * 0.3f * travelModifier)
                - perpendicular * (distance * 0.3f * waveModifier)
                + Vector3.up * (distance * 0.5f * verticalModifier2 * waveModifier); // Opposite vertical direction

            if (overshoot)
            {
                control2 += direction * (distance * 0.2f);
            }

            controlPoints.Add(start);
            controlPoints.Add(control1);
            controlPoints.Add(control2);
            controlPoints.Add(end);

            return controlPoints;
        }


        /// <summary>
        /// Generates control points for a lobbed arc.
        /// Similar to how a grenade or fireball might travel.
        /// </summary>
        public static List<Vector3> LobbedArc(Vector3 startPosition, ActorInstance target, float travelModifier = 0.8f, float waveModifier = 1.5f)
        {
            List<Vector3> controlPoints = new List<Vector3>();
            Vector3 start = startPosition;
            Vector3 end = target.position;

            float distance = Vector3.Distance(start, end);
            Vector3 direction = (end - start).normalized;

            Vector3 control1 = start
                + direction * (distance * 0.5f * travelModifier)
                + Vector3.up * (distance * 1.5f * waveModifier);

            Vector3 control2 = end
                - direction * (distance * 0.2f * travelModifier)
                + Vector3.up * (distance * 0.5f * waveModifier);

            controlPoints.Add(start);
            controlPoints.Add(control1);
            controlPoints.Add(control2);
            controlPoints.Add(end);

            return controlPoints;
        }

        /// <summary>
        /// Generates control points for a reverse boomerang arc.
        /// The projectile overshoots the target and curves back dramatically.
        /// </summary>
        public static List<Vector3> Boomerang(Vector3 startPosition, ActorInstance target, float travelModifier = 1.2f, float waveModifier = 0.8f)
        {
            List<Vector3> controlPoints = new List<Vector3>();
            Vector3 start = startPosition;
            Vector3 end = target.position;

            float distance = Vector3.Distance(start, end);
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

            float verticalModifier = RNG.Boolean ? 1f : -1f;

            Vector3 control1 = start
                + direction * (distance * 0.5f * travelModifier)
                + perpendicular * (distance * 0.3f * waveModifier)
                + Vector3.up * (distance * 1.0f * verticalModifier * waveModifier);

            Vector3 control2 = end
                + direction * (distance * 0.3f * travelModifier)
                - perpendicular * (distance * 0.3f * waveModifier)
                + Vector3.up * (distance * 0.5f * verticalModifier * waveModifier);

            controlPoints.Add(start);
            controlPoints.Add(control1);
            controlPoints.Add(control2);
            controlPoints.Add(end);

            return controlPoints;
        }

        /// <summary>
        /// Generates control points for a homing spiral effect.
        /// The projectile moves in a corkscrew pattern toward the target.
        /// </summary>
        public static List<Vector3> HomingSpiral(Vector3 startPosition, ActorInstance target, float travelModifier = 1f, float waveModifier = 2f)
        {
            List<Vector3> controlPoints = new List<Vector3>();
            Vector3 start = startPosition;
            Vector3 end = target.position;

            float distance = Vector3.Distance(start, end);
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

            Vector3 control1 = start
                + direction * (distance * 0.3f * travelModifier)
                + perpendicular * (distance * 0.5f * waveModifier)
                + Vector3.up * (distance * 0.5f * waveModifier);

            Vector3 control2 = start
                + direction * (distance * 0.6f * travelModifier)
                - perpendicular * (distance * 0.5f * waveModifier)
                + Vector3.up * (distance * 1.0f * waveModifier);

            controlPoints.Add(start);
            controlPoints.Add(control1);
            controlPoints.Add(control2);
            controlPoints.Add(end);

            return controlPoints;
        }

        /// <summary>
        /// Generates control points for a zig-zag dash.
        /// The projectile moves erratically toward the target.
        /// </summary>
        public static List<Vector3> ZigZagDash(Vector3 startPosition, ActorInstance target, float travelModifier = 1.1f, float waveModifier = 1.2f)
        {
            List<Vector3> controlPoints = new List<Vector3>();
            Vector3 start = startPosition;
            Vector3 end = target.position;

            float distance = Vector3.Distance(start, end);
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

            Vector3 control1 = start
                + direction * (distance * 0.25f * travelModifier)
                + perpendicular * (distance * 0.4f * waveModifier);

            Vector3 control2 = start
                + direction * (distance * 0.5f * travelModifier)
                - perpendicular * (distance * 0.4f * waveModifier);

            Vector3 control3 = start
                + direction * (distance * 0.75f * travelModifier)
                + perpendicular * (distance * 0.3f * waveModifier);

            controlPoints.Add(start);
            controlPoints.Add(control1);
            controlPoints.Add(control2);
            controlPoints.Add(control3);
            controlPoints.Add(end);

            return controlPoints;
        }


    }


}