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
    [SerializeField] private float lifetime; // Time before the image is destroyed
    [SerializeField] private float rotationSpeedMin; // Min rotation speed
    [SerializeField] private float rotationSpeedMax; // Max rotation speed
    [SerializeField] private float fallSpeedMin; // Minimum downward speed
    [SerializeField] private float fallSpeedMax; // Maximum downward speed
    [SerializeField] private float scaleMin; // Minimum scale
    [SerializeField] private float scaleMax; // Maximum scale
    [SerializeField] private Sprite[] spriteSheet; // Array of sprites from the sprite sheet

    void Start()
    {
        StartCoroutine(SpawnImages());
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

    private void SpawnImage()
    {
        GameObject newImage = Instantiate(imagePrefab, canvasTransform);
        RectTransform imageRect = newImage.GetComponent<RectTransform>();
        Image imageComponent = newImage.GetComponent<Image>();
        if (imageRect == null || imageComponent == null) return;

        // Assign a random sprite from the sprite sheet
        if (spriteSheet.Length > 0)
        {
            imageComponent.sprite = spriteSheet[Random.Int(0, spriteSheet.Length - 1)];
        }

        // Set random start position (off-screen left)
        float startX = -50; //-canvasTransform.rect.width / 2 - 50
        float startY = Random.Float(yMin, yMax);
        imageRect.anchoredPosition = new Vector2(startX, startY);

        // Assign random rotation speed, movement, and scale
        float rotationSpeed = Random.Float(rotationSpeedMin, rotationSpeedMax);
        float horizontalSpeed = Random.Float(speedMin, speedMax);
        float fallSpeed = Random.Float(fallSpeedMin, fallSpeedMax);
        float scale = Random.Float(scaleMin, scaleMax);
        imageRect.localScale = new Vector3(scale, scale, 1f);

        CanvasParticleInstance instance = newImage.AddComponent<CanvasParticleInstance>();
        instance.Initialize(rotationSpeed, horizontalSpeed, fallSpeed, lifetime);
    }
}
