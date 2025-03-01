using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BackgroundInstance : MonoBehaviour
{
    //Quick Reference Properties
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;

    //Fields
    private bool isMoving;
    private Vector3 initialPosition;
    private SpriteRenderer spriteRenderer;
    private Vector2 padding;
    private Vector3 scale;
    private Vector2 amplitude;
    private Vector2 speed;
    private float time;
    private Vector3 targetPosition;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position; //Store the starting position

        //Get screen dimensions in world units
        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Camera.main.aspect;

        //Get the sprite's size in world units
        spriteRenderer.sprite = resourceManager.Load<Sprite>("Backgrounds/CandleLitPath");
        Bounds spriteBounds = spriteRenderer.sprite.bounds;
        Vector2 spriteSize = spriteBounds.size;

        //Calculate scale factors
        padding = new Vector2(screenWidth * 0.01f, screenHeight * 0.01f);
        scale = new Vector3(screenWidth / spriteSize.x + padding.x, screenHeight / spriteSize.y + padding.y, 1);
        transform.localScale = scale;

        amplitude = new Vector2(padding.x, padding.y);
        speed = new Vector2(0.2f, 0.2f);

        //StartCoroutine(MoveToRandomPositions());

    }

    private void FixedUpdate()
    {
        time += Time.deltaTime;

        //Calculate the new position using a sine wave
        var x = initialPosition.x + Mathf.Sin(time * speed.x) * amplitude.x;
        var y = initialPosition.y + Mathf.Sin(time * speed.y) * amplitude.y;
        transform.position = new Vector3(x, y, initialPosition.z);
    }

}