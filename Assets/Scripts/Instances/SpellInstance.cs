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
    public float travelDuration = 1.0f;

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
        StartCoroutine(MoveAlongCurve());
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
