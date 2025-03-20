//using UnityEngine;

//namespace Assets.Scripts.Models
//{
//    using UnityEngine;

//    [RequireComponent(typeof(SpriteRenderer))]
//    public class SpritePan : MonoBehaviour
//    {
//        // Cropping parameters used for panning.
//        // These will be set based on the ThumbnailSettings.
//        [SerializeField] private float rangeX = 44f;  // Extra width available for panning in X.
//        [SerializeField] private float sizeX = 256f;    // Perfect frame width (from ThumbnailSettings).
//        [SerializeField] private float rangeY = 44f;    // Extra height available for panning in Y.
//        [SerializeField] private float sizeY = 256f;    // Perfect frame height (from ThumbnailSettings).

//        public float RangeX { get => rangeX; set => rangeX = value; }
//        public float SizeX { get => sizeX; set => sizeX = value; }
//        public float RangeY { get => rangeY; set => rangeY = value; }
//        public float SizeY { get => sizeY; set => sizeY = value; }

//        // Pan speed property (controls how fast the panning occurs).
//        [SerializeField] private float panSpeed = 1f;
//        public float PanSpeed { get => panSpeed; set => panSpeed = value; }

//        // Wobble amplitude factors (as fractions of the maximum offset).
//        [SerializeField] private float wobbleAmplitudeFactorX = 0.5f;
//        [SerializeField] private float wobbleAmplitudeFactorY = 0.5f;
//        public float WobbleAmplitudeFactorX { get => wobbleAmplitudeFactorX; set => wobbleAmplitudeFactorX = value; }
//        public float WobbleAmplitudeFactorY { get => wobbleAmplitudeFactorY; set => wobbleAmplitudeFactorY = value; }

//        // Private random seeds for Perlin noise (unique per instance).
//        private float noiseSeedX;
//        private float noiseSeedY;

//        // Variables for a natural pause cycle.
//        [SerializeField] private float nextPauseInterval = 5f; // e.g., move for ~5 sec before pausing.
//        [SerializeField] private float pauseDuration = 2f;       // e.g., pause fully for ~2 sec.
//        [SerializeField] private float pauseRampDuration = 0.5f;   // time to ease down/up.
//        private float effectiveNoiseTime = 0f;
//        private float cycleTime = 0f;
//        private float cyclePeriod;

//        // Slow overall speed factor (400% slower means dividing panSpeed by 4).
//        private float slowSpeed;

//        private Material materialInstance;

//        private void Awake()
//        {
//            materialInstance = GetComponent<SpriteRenderer>().material;

//            // Generate random seeds for unique movement.
//            noiseSeedX = Random.Range(0f, 100f);
//            noiseSeedY = Random.Range(0f, 100f);

//            // You can randomize the pause timing if desired.
//            nextPauseInterval = Random.Range(3f, 10f);
//            pauseDuration = Random.Range(1f, 3f);
//            cyclePeriod = nextPauseInterval + pauseDuration + 2f * pauseRampDuration;

//            slowSpeed = panSpeed / 4f;
//            effectiveNoiseTime = 0f;
//            cycleTime = 0f;
//        }

//        /// <summary>
//        /// Use this method to feed in the perfect framing from ThumbnailSettings.
//        /// extraRangeX/Y define how many extra pixels beyond the perfect frame are available for panning.
//        /// </summary>
//        public void SetThumbnailSettings(ThumbnailSettings settings, float extraRangeX, float extraRangeY)
//        {
//            // Set the perfect frame dimensions.
//            sizeX = settings.Width;
//            sizeY = settings.Height;
//            // Set the extra panning range.
//            rangeX = extraRangeX;
//            rangeY = extraRangeY;
//        }

//        private void Update()
//        {
//            // Calculate full dimensions of the cropping region.
//            float fullWidth = sizeX + rangeX;
//            float fullHeight = sizeY + rangeY;

//            // Maximum normalized offset (in UV space).
//            float maxOffsetX = rangeX / fullWidth;
//            float maxOffsetY = rangeY / fullHeight;

//            // Update cycle timer.
//            cycleTime += Time.deltaTime;
//            if (cycleTime >= cyclePeriod)
//                cycleTime -= cyclePeriod;

//            // Determine a speed multiplier based on the cycle.
//            float multiplier = 1f;
//            if (cycleTime < (nextPauseInterval - pauseRampDuration))
//            {
//                multiplier = 1f;
//            }
//            else if (cycleTime < nextPauseInterval)
//            {
//                float t = (cycleTime - (nextPauseInterval - pauseRampDuration)) / pauseRampDuration;
//                multiplier = Mathf.Lerp(1f, 0f, t);
//            }
//            else if (cycleTime < (nextPauseInterval + pauseDuration))
//            {
//                multiplier = 0f;
//            }
//            else if (cycleTime < (nextPauseInterval + pauseDuration + pauseRampDuration))
//            {
//                float t = (cycleTime - (nextPauseInterval + pauseDuration)) / pauseRampDuration;
//                multiplier = Mathf.Lerp(0f, 1f, t);
//            }
//            else
//            {
//                multiplier = 1f;
//            }

//            // Advance our effective noise time only when not paused.
//            effectiveNoiseTime += Time.deltaTime * multiplier * slowSpeed;

//            // Generate Perlin noise values for natural movement.
//            float noiseX = Mathf.PerlinNoise(effectiveNoiseTime, noiseSeedX);
//            float noiseY = Mathf.PerlinNoise(effectiveNoiseTime, noiseSeedY);

//            // Center the noise (-0.5 to +0.5).
//            float centeredNoiseX = noiseX - 0.5f;
//            float centeredNoiseY = noiseY - 0.5f;

//            // Calculate the wobble offset (reduce overall movement by half).
//            float wobbleX = centeredNoiseX * maxOffsetX * wobbleAmplitudeFactorX * 0.5f;
//            float wobbleY = centeredNoiseY * maxOffsetY * wobbleAmplitudeFactorY * 0.5f;

//            // Base offset to center the cropping area.
//            float baseOffsetX = maxOffsetX * 0.5f;
//            float baseOffsetY = maxOffsetY * 0.5f;

//            // Final UV offset.
//            float offsetX = baseOffsetX + wobbleX;
//            float offsetY = baseOffsetY + wobbleY;
//            offsetX = Mathf.Clamp(offsetX, 0, maxOffsetX);
//            offsetY = Mathf.Clamp(offsetY, 0, maxOffsetY);

//            Vector4 newOffset = new Vector4(offsetX, offsetY, 0, 0);
//            materialInstance.SetVector("_MainTexOffset", newOffset);
//        }
//    }


//}
