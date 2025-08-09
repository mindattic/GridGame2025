using Assets.Helper;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using g = Assets.Helpers.GameHelper;

public class ProjectileInstance : MonoBehaviour
{
    public Transform parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true);
    }

    public Vector3 position
    {
        get => gameObject.transform.position;
        set => gameObject.transform.position = value;
    }

    public Quaternion rotation
    {
        get => gameObject.transform.rotation;
        set => gameObject.transform.rotation = value;
    }

    public Vector3 scale
    {
        get => gameObject.transform.localScale;
        set => gameObject.transform.localScale = value;
    }

    private ProjectileSettings projectile = new ProjectileSettings();

    // Private fields for move and for the instantiated trailInstance.
    private Vector3 startPosition;
    private Vector3 endPosition;
    private GameObject trailInstance;

    public IEnumerator Spawn(ProjectileSettings projectile)
    {
        this.projectile = projectile;
        yield return SpawnTrail();
        yield return SpawnVFX();
        g.ProjectileManager.Despawn(gameObject.name);
    }

    private IEnumerator SpawnTrail()
    {
        startPosition = projectile.startPosition;
        endPosition = projectile.target.position;
      
        TrailEffectAsset asset = TrailEffectRepo.TrailEffects[projectile.trailKey];
        trailInstance = Instantiate(asset.Prefab, transform.position, Quaternion.identity, transform);
        trailInstance.name = $"TailEffect_{projectile.friendlyName}_{Guid.NewGuid():N}";
        trailInstance.transform.localPosition = asset.RelativeOffset;
        trailInstance.transform.localEulerAngles = asset.AngularRotation;
        trailInstance.transform.localScale = g.TileScale.MultiplyBy(asset.RelativeScale);

        switch (projectile.path)
        {
            case ProjectilePath.BezierCurve:
                yield return StartCoroutine(MoveAlongBezierCurve());
                break;
            case ProjectilePath.AnimationCurve:
            default:
                yield return StartCoroutine(MoveAlongCurve());
                break;
        }

    }

    private IEnumerator MoveAlongCurve()
    {
        float elapsed = 0f;
        Vector3 direction = (endPosition - startPosition).normalized; // Travel direction
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized; // Perpendicular axis
        Vector3 position;

        while (elapsed < projectile.duration)
        {
            float t = elapsed / projectile.duration;

            // Interpolate position along the travel curve
            position = Vector3.Lerp(startPosition, endPosition, projectile.travelCurve.Evaluate(t));

            // Calculate wave offset along the perpendicular direction
            float waveOffset = projectile.waveCurve.Evaluate(t);
            position += perpendicular * waveOffset;

            // Apply position update
            transform.position = position;
            elapsed += Time.deltaTime;
            yield return Wait.None();
        }

        transform.position = endPosition;
    }

    private IEnumerator MoveAlongBezierCurve()
    {
        if (projectile.controlPoints == null || projectile.controlPoints.Count < 2)
        {
            projectile.controlPoints = GenerateBezierControlPoints();
        }

        float elapsed = 0f;
        float t;
        Vector3 position;
        while (elapsed < projectile.duration)
        {
            t = elapsed / projectile.duration;
            position = EvaluateBezier(projectile.controlPoints, t);

            // Seek the projectile instance, not just the trail!
            transform.position = position;

            elapsed += Time.deltaTime;
            yield return Wait.None();
        }

        // Snap to final position
        transform.position = projectile.controlPoints[projectile.controlPoints.Count - 1];
    }


    private Vector3 EvaluateBezier(List<Vector3> points, float t)
    {
        if (points.Count == 1)
            return points[0];


        List<Vector3> newPoints = new List<Vector3>();
        for (int i = 0; i < points.Count - 1; i++)
        {
            newPoints.Add(Vector3.Lerp(points[i], points[i + 1], t));
        }

        return EvaluateBezier(newPoints, t);
    }

    private List<Vector3> GenerateBezierControlPoints()
    {
        List<Vector3> controlPoints = new List<Vector3>();
        Vector3 start = startPosition;
        Vector3 end = endPosition;

        float distance = Vector3.Distance(start, end);
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        // Always add start position as first point
        controlPoints.Add(start);

        // TriggerEnqueueAttacks if control points are provided
        if (projectile.controlPoints != null && projectile.controlPoints.Count > 0)
        {
            // Use provided control points
            controlPoints.AddRange(projectile.controlPoints);
        }
        else
        {
            // Default to 2 auto-generated control points for a smooth curve
            int numControlPoints = 2;

            for (int i = 1; i <= numControlPoints; i++)
            {
                float factor = (float)i / (numControlPoints + 1); // Distribute points evenly
                float forwardOffset = distance * factor;
                float sideOffset = Mathf.Sin(factor * Mathf.PI) * distance * projectile.curveDeviation;
                float heightOffset = Mathf.Cos(factor * Mathf.PI) * distance * projectile.curveHeightFactor;

                Vector3 control = start
                    + direction * forwardOffset
                    + perpendicular * sideOffset
                    + Vector3.up * heightOffset;

                controlPoints.Add(control);
            }
        }

        // Always add end position as last point
        controlPoints.Add(end);

        return controlPoints;
    }

    //List<Vector3> GenerateCubicBezierControlPoints()
    //{
    //    List<Vector3> controlPoints = new List<Vector3>();
    //    Vector3 start = startPosition;
    //    Vector3 end = endPosition;

    //    float distance = Vector3.Distance(start, end);
    //    Vector3 direction = (end - start).normalized;
    //    Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

    //    // First control point - closer to start
    //    Vector3 control1 = start
    //        + direction * (distance * 0.3f)    // Seek forward (30% of distance)
    //        + perpendicular * (distance * 0.5f) // Side deviation
    //        + Vector3.up * (distance * 0.8f);  // Height deviation

    //    // Second control point - closer to end
    //    Vector3 control2 = end
    //        - direction * (distance * 0.3f)    // Seek backward (mirrored to control1)
    //        - perpendicular * (distance * 0.5f) // Side deviation (mirrored to control1)
    //        + Vector3.up * (distance * 0.8f);  // Height deviation (same as control1)

    //    // Add all points in order
    //    controlPoints.Add(start);   // P0
    //    controlPoints.Add(control1); // P1
    //    controlPoints.Add(control2); // P2
    //    controlPoints.Add(end);     // P3

    //    return controlPoints;
    //}

    private IEnumerator SpawnVFX()
    {
        //TODO: Differnet trail hides? Hide, FadeInstance, Shrink, etc...
        trailInstance.SetActive(false); //Hide trail until end

        VFXAsset vfxResource = VisualEffectRepo.VisualEffects[projectile.vfxKey];
        yield return g.VfxManager.SpawnTrigger(vfxResource, projectile.target.position, projectile.trigger);

        if (trailInstance != null)
            Destroy(trailInstance);
    }


}
