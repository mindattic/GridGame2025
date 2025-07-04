using System.Collections;
using TMPro;
using UnityEngine;

public class DamageTextInstance : MonoBehaviour
{
    //Quick Reference Properties
    protected float tileSize => GameManager.instance.tileSize;


    //Fields
    [SerializeField] AnimationCurve riseCurve;
    public TextMeshPro textMesh;
    public Vector3 speed;
    public DamageTextStyle style = DamageTextStyle.Oscillate;

    //Properties
    public Transform parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true);
    }

    public Vector3 position
    {
        get => gameObject.transform.position;
        set => gameObject.transform.position = value;
    }

    //Method which is used for initialization tasks that need to occur before the game starts 
    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        speed = new Vector3(tileSize, tileSize / 32, 0);
    }

    public void Spawn(string text, Vector3 position, DamageTextStyle style = DamageTextStyle.Oscillate)
    {
        this.style = style;
        textMesh.text = text;
        transform.position = new Vector3(position.x, position.y + tileSize / 4, 0);

        switch (style)
        {
            case DamageTextStyle.Float:
                StartCoroutine(Float());
                break;
            case DamageTextStyle.Oscillate:
                StartCoroutine(Oscillate());
                break;
            case DamageTextStyle.Bounce:
                StartCoroutine(Bounce());
                break;
            default:
                StartCoroutine(Float());
                break;
        }

    }

    private IEnumerator Float()
    {

        //Before:
        float alpha = 1;
        Color color = ColorHelper.Solid.White;
        Vector3 initialPosition = transform.position;
        float timer = 0f;
        
        //During:
        while (textMesh.color.a > 0)
        {
            alpha -= Increment.OnePercent * 3;
            alpha = Mathf.Max(alpha, 0);

            if (alpha < 0.5)
            {
                color.a = alpha;
                textMesh.color = color;
            }

            timer += Time.deltaTime;

            var x = initialPosition.x;
            var y = position.y + speed.y;

            transform.position = new Vector3(x, y, 0);
            yield return Wait.For(Interval.OneTick);
        }

        //After:
        Destroy(gameObject);
    }

    private IEnumerator Oscillate()
    {

        //Before:
        float alpha = 1;
        Color color = ColorHelper.Solid.White;
        Vector3 initialPosition = transform.position;
        float timer = 0f;
        float duration = 0.25f; //Time for one complete back-and-forth loop

        //During:
        while (textMesh.color.a > 0)
        {
            alpha -= Increment.OnePercent * 3;
            alpha = Mathf.Max(alpha, 0);

            if (alpha < 0.5)
            {
                color.a = alpha;
                textMesh.color = color;
            }

            timer += Time.deltaTime;

            //Calculate the normalized time (0 to 1) based on the duration
            float normalizedTime = (timer % duration) / duration;

            //Use the travelCurve to determine the horizontal boardPosition
            float curveValue = riseCurve.Evaluate(normalizedTime) * tileSize / 8;

            var x = initialPosition.x + curveValue;
            var y = position.y + speed.y;

            transform.position = new Vector3(x, y, 0);
            yield return Wait.For(Interval.OneTick);
        }

        //After:
        Destroy(gameObject);

    }
    private IEnumerator Bounce()
    {
        //Before:
        float alpha = 1f;
        Color color = ColorHelper.Solid.White;
        Vector3 initialPosition = transform.position;

        // Setup bounce physics parameters
        float vY = tileSize * 6;                    // Initial upward velocity
        float gravity = -tileSize * 18f;           // Gravity pulling the textarea down
        float bounceDamping = 0.5f;                // Reduces bounce height after each impact
        float groundY = initialPosition.y;         // The starting y position is treated as ground level
        float bounceEnd = tileSize * 0.1f;         // Threshold below which bouncing stops
        float horizontalFocus = tileSize * Constants.percent33 * Random.Float(-1f, 1f);
        int bounceCount = 0;

        // Flag to determine when the fade should start (once the first bounce occurs)
        bool fadeStarted = false;

        // Bounce Phase: simulate bouncing until fully faded out
        while (alpha > 0)
        {
            // Apply gravity to vertical velocity
            vY += gravity * Time.deltaTime;

            // Save the CurrentProfile position based on vertical velocity and horizontal move
            Vector3 position = transform.position;
            position.y += vY * Time.deltaTime;
            position.x += bounceCount <= 3 ? horizontalFocus * Time.deltaTime : 0;

            // Check if the textarea has hit (or gone below) the ground level
            if (position.y <= groundY)
            {
                position.y = groundY;

                // Start fading as soon as the first bounce occurs
                if (!fadeStarted)
                {
                    fadeStarted = true;
                }

                // Bounce logic: if the bounce energy is too low, set velocity to zero;
                // otherwise, reverse the velocity with damping to simulate a bounce.
                if (Mathf.Abs(vY) < bounceEnd)
                {
                    vY = 0;
                }
                else
                {
                    vY = -vY * bounceDamping;
                    bounceCount++;
                }
            }

            // Apply the updated position to the object
            transform.position = position;

            // If fading has started, reduce the alpha concurrently with bouncing
            if (fadeStarted)
            {
                alpha -= Increment.OnePercent * 3;
                alpha = Mathf.Max(alpha, 0);
                color.a = alpha;
                textMesh.color = color;
            }

            yield return Wait.For(Interval.OneTick);
        }

        //After:
        Destroy(gameObject);
    }



}
