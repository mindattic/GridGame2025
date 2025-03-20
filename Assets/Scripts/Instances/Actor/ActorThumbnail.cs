using UnityEngine;
using Assets.Scripts.Models; // for ThumbnailSettings, etc.

[RequireComponent(typeof(SpriteRenderer))]
public class ActorThumbnail : MonoBehaviour
{
    // Reference to the actor this thumbnail represents.
    private ActorInstance instance;

    // The full texture and generated sprite.
    public Texture2D texture;
    public Sprite sprite;

    // Thumbnail settings will define the "perfect framing".
    private ThumbnailSettings thumbnailSettings;

    // Reference to the SpriteRenderer on this object.
    private SpriteRenderer spriteRenderer;

    // Cropping parameters for panning.
    // "Perfect" frame dimensions come from ThumbnailSettings.
    private float sizeX = 256f;
    private float sizeY = 256f;
    // Extra panning range (in pixels) beyond the perfect frame.
    [SerializeField] private float extraRangeX = 44f;
    [SerializeField] private float extraRangeY = 44f;
    // Internal panning values derived from extraRange.
    private float rangeX = 44f;
    private float rangeY = 44f;

    // Pan speed and wobble factors.
    [SerializeField] private float panSpeed = 1f;
    [SerializeField] private float wobbleAmplitudeFactorX = 0.5f;
    [SerializeField] private float wobbleAmplitudeFactorY = 0.5f;

    // Pause cycle variables.
    [SerializeField] private float nextPauseInterval = 5f; // How long to move before pausing.
    [SerializeField] private float pauseDuration = 2f;       // How long to hold a pause.
    [SerializeField] private float pauseRampDuration = 0.5f;   // Easing time for ramping down/up.
    private float effectiveNoiseTime = 0f;
    private float cycleTime = 0f;
    private float cyclePeriod;

    // We'll slow the overall movement by a factor (400% slower = divide by 4).
    private float slowSpeed;

    // Random seeds for Perlin noise so each instance moves uniquely.
    private float noiseSeedX;
    private float noiseSeedY;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Generate random seeds.
        noiseSeedX = Random.Float(0f, 100f);
        noiseSeedY = Random.Float(0f, 100f);
        // Optionally randomize pause intervals.
        nextPauseInterval = Random.Float(3f, 10f);
        pauseDuration = Random.Float(1f, 3f);
        cyclePeriod = nextPauseInterval + pauseDuration + 2f * pauseRampDuration;
        slowSpeed = panSpeed / 4f;
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
    /// The perfect frame dimensions (sizeX/sizeY) are taken from the settings,
    /// and extra panning range is provided via extraRangeX/extraRangeY.
    /// </summary>
    public void Generate(ThumbnailSettings other = null)
    {
        // Get the full texture from the resource manager.
        texture = GameManager.instance.resourceManager.Portrait(instance.character.ToString()).Value;

        // Retrieve the thumbnail settings.
        if (other == null)
            thumbnailSettings = ActorStore.instance.GetThumbnailSetting(instance.character);
        else
            thumbnailSettings = new ThumbnailSettings(other);

        // Use the perfect framing dimensions from the settings.
        sizeX = thumbnailSettings.Width;
        sizeY = thumbnailSettings.Height;

        // Set the extra panning range from our extraRange variables.
        rangeX = extraRangeX;
        rangeY = extraRangeY;

        // Calculate the centered offset for the perfect frame.
        // Start by centering the perfect frame within the texture,
        // then apply the user-defined shift.
        int perfectOffsetX = (texture.width - thumbnailSettings.Width) / 2 + thumbnailSettings.OffsetX;
        int perfectOffsetY = (texture.height - thumbnailSettings.Height) / 2 + thumbnailSettings.OffsetY;

        // Now, extend the cropping rectangle to include extra pixels on all sides.
        // Subtract half of the extra range from the perfect offset,
        // and increase the width and height by the full extra range.
        int rectX = perfectOffsetX - (int)(rangeX / 2);
        int rectY = perfectOffsetY - (int)(rangeY / 2);
        int rectWidth = thumbnailSettings.Width + (int)rangeX;
        int rectHeight = thumbnailSettings.Height + (int)rangeY;

        // Clamp the cropping rect so it stays within the texture bounds.
        rectX = Mathf.Clamp(rectX, 0, texture.width - rectWidth);
        rectY = Mathf.Clamp(rectY, 0, texture.height - rectHeight);

        Rect rect = new Rect(rectX, rectY, rectWidth, rectHeight);

        // Create a sprite from the extended cropped region.
        sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f);
        // Assign the sprite to the SpriteRenderer.
        spriteRenderer.sprite = sprite;
    }


    private void Update()
    {
        // --- Panning Code ---
        // Calculate the full dimensions of the cropping region.
        float fullWidth = sizeX + rangeX;
        float fullHeight = sizeY + rangeY;

        // Maximum normalized offset (in UV space).
        float maxOffsetX = rangeX / fullWidth;
        float maxOffsetY = rangeY / fullHeight;

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
        float noiseX = Mathf.PerlinNoise(effectiveNoiseTime, noiseSeedX);
        float noiseY = Mathf.PerlinNoise(effectiveNoiseTime, noiseSeedY);

        // Center the noise around 0.
        float centeredNoiseX = noiseX - 0.5f;
        float centeredNoiseY = noiseY - 0.5f;

        // Calculate wobble offsets. (Multiplying by 0.5 to reduce the movement amplitude.)
        float wobbleX = centeredNoiseX * maxOffsetX * wobbleAmplitudeFactorX * 0.5f;
        float wobbleY = centeredNoiseY * maxOffsetY * wobbleAmplitudeFactorY * 0.5f;

        // Base offset centers the cropping window.
        float baseOffsetX = maxOffsetX * 0.5f;
        float baseOffsetY = maxOffsetY * 0.5f;

        // Final computed UV offset.
        float offsetX = baseOffsetX + wobbleX;
        float offsetY = baseOffsetY + wobbleY;
        offsetX = Mathf.Clamp(offsetX, 0, maxOffsetX);
        offsetY = Mathf.Clamp(offsetY, 0, maxOffsetY);

        Vector4 newOffset = new Vector4(offsetX, offsetY, 0, 0);

        // Update the shader's custom offset property.
        // Make sure your material uses a shader (like Custom/SpriteOffset) that supports _MainTexOffset.
        spriteRenderer.material.SetVector("_MainTexOffset", newOffset);
    }
}
