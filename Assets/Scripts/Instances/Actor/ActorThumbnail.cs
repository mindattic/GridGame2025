using Assets.Scripts.Models; // for ThumbnailSettings, etc.
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ActorThumbnail : MonoBehaviour
{
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;

    // Reference to the actor this thumbnail represents.
    private ActorInstance instance;

    // The full texture and generated sprite.
    public Texture2D texture;
    public Sprite sprite;

    // Thumbnail settings will define the "perfect framing".
    private ThumbnailSettings thumbnailSettings;

    // Reference to the SpriteRenderer on this object.
    private SpriteRenderer spriteRenderer;

    // Internal panning values derived from extraRange.
    //public float range.x = 44f;
    //public float range.y = 44f;
    public float rangeMultiplier;
    public Vector2 range;

    // Pan speed and wobble factors.
    public float panSpeed;
    public float wobbleAmplitudeFactorX;
    public float wobbleAmplitudeFactorY;

    // Pause cycle variables.
    public float nextPauseInterval;
    public float pauseDuration;
    public float pauseRampDuration;
    private float effectiveNoiseTime;
    private float cycleTime;
    private float cyclePeriod;

    // We'll slow the overall movement by a factor (400% slower = divide by 4).
    private float slowSpeed;

    // Random seeds for Perlin noise so each instance moves uniquely.

    private Vector2 noiseSeed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        noiseSeed = new Vector2(Random.Float(0f, 100f), Random.Float(0f, 100f));
        rangeMultiplier = 0.05f;
        panSpeed = 1f;
        wobbleAmplitudeFactorX = 0.5f;
        wobbleAmplitudeFactorY = 0.5f;
        nextPauseInterval = Random.Float(3f, 7f);
        pauseDuration = Random.Float(2f, 5f);
        cyclePeriod = nextPauseInterval + pauseDuration + 2f * pauseRampDuration;
        slowSpeed = panSpeed / 4f;
        nextPauseInterval = 5f;                 // How long to move before pausing.
        pauseDuration = 2f;                     // How long to hold a pause.
        pauseRampDuration = 0.5f;               // Easing time for ramping down/up.
        effectiveNoiseTime = 0f;
        cycleTime = 0f;
    }

    /// <summary>
    /// Call this method to supply the ActorInstance.
    /// </summary>
    public void Initialize(ActorInstance parentInstance)
    {
        instance = parentInstance;
        Generate();
    }

    /// <summary>
    /// Generates the thumbnail sprite using ThumbnailSettings.
    /// The perfect frame is defined by a top‑left coordinate (X, Y) and Width/Height.
    /// Extra panning range is provided via extrarange.x/extrarange.y. Since Sprite.Create
    /// requires a bottom‑left origin, we convert the perfect frame's top‑left to bottom‑left.
    /// </summary>
    public void Generate(ThumbnailSettings other = null)
    {
        //Retrieve applicable settings
        rangeMultiplier = ProfileStore.instance.CurrentProfile.Settings.ActorPanMultiplier;


        // GetProfile the full texture from the resource manager.
        texture = GameManager.instance.resourceManager.Portrait(instance.character.ToString()).Value;

        // Retrieve the thumbnail settings.
        if (other == null)
            thumbnailSettings = ActorStore.instance.GetThumbnailSetting(instance.character);
        else
            thumbnailSettings = new ThumbnailSettings(other);

        range = new Vector2(
            thumbnailSettings.Width * rangeMultiplier,
            thumbnailSettings.Height * rangeMultiplier);

        // In ThumbnailSettings, X and Y represent the top‑left coordinate of the perfect frame.
        // Convert to bottom‑left coordinate for Sprite.Create:

        var topLeft = new Vector2Int(thumbnailSettings.X, thumbnailSettings.Y);


        int perfectBLX = topLeft.x;
        int perfectBLY = texture.height - topLeft.y - thumbnailSettings.Height;

        // Calculate the desired extended cropping rectangle.
        int desiredWidth = thumbnailSettings.Width + (int)range.x;
        int desiredHeight = thumbnailSettings.Height + (int)range.y;

        // Ensure the desired dimensions do not exceed the texture size.
        int rectWidth = Mathf.Min(desiredWidth, texture.width);
        int rectHeight = Mathf.Min(desiredHeight, texture.height);

        // We want the perfect frame to remain centered in the extended region.
        // Subtract half of the extra range from the perfect frame's bottom‑left coordinate.
        int rectX = perfectBLX - (int)(range.x / 2);
        int rectY = perfectBLY - (int)(range.y / 2);

        // Clamp the cropping rectangle so it remains within the texture bounds.
        rectX = Mathf.Clamp(rectX, 0, texture.width - rectWidth);
        rectY = Mathf.Clamp(rectY, 0, texture.height - rectHeight);

        Rect rect = new Rect(rectX, rectY, rectWidth, rectHeight);

        // Create a sprite from the extended cropped region.
        // The pivot is set to (0.5, 0.5) so that panning (via UV offsets) remains centered.
        sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f);
        spriteRenderer.sprite = sprite;
    }


    private void Update()
    {
        // --- Panning Code ---
        // Calculate the full dimensions of the cropping region.
        float fullWidth = thumbnailSettings.Width + range.x;
        float fullHeight = thumbnailSettings.Height + range.y;

        // Maximum normalized offset (in UV space).
        var maxOffset = new Vector2(range.x / fullWidth, range.y / fullHeight);

        // Update our cycle timer.
        cycleTime += Time.deltaTime;
        if (cycleTime >= cyclePeriod)
            cycleTime -= cyclePeriod;

        // Determine a speed multiplier based on our pause cycle.
        float multiplier = 1f;
        if (cycleTime < (nextPauseInterval - pauseRampDuration))
        {
            multiplier = 1f;
        }
        else if (cycleTime < nextPauseInterval)
        {
            float t = (cycleTime - (nextPauseInterval - pauseRampDuration)) / pauseRampDuration;
            multiplier = Mathf.Lerp(1f, 0f, t);
        }
        else if (cycleTime < (nextPauseInterval + pauseDuration))
        {
            multiplier = 0f;
        }
        else if (cycleTime < (nextPauseInterval + pauseDuration + pauseRampDuration))
        {
            float t = (cycleTime - (nextPauseInterval + pauseDuration)) / pauseRampDuration;
            multiplier = Mathf.Lerp(0f, 1f, t);
        }
        else
        {
            multiplier = 1f;
        }

        // Advance effective noise time based on multiplier and slow speed.
        effectiveNoiseTime += Time.deltaTime * multiplier * slowSpeed;

        // Generate smooth Perlin noise values.
        float noiseX = Mathf.PerlinNoise(effectiveNoiseTime, noiseSeed.x);
        float noiseY = Mathf.PerlinNoise(effectiveNoiseTime, noiseSeed.y);

        // Center the noise around 0.
        float centeredNoiseX = noiseX - 0.5f;
        float centeredNoiseY = noiseY - 0.5f;

        // Calculate wobble offsets. (Multiplying by 0.5 to reduce the movement amplitude.)
        float wobbleX = centeredNoiseX * maxOffset.x * wobbleAmplitudeFactorX * 0.5f;
        float wobbleY = centeredNoiseY * maxOffset.y * wobbleAmplitudeFactorY * 0.5f;

        // Base offset centers the cropping window.
        float baseOffsetX = maxOffset.x * 0.5f;
        float baseOffsetY = maxOffset.y * 0.5f;

        // Final computed UV offset.
        float offsetX = baseOffsetX + wobbleX;
        float offsetY = baseOffsetY + wobbleY;
        offsetX = Mathf.Clamp(offsetX, 0, maxOffset.x);
        offsetY = Mathf.Clamp(offsetY, 0, maxOffset.y);
        Vector4 newOffset = new Vector4(offsetX, offsetY, 0, 0);

        // Update the shader's custom offset property.
        // Make sure your material uses a shader (like Custom/SpriteOffset) that supports _MainTexOffset.
        spriteRenderer.material.SetVector("_MainTexOffset", newOffset);
    }
}
