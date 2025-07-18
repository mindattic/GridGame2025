
using Assets.Scripts.Models;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ActorThumbnail : MonoBehaviour
{
    private ActorInstance instance;
    public ThumbnailSettings settings;
    private SpriteRenderer spriteRenderer;

    public float rangeMultiplier;
    public Vector2 range;
    public float panFocus;
    public float wobbleAmplitudeFactorX;
    public float wobbleAmplitudeFactorY;
    public float nextPauseInterval;
    public float pauseDuration;
    public float pauseRampDuration;
    private float effectiveNoiseTime;
    private float cycleTime;
    private float cyclePeriod;
    private Vector2 noiseSeed;
  
    //Properties
    public Texture2D texture => spriteRenderer.sprite.texture;


    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        noiseSeed = new Vector2(Random.Float(0f, 100f), Random.Float(0f, 100f));

        // Scale multiplier proportionally
        float baseTextureSize = 4096f;
        float textureSize = Mathf.Max(texture.width, texture.height);
        rangeMultiplier = 0.05f * (textureSize / baseTextureSize);


        panFocus = 0.25f;
    
        effectiveNoiseTime = 0f;
        cycleTime = 0f;

        nextPauseInterval = Random.Float(3f, 7f);
        pauseDuration = Random.Float(2f, 5f);
        pauseRampDuration = Random.Float(0.25f, 0.75f);
        cyclePeriod = nextPauseInterval + pauseDuration + 2f * pauseRampDuration;


        range = new Vector2(0.1f, 0.1f);
        wobbleAmplitudeFactorX = 0.2f;
        wobbleAmplitudeFactorY = 0.2f;


    }

    public void Set(Vector3 position, Vector3 scale)
    {
        settings = new ThumbnailSettings(position, scale);
        transform.localPosition = settings.Position;
        transform.localScale = settings.Scale;
    }

    public void Initialize(ActorInstance parentInstance)
    {
        instance = parentInstance;

        var actorData = ActorRepo.Actors[instance.characterName];

        spriteRenderer.sprite = actorData.Portrait;
        spriteRenderer.material.SetTexture("_MainTex", spriteRenderer.sprite.texture);

        settings = new ThumbnailSettings(actorData.ThumbnailSettings);
        transform.localPosition = settings.Position;
        transform.localScale = settings.Scale;

        // Dynamic range based on texture size
        float textureSize = Mathf.Max(texture.width, texture.height);
        rangeMultiplier = 0.05f * (textureSize / Constants.PortraitSize);

        range = new Vector2(0.1f, 0.1f);
        wobbleAmplitudeFactorX = 0.25f;
        wobbleAmplitudeFactorY = 0.25f;
    }

    private void Update()
    {
        float fullWidth = Mathf.Max(settings.Scale.x * range.x, 0.0001f);
        float fullHeight = Mathf.Max(settings.Scale.y * range.y, 0.0001f);

        Vector2 maxOffset = new Vector2(range.x / fullWidth, range.y / fullHeight);

        // Update the cycle timer
        cycleTime += Time.deltaTime;
        if (cycleTime >= cyclePeriod)
        {
            cycleTime -= cyclePeriod;

            nextPauseInterval = Random.Float(3f, 7f);
            pauseDuration = Random.Float(2f, 5f);
            pauseRampDuration = Random.Float(0.25f, 0.75f);
            cyclePeriod = nextPauseInterval + pauseDuration + 2f * pauseRampDuration;

        }


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
        effectiveNoiseTime += Time.deltaTime * multiplier * panFocus;

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
        float offsetX = baseOffsetX + wobbleX;
        float offsetY = baseOffsetY + wobbleY;
        spriteRenderer.material.SetVector("_MainTexOffset", new Vector4(offsetX, offsetY, 0, 0));
    }


}

