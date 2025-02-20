using Assets.Scripts.Models;
using Game.Behaviors;
using System.Collections;
using TMPro;
using UnityEngine;


public class SpellInstance : MonoBehaviour
{
    protected BoardInstance board => GameManager.instance.board;
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected VFXManager vfxManager => GameManager.instance.vfxManager;
    protected SpellManager spellManager => GameManager.instance.spellManager;
    protected float tileSize => GameManager.instance.tileSize;


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
        trailInstance = Instantiate(trailResource.Prefab, transform.position, Quaternion.identity, transform);
        //trailInstance.transform.parent = board.transform;
        trailInstance.transform.localPosition = trailResource.RelativeOffset;
        trailInstance.transform.localEulerAngles = trailResource.AngularRotation;
        trailInstance.transform.localScale = trailResource.RelativeScale;

        switch (spell.path)
        {
            case SpellPath.AnimationCurve:
                yield return StartCoroutine(MoveAlongCurve());
                break;
            case SpellPath.Elastic:
                yield return StartCoroutine(MoveAlongElastic());
                break;
            case SpellPath.BezierCurve:
                yield return StartCoroutine(MoveAlongBezierCurve());
                break;
            case SpellPath.CubicBezierCurve:
                yield return StartCoroutine(MoveAlongCubicBezierCurve());
                break;
            default:
                yield return StartCoroutine(MoveAlongCurve());
                break;
        }

    }

    private IEnumerator MoveAlongCurve()
    {
        float elapsed = 0f;
        while (elapsed < spell.duration)
        {
            float t = elapsed / spell.duration;

            // Interpolate between start and end positions.
            var pos = Vector3.Lerp(startPosition, endPosition, spell.curve.Evaluate(t));


            //var offset = Random.Float(-45, 45);
            //pos.y += offset;

            trailInstance.transform.position = pos;
            elapsed += Time.deltaTime;
            yield return null;
        }


        transform.position = endPosition;
    }

    private IEnumerator MoveAlongElastic()
    {
        float elapsed = 0f;

        Vector3 direction = (endPosition - startPosition);
        Vector3 initialOffset = startPosition;

        while (elapsed < spell.duration)
        {
            float t = elapsed / spell.duration;

            // Damped spring function with smooth approach
            float springFactor
                = spell.overshootIntensity
                * (1 - Mathf.Exp(-spell.dampingFactor * t)
                * Mathf.Cos(spell.oscillationFrequency * t));

            // Apply movement along the calculated direction
            trailInstance.transform.position = Vector3.Lerp(
                transform.position,
                initialOffset + direction * springFactor,
                spell.smoothingFactor);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final smoothing to ensure exact target position
        float smoothDuration = 0.1f;
        float smoothElapsed = 0f;
        while (smoothElapsed < smoothDuration)
        {
            trailInstance.transform.position = Vector3.Lerp(transform.position, endPosition, smoothElapsed / smoothDuration);
            smoothElapsed += Time.deltaTime;
            yield return null;
        }

        trailInstance.transform.position = endPosition;

    }

    //private IEnumerator MoveAlongBezierCurve()
    //{
    //    float elapsed = 0f;

    //    // Compute direction from source to target
    //    Vector3 direction = (endPosition - startPosition).normalized;

    //    // Apply launch angle offset
    //    Vector3 launchDirection = Quaternion.Euler(0, spell.launchAngle, 0) * direction;

    //    // Determine launch point (how far the spell travels before curving)
    //    float launchDistance = Vector3.Distance(startPosition, endPosition) * spell.launchDistanceFactor;
    //    Vector3 launchPoint = startPosition + (launchDirection * launchDistance);

    //    // Apply curve deviation
    //    Vector3 curveDirection = Quaternion.Euler(0, spell.curveDeviation, 0) * (endPosition - launchPoint).normalized;
    //    Vector3 controlPoint = Vector3.Lerp(launchPoint, endPosition, 0.5f)
    //                           + curveDirection * (launchDistance * spell.launchDistanceFactor)
    //                           + Vector3.up * spell.curveHeightFactor;

    //    Debug.Log($"[Bezier] Start={startPosition}, End={endPosition}, Launch={launchPoint}, Control={controlPoint}");

    //    while (elapsed < spell.duration)
    //    {
    //        float t = elapsed / spell.duration;

    //        // Quadratic Bezier
    //        Vector3 pos = (1 - t) * (1 - t) * startPosition
    //                    + 2 * (1 - t) * t * controlPoint
    //                    + t * t * endPosition;

    //        trailInstance.transform.position = pos;

    //        elapsed += Time.deltaTime;
    //        yield return null;
    //    }

    //    // Snap exactly to the end
    //    trailInstance.transform.position = endPosition;
    //}

    private IEnumerator MoveAlongBezierCurve()
    {
        // === Variables to tweak ===
        float duration = 5f;         // total flight time in seconds
        float sideAngleDegrees = 30f;        // rotate 'forwardDir' by this many degrees to left/right
        float sideFactor = 1.5f;       // how wide the arc is as a fraction of distance
        float upFactor = 1.0f;       // how high the arc is as a fraction of distance
        float forwardFactor = 0.5f;       // how far along the path to place the control point (in forward direction)
        // ==========================

        // Basic references
        Vector3 start = startPosition;
        Vector3 end = endPosition;

        float distance = Vector3.Distance(start, end);
        Vector3 directFwd = (end - start).normalized;

        // 1) Rotate the forward direction by sideAngleDegrees around Y-axis
        Vector3 angledForward = Quaternion.Euler(0f, sideAngleDegrees, 0f) * directFwd;

        // 2) A perpendicular direction to the angledForward for big side arcs
        Vector3 sideDir = Vector3.Cross(angledForward, Vector3.up).normalized;

        // 3) Define a single control point for the quadratic
        //    This point is partway along angledForward, plus some side/up offsets
        //    e.g., big arc = big sideFactor / upFactor
        Vector3 control = start
            + angledForward * (distance * forwardFactor)
            + sideDir * (distance * sideFactor)
            + Vector3.up * (distance * upFactor);

        // 4) Animate from t=0 to t=1 over 'duration'
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float omt = 1f - t;

            // Quadratic Bezier formula:
            // B(t) = (1 - t)^2 * start
            //      + 2 (1 - t) t * control
            //      + t^2         * end
            Vector3 pos =
                  (omt * omt) * start
                + 2f * omt * t * control
                + (t * t) * end;

            trailInstance.transform.position = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to the final position
        trailInstance.transform.position = end;
    }

    private IEnumerator MoveAlongCubicBezierCurve()
    {
        // === Variables to tweak ===
        float duration = 5f;    // total flight time in seconds
        float sideAngleDegrees = 30f;   // rotate 'forwardDir' by this many degrees to left/right
        float sideFactor = 1.5f;  // how wide the arc is as a fraction of distance
        float upFactor = 1.0f;  // how high the arc is as a fraction of distance
        float forwardFactorStart = 0.3f;  // how far from the start to place the first control point
        float forwardFactorEnd = 0.3f;  // how far from the end to place the second control point
        // ==========================

        // Basic references
        Vector3 start = startPosition;
        Vector3 end = endPosition;

        float distance = Vector3.Distance(start, end);
        Vector3 directForward = (end - start).normalized;

        // 1) Rotate the forward direction by sideAngleDegrees around Y-axis
        //    Positive angle means "turn right," negative angle means "turn left."
        Vector3 angledForward = Quaternion.Euler(0f, sideAngleDegrees, 0f) * directForward;

        // 2) Compute a perpendicular direction to angledForward for big side arcs
        Vector3 sideDir = Vector3.Cross(angledForward, Vector3.up).normalized;

        // 3) Define two control points
        //    - control1 near the start, offset by angledForward + side/vertical
        //    - control2 near the end, offset in the opposite side direction
        Vector3 control1 = start
            + angledForward * (distance * forwardFactorStart)
            + sideDir * (distance * sideFactor)
            + Vector3.up * (distance * upFactor);

        Vector3 control2 = end
            - angledForward * (distance * forwardFactorEnd)
            - sideDir * (distance * sideFactor)
            + Vector3.up * (distance * upFactor);

        // 4) Animate from t=0 to t=1 over 'duration'
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float omt = 1f - t;

            // Standard Cubic Bezier formula:
            // B(t) = (1 - t)^3 * start
            //      + 3(1 - t)^2 t * control1
            //      + 3(1 - t) t^2 * control2
            //      + t^3 * end
            Vector3 pos =
                (omt * omt * omt) * start +
                3f * (omt * omt) * t * control1 +
                3f * omt * (t * t) * control2 +
                (t * t * t) * end;

            trailInstance.transform.position = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to the final position
        trailInstance.transform.position = end;
    }

    

    private IEnumerator SpawnVFX()
    {
        //TODO: Differnet trail hides? Hide, Fade, Shrink, etc...
        trailInstance.SetActive(false); //Hide trail until end

        VFXResource vfxResource = resourceManager.VisualEffect(spell.vfxKey);
        yield return vfxManager.Spawn(vfxResource, spell.target.position, spell.trigger);
    }


}
