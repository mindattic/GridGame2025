using Assets.Scripts.Models;
using Game.Behaviors;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;


public class SpellInstance : MonoBehaviour
{
    protected BoardInstance board => GameManager.instance.board;
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected VFXManager vfxManager => GameManager.instance.vfxManager;
    protected SpellManager spellManager => GameManager.instance.spellManager;
    protected float tileSize => GameManager.instance.tileSize;
    protected Vector3 tileScale => GameManager.instance.tileScale;


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

    private SpellSettings spell = new SpellSettings();

    // Private fields for movement and for the instantiated trailInstance.
    private Vector3 startPosition;
    private Vector3 endPosition;
    private GameObject trailInstance;

    public IEnumerator Spawn(SpellSettings spell)
    {
        this.spell = spell;
        yield return SpawnTrail();
        yield return SpawnVFX();
        spellManager.Despawn(gameObject.name);
    }

    private IEnumerator SpawnTrail()
    {
        startPosition = spell.source.position;
        endPosition = spell.target.position;
        transform.position = startPosition;

        TrailResource trailResource = resourceManager.TrailEffect(spell.trailKey);
        trailInstance = Instantiate(trailResource.Prefab, transform.position, Quaternion.identity);
        //trailInstance.transform.parent = board.transform;
        trailInstance.transform.localPosition = trailResource.RelativeOffset;
        trailInstance.transform.localEulerAngles = trailResource.AngularRotation;
        trailInstance.transform.localScale = tileScale.MultiplyBy(trailResource.RelativeScale);

        switch (spell.path)
        {
            case SpellPath.AnimationCurve:
                yield return StartCoroutine(MoveAlongCurve());
                break;
            case SpellPath.BezierCurve:
                yield return StartCoroutine(MoveAlongBezierCurve());
                break;
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

        while (elapsed < spell.duration)
        {
            float t = elapsed / spell.duration;

            // Interpolate position along the travel curve
            Vector3 pos = Vector3.Lerp(startPosition, endPosition, spell.travelCurve.Evaluate(t));

            // Calculate wave offset along the perpendicular direction
            float waveOffset = spell.waveCurve.Evaluate(t);
            pos += perpendicular * waveOffset;

            // Apply position update
            trailInstance.transform.position = pos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }

    private IEnumerator MoveAlongBezierCurve()
    {
        if (spell.controlPoints == null || spell.controlPoints.Count < 2)
        {
            spell.controlPoints = GenerateBezierControlPoints();
        }

        float elapsed = 0f;
        while (elapsed < spell.duration)
        {
            float t = elapsed / spell.duration;
            Vector3 pos = EvaluateBezier(spell.controlPoints, t);

            trailInstance.transform.position = pos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to final position
        trailInstance.transform.position = spell.controlPoints[spell.controlPoints.Count - 1];
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

        // Check if control points are provided
        if (spell.controlPoints != null && spell.controlPoints.Count > 0)
        {
            // Use provided control points
            controlPoints.AddRange(spell.controlPoints);
        }
        else
        {
            // Default to 2 auto-generated control points for a smooth curve
            int numControlPoints = 2;

            for (int i = 1; i <= numControlPoints; i++)
            {
                float factor = (float)i / (numControlPoints + 1); // Distribute points evenly
                float forwardOffset = distance * factor;
                float sideOffset = Mathf.Sin(factor * Mathf.PI) * distance * spell.curveDeviation;
                float heightOffset = Mathf.Cos(factor * Mathf.PI) * distance * spell.curveHeightFactor;

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
    //        + direction * (distance * 0.3f)    // Move forward (30% of distance)
    //        + perpendicular * (distance * 0.5f) // Side deviation
    //        + Vector3.up * (distance * 0.8f);  // Height deviation

    //    // Second control point - closer to end
    //    Vector3 control2 = end
    //        - direction * (distance * 0.3f)    // Move backward (mirrored to control1)
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
        //TODO: Differnet trail hides? Hide, Fade, Shrink, etc...
        trailInstance.SetActive(false); //Hide trail until end

        VFXResource vfxResource = resourceManager.VisualEffect(spell.vfxKey);
        yield return vfxManager.Spawn(vfxResource, spell.target.position, spell.trigger);
    }


}
