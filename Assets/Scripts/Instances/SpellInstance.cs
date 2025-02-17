using System.Collections;
using UnityEngine;

public class SpellInstance : MonoBehaviour
{
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;
    protected VFXManager vfxManager => GameManager.instance.vfxManager;
    protected SpellManager spellManager => GameManager.instance.spellManager;

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

    // Source and target actors.
    public ActorInstance caster;
    public ActorInstance target;

    // Keys for visual effects.
    public string trailKey;   // e.g., "GreenSparkle" or "Fireball"
    public string vfxKey;    // e.g., "BuffLife" or "PuffyExplosion"

    // Movement parameters.
    public AnimationCurve pathCurve; // Used to add an arc (e.g., vertical offset).
    public float travelDuration = 2.0f;

    // Callback to invoke on arrival (e.g., to heal or damage the target).
    public System.Action onArrival;

    // Private fields for movement and for the instantiated trail.
    private Vector3 startPosition;
    private Vector3 endPosition;
    private GameObject trailInstance;

    public void Spawn()
    {
        // Capture caster and target positions.
        startPosition = caster.position;
        endPosition = target.position;
        transform.position = startPosition;

        // Look up and instantiate the trail effect from the resourceManager.
        TrailResource trailRes;
        if (resourceManager.trailEffects.TryGetValue(trailKey, out trailRes))
        {
            // Instantiate the trail prefab as a child of this SpellInstance.
            trailInstance = Instantiate(trailRes.Prefab, transform.position, Quaternion.identity, transform);
            // Adjust its transform based on the resource.
            trailInstance.transform.localPosition = trailRes.RelativeOffset;
            trailInstance.transform.localEulerAngles = trailRes.AngularRotation;
            trailInstance.transform.localScale = trailRes.RelativeScale;
            // The prefab itself should handle delay, duration, and looping.
        }
        else
        {
            Debug.LogWarning("Trail resource not found for key: " + trailKey);
        }

        // Begin movement along the animation curve.
        //StartCoroutine(MoveAlongCurve());
        //StartCoroutine(MoveWithSpringEffect());
        //StartCoroutine(MoveWithBezierCurve());

        Vector3 launchDir = (target.position - caster.position).normalized + Vector3.up * 0.7f;
        float launchDist = 3.0f;
        StartCoroutine(MoveWithDirectedBezierCurve(launchDir, launchDist));
    }

    private IEnumerator MoveWithSpringEffect()
    {
        float elapsed = 0f;
        float duration = travelDuration;

        // Damping motion parameters (adjust as needed)
        float A = 1.3f;  // Overshoot intensity
        float b = 3.5f;  // Damping factor (higher = stops oscillating faster)
        float omega = 8f; // Frequency of oscillation

        Vector3 direction = (endPosition - startPosition);
        Vector3 initialOffset = startPosition;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Damped spring function with smooth approach
            float springFactor = A * (1 - Mathf.Exp(-b * t) * Mathf.Cos(omega * t));

            // Apply movement along the calculated direction
            transform.position = Vector3.Lerp(transform.position, initialOffset + direction * springFactor, 0.9f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final smoothing to ensure exact target position
        float smoothDuration = 0.1f;
        float smoothElapsed = 0f;
        while (smoothElapsed < smoothDuration)
        {
            transform.position = Vector3.Lerp(transform.position, endPosition, smoothElapsed / smoothDuration);
            smoothElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }

    private IEnumerator MoveWithBezierCurve()
    {
        float elapsed = 0f;
        float duration = travelDuration;

        // Calculate control point (this is past the target for the overshoot)
        Vector3 controlPoint = endPosition + (endPosition - startPosition) * 0.7f; // Adjust multiplier for more/less overshoot

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Quadratic Bezier Curve formula
            Vector3 pos = (1 - t) * (1 - t) * startPosition
                        + 2 * (1 - t) * t * controlPoint
                        + t * t * endPosition;

            transform.position = pos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is exact
        transform.position = endPosition;
    }

    private IEnumerator MoveWithDirectedBezierCurve(Vector3 launchDirection, float launchDistance)
    {
        float elapsed = 0f;
        float duration = travelDuration;

        // Calculate the launch point (in the desired direction)
        Vector3 launchPoint = startPosition + (launchDirection.normalized * launchDistance);

        // Calculate the control point that ensures a smooth curve to the target
        Vector3 controlPoint = (launchPoint + endPosition) * 0.5f + Vector3.up * 1.5f; // Adjust for curve height

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Quadratic Bezier Curve formula
            Vector3 pos = (1 - t) * (1 - t) * startPosition
                        + 2 * (1 - t) * t * controlPoint
                        + t * t * endPosition;

            transform.position = pos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is exact
        transform.position = endPosition;
    }


    private IEnumerator MoveAlongCurve()
    {
        float elapsed = 0f;
        while (elapsed < travelDuration)
        {
            float t = elapsed / travelDuration;

            // Interpolate between start and end positions.
            Vector3 pos = Vector3.Lerp(startPosition, endPosition, t);
            // Add an offset from the animation curve (for example, a vertical arc).
            float offset = pathCurve.Evaluate(t);
            pos.y += offset;

            transform.position = pos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap exactly to the target's position.
        transform.position = endPosition;

        // Remove the trail effect.
        if (trailInstance != null)
        {
            Destroy(trailInstance);
        }

        // Look up the arrival VFX resource from the resourceManager.
        VFXResource vfxRes;
        if (resourceManager.visualEffects.TryGetValue(vfxKey, out vfxRes))
        {
            // Spawn the VFX at the target's position.
            yield return vfxManager.Spawn(vfxRes, target.position);
        }
        else
        {
            Debug.LogWarning("VFX resource not found for key: " + vfxKey);
        }

        // Invoke the arrival callback to trigger healing or damage.
        onArrival?.Invoke();

        // Finally, remove this SpellInstance.
        spellManager.Despawn(name);
    }
}
