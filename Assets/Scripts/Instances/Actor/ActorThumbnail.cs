using UnityEngine;
using Assets.Scripts.Models; // for ThumbnailSettings, etc.

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
    public float rangeX = 44f;
    public float rangeY = 44f;

    // Pan speed and wobble factors.
    public float panSpeed = 1f;
    public float wobbleAmplitudeFactorX = 0.5f;
    public float wobbleAmplitudeFactorY = 0.5f;

    // Pause cycle variables.
    public float nextPauseInterval = 5f; // How long to move before pausing.
    public float pauseDuration = 2f;       // How long to hold a pause.
    public float pauseRampDuration = 0.5f;   // Easing time for ramping down/up.
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
    /// The perfect frame is defined by a top‑left coordinate (X, Y) and Width/Height.
    /// Extra panning range is provided via extraRangeX/extraRangeY. Since Sprite.Create
    /// requires a bottom‑left origin, we convert the perfect frame's top‑left to bottom‑left.
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

        // In ThumbnailSettings, X and Y represent the top‑left coordinate of the perfect frame.
        // Convert to bottom‑left coordinate for Sprite.Create:
        int perfectTopLeftX = thumbnailSettings.X;
        int perfectTopLeftY = thumbnailSettings.Y;
        int perfectBLX = perfectTopLeftX;
        int perfectBLY = texture.height - perfectTopLeftY - thumbnailSettings.Height;

        // Calculate the desired extended cropping rectangle.
        int desiredWidth = thumbnailSettings.Width + (int)rangeX;
        int desiredHeight = thumbnailSettings.Height + (int)rangeY;
        // Ensure the desired dimensions do not exceed the texture size.
        int rectWidth = Mathf.Min(desiredWidth, texture.width);
        int rectHeight = Mathf.Min(desiredHeight, texture.height);

        // We want the perfect frame to remain centered in the extended region.
        // Subtract half of the extra range from the perfect frame's bottom‑left coordinate.
        int rectX = perfectBLX - (int)(rangeX / 2);
        int rectY = perfectBLY - (int)(rangeY / 2);

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
        float fullWidth = thumbnailSettings.Width + rangeX;
        float fullHeight = thumbnailSettings.Height + rangeY;

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
