
using Assets.Scripts.Models;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ActorThumbnail : MonoBehaviour
{
    private ActorInstance instance;
    private ThumbnailSettings thumbnailSettings;
    private SpriteRenderer spriteRenderer;

    public float rangeMultiplier;
    public Vector2 range;
    public float panSpeed;
    public float wobbleAmplitudeFactorX;
    public float wobbleAmplitudeFactorY;
    public float nextPauseInterval;
    public float pauseDuration;
    public float pauseRampDuration;
    private float effectiveNoiseTime;
    private float cycleTime;
    private float cyclePeriod;
    private Vector2 noiseSeed;
    private float slowSpeed;

    //Properties
    public Texture2D texture => spriteRenderer.sprite.texture;


    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        noiseSeed = new Vector2(Random.Float(0f, 100f), Random.Float(0f, 100f));

        // Scale multiplier proportionally
        float baseTextureSize = 1024f;
        float textureSize = Mathf.Max(texture.width, texture.height);
        rangeMultiplier = 0.05f * (textureSize / baseTextureSize);
        range = new Vector2(1, 1);

        panSpeed = 1f;
        wobbleAmplitudeFactorX = 0.7f;
        wobbleAmplitudeFactorY = 0.7f;
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

    public void Set(Vector3 position, Vector3 scale)
    {
        thumbnailSettings = new ThumbnailSettings(position, scale);
        transform.localScale = thumbnailSettings.Scale;
        transform.localPosition = thumbnailSettings.Position;
    }

    public void Initialize(ActorInstance parentInstance)
    {
        instance = parentInstance;

        var actorData = ActorRepo.instance.Actors[instance.characterName];

        spriteRenderer.sprite = actorData.Portrait;
        spriteRenderer.material.SetTexture("_MainTex", spriteRenderer.sprite.texture);

        thumbnailSettings = actorData.ThumbnailSettings;
        transform.localScale = thumbnailSettings.Scale;
        transform.localPosition = thumbnailSettings.Position;

        // Dynamic range based on texture size
        float baseTextureSize = 256f;  // Use your original target baseline
        float textureSize = Mathf.Max(texture.width, texture.height);
        rangeMultiplier = 0.05f * (textureSize / baseTextureSize);

        range = new Vector2(
            rangeMultiplier * texture.width,
            rangeMultiplier * texture.height
        );
    }

    private void Update()
    {
        float fullWidth = Mathf.Max(thumbnailSettings.Scale.x * range.x, 0.0001f);
        float fullHeight = Mathf.Max(thumbnailSettings.Scale.y * range.y, 0.0001f);

        Vector2 maxOffset = new Vector2(range.x / fullWidth, range.y / fullHeight);

        // Update the cycle timer
        cycleTime += Time.deltaTime;
        if (cycleTime >= cyclePeriod)
            cycleTime -= cyclePeriod;

        // Determine speed multiplier based on pause cycle
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

        // Advance effective noise time
        effectiveNoiseTime += Time.deltaTime * multiplier * slowSpeed;

        // Generate Perlin noise values
        float noiseX = Mathf.PerlinNoise(effectiveNoiseTime, noiseSeed.x);
        float noiseY = Mathf.PerlinNoise(effectiveNoiseTime, noiseSeed.y);

        // Center noise and calculate UV offset
        float centeredNoiseX = noiseX - 0.5f;
        float centeredNoiseY = noiseY - 0.5f;
        float wobbleX = centeredNoiseX * maxOffset.x * wobbleAmplitudeFactorX * 0.5f;
        float wobbleY = centeredNoiseY * maxOffset.y * wobbleAmplitudeFactorY * 0.5f;
        float baseOffsetX = maxOffset.x * 0.5f;
        float baseOffsetY = maxOffset.y * 0.5f;
        float offsetX = Mathf.Clamp(baseOffsetX + wobbleX, 0, maxOffset.x);
        float offsetY = Mathf.Clamp(baseOffsetY + wobbleY, 0, maxOffset.y);

        // Update shader offset property
        spriteRenderer.material.SetVector("_MainTexOffset", new Vector4(offsetX, offsetY, 0, 0));
    }


}

