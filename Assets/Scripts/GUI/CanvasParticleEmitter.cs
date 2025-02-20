using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasParticleEmitter : MonoBehaviour
{
    [SerializeField] private GameObject imagePrefab; // Prefab with an Image component
    [SerializeField] private RectTransform canvasTransform; // Assign the Canvas
    [SerializeField] private float spawnIntervalMin; // Time between spawns
    [SerializeField] private float spawnIntervalMax; // Time between spawns
    [SerializeField] private float speedMin;
    [SerializeField] private float speedMax;
    [SerializeField] private float x;
    [SerializeField] private float yMin;
    [SerializeField] private float yMax;
    [SerializeField] private float rotationSpeedMin; // Min rotation speed
    [SerializeField] private float rotationSpeedMax; // Max rotation speed
    [SerializeField] private float fallSpeedMin; // Minimum downward speed
    [SerializeField] private float fallSpeedMax; // Maximum downward speed
    [SerializeField] private float scaleMin; // Minimum scale
    [SerializeField] private float scaleMax; // Maximum scale
    [SerializeField] private int prewarmCount; // Number of particles to spawn on start
    [SerializeField] private Sprite[] sprites; // Array of sprites from the sprite sheet
   

    void Start()
    {
        PrewarmParticles();  // Spawn initial particles
        StartCoroutine(SpawnImages());
    }

    private void PrewarmParticles()
    {
        for (int i = 0; i < prewarmCount; i++)
        {
            SpawnImage(prewarm: true);
        }
    }

    private IEnumerator SpawnImages()
    {
        while (true)
        {
            SpawnImage();
            var spawnInterval = Random.Float(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnImage(bool prewarm = false)
    {
        GameObject newImage = Instantiate(imagePrefab, canvasTransform);
        RectTransform imageRect = newImage.GetComponent<RectTransform>();
        Image imageComponent = newImage.GetComponent<Image>();
        if (imageRect == null || imageComponent == null) return;

        // Assign a random sprite from the sprite sheet
        if (sprites.Length > 0)
        {
            imageComponent.sprite = sprites[Random.Int(0, sprites.Length - 1)];
        }

        // Assign start position
        float startX = prewarm ? Random.Float(-50, Screen.width) : -50; // Prewarm particles start mid-flight
        float startY = Random.Float(yMin, yMax);
        imageRect.anchoredPosition = new Vector2(startX, startY);

        // Assign random rotation speed, movement, and scale
        float rotationSpeed = Random.Float(rotationSpeedMin, rotationSpeedMax);
        float horizontalSpeed = Random.Float(speedMin, speedMax);
        float fallSpeed = Random.Float(fallSpeedMin, fallSpeedMax);
        float scale = Random.Float(scaleMin, scaleMax);
        imageRect.localScale = new Vector3(scale, scale, 1f);

        CanvasParticleInstance instance = newImage.AddComponent<CanvasParticleInstance>();
        instance.Initialize(rotationSpeed, horizontalSpeed, fallSpeed);
    }
}
