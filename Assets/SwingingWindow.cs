using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwingingWindow : MonoBehaviour
{
    private float minAngle;
    private float maxAngle;
    private float windSpeedMultiplier;
    private float accelerationFactor;
    private float noiseScale;
    private float maxAcceleration;
    private float minAcceleration;
    private float snapThreshold;
    private float variationMin;
    private float variationMax;
    private float windShiftChance;
    private float waitTimeMin;
    private float waitTimeMax;
    private float wiggleIntensity;
    private float wiggleFrequency;
    private float noiseOffset;
    private float currentYRotation;
    private float currentVelocity = 0f;
    private Queue<float> targetRotations = new Queue<float>();

    private void Awake()
    {
        minAngle = -90f;
        maxAngle = 90f;
        windSpeedMultiplier = 10f;
        accelerationFactor = 10f;
        noiseScale = 0.2f;
        maxAcceleration = 5f;
        minAcceleration = 1f;
        snapThreshold = 1f;
        variationMin = 5f;
        variationMax = 60f;
        windShiftChance = 0.1f;
        waitTimeMin = 0.05f;
        waitTimeMax = 0.5f;
        wiggleIntensity = 1f;
        wiggleFrequency = 10f;
        noiseOffset = Random.Float(0f, 100f);
        currentYRotation = 0f;
    }

    void Start()
    {
        GenerateRotationBuffer();
        StartCoroutine(SwingWindow());
    }

    private void GenerateRotationBuffer()
    {
        float initialRotation = 0f;
        float variation = Random.Float(variationMin, variationMax) * (Random.Float(0f, 1f) < 0.5f ? -1f : 1f);
        initialRotation = Mathf.Clamp(initialRotation + variation, minAngle, maxAngle);
        targetRotations.Enqueue(initialRotation);
    }

    private IEnumerator SwingWindow()
    {
        while (true)
        {
            if (targetRotations.Count == 0)
                GenerateRotationBuffer();

            float targetYRotation = targetRotations.Dequeue();

            while (Mathf.Abs(currentYRotation - targetYRotation) > snapThreshold)
            {
                float time = Time.time;
                float perlin = Mathf.PerlinNoise(noiseOffset, time * noiseScale) * 2 - 1;
                float windStrength = Mathf.Abs(perlin);
                float acceleration = Mathf.Lerp(minAcceleration, maxAcceleration, windStrength) * accelerationFactor;
                currentVelocity = Mathf.Lerp(currentVelocity, acceleration, Time.deltaTime * 3f);

                // Add wiggle effect to simulate air currents while moving
                float wiggle = Mathf.Sin(time * wiggleFrequency) * wiggleIntensity;
                float adjustedTarget = Mathf.Clamp(targetYRotation + wiggle, minAngle, maxAngle);

                currentYRotation = Mathf.Lerp(currentYRotation, adjustedTarget, Time.deltaTime * windSpeedMultiplier);
                currentYRotation = Mathf.Clamp(currentYRotation, minAngle, maxAngle);
                transform.rotation = Quaternion.Euler(0, currentYRotation, 9f);
                yield return null;
            }

            currentYRotation = targetYRotation;
            transform.rotation = Quaternion.Euler(0, currentYRotation, 9f);

            if (Random.Float(0f, 1f) < windShiftChance) // Configurable chance for sudden wind shift
                GenerateRotationBuffer();

            yield return new WaitForSeconds(Random.Float(waitTimeMin, waitTimeMax));
        }
    }
}
