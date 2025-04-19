using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CanvasParticleEmitter : MonoBehaviour
{


    private RectTransform canvas2D; // SelectProfile the Canvas
    private GameObject canvasParticlePrefab;
    
    private float spawnIntervalMin; // Time between spawns
    private float spawnIntervalMax; // Time between spawns
    private float speedMin;
    private float speedMax;
    private float rotationSpeedMin; // Min rotation speed
    private float rotationSpeedMax; // Max rotation speed
    private float fallSpeedMin; // Minimum downward speed
    private float fallSpeedMax; // Maximum downward speed
    private float scaleMin; // Minimum scale
    private float scaleMax; // Maximum scale
    private int prewarmCount; // Index of particles to spawn on start
    private Sprite[] sprites; // Array of sprites from the sprite sheet
    private float xMin;
    private float xMax;
    private float yMin;
    private float yMax;

    private void Awake()
    {
        canvas2D = GameObject.Find("Canvas2D").GetComponent<RectTransform>();
        canvasParticlePrefab = PrefabRepo.instance.Prefabs["CanvasParticlePrefab"];


        xMin = -Screen.width;
        xMax = Screen.width;
        yMin = -200;
        yMax = 200;
        spawnIntervalMin = 0.1f;
        spawnIntervalMax = 0.25f;
        speedMin = 300;
        speedMax = 600;
        yMin = -1000;
        yMax = 1000;
        rotationSpeedMin = 70;
        rotationSpeedMax = 100;
        fallSpeedMin = 40;
        fallSpeedMax = 100;
        scaleMin = 0.3f;
        scaleMax = 0.4f;
        prewarmCount = 20;

        sprites = new Sprite[]
        {
            SpriteRepo.instance.Leaves["Leaf1"],
            SpriteRepo.instance.Leaves["Leaf2"],
            SpriteRepo.instance.Leaves["MapleLeaf1"],
            SpriteRepo.instance.Leaves["MapleLeaf2"],
        };

    }

    void Start()
    {
        PrewarmParticles();  // Assign initial particles
        StartCoroutine(SpawnImages());
    }

    private void PrewarmParticles()
    {
        for (int i = 0; i < prewarmCount; i++)
        {
            SpawnImage(preheat: true);
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

    private void SpawnImage(bool preheat = false)
    {
        GameObject newImage = Instantiate(canvasParticlePrefab, canvas2D);
        RectTransform rect = newImage.GetComponent<RectTransform>();
        Image image = newImage.GetComponent<Image>();
        if (rect == null || image == null)
            return;

        // SelectProfile a random sprite from the sprite sheet
        image.sprite = sprites.ShuffleFirst();

        // SelectProfile start position
        float startX = preheat ? Random.Float(xMin, xMax) : xMin; // Prewarm particles start mid-flight
        float startY = Random.Float(yMin, yMax);
        rect.anchoredPosition = new Vector2(startX, startY);

        // SelectProfile random rotation speed, movement, and scale
        float rotRange = Random.Float(rotationSpeedMin, rotationSpeedMax);
        float rotWildcard = Random.Int(1, 3) == 1 ? Random.Float(1, 3f) : 1f;
        float rotDirection = Random.Boolean ? -1f : 1f;

        float rotationSpeed = rotRange * rotWildcard * rotDirection;
        float horizontalSpeed = Random.Float(speedMin, speedMax);
        float fallSpeed = Random.Float(fallSpeedMin, fallSpeedMax);
        float scale = Random.Float(scaleMin, scaleMax);
        rect.localScale = new Vector3(scale, scale, 1f);

        CanvasParticleInstance instance = newImage.AddComponent<CanvasParticleInstance>();
        instance.parent = transform;
        instance.Initialize(rotationSpeed, horizontalSpeed, fallSpeed);
    }
}
