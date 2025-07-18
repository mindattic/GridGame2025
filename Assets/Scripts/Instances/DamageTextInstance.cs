using System.Collections;
using TMPro;
using UnityEngine;
using game = GameManagerHelper;

public class DamageTextInstance : MonoBehaviour
{
    // Quick reference to tile size from GameManager
    //protected float tileSize => GameManager.instance.tileSize;

    [SerializeField] AnimationCurve riseCurve;
    public TextMeshPro textMesh;
    public Vector3 speed;
    public TextMotionStyle style = TextMotionStyle.Oscillate;

    // Transform parent property
    public Transform parent
    {
        get => transform.parent;
        set => transform.SetParent(value, true);
    }

    // Transform position property
    public Vector3 position
    {
        get => transform.position;
        set => transform.position = value;
    }

    // Called before the first frame update
    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        speed = new Vector3(game.TileSize, game.TileSize / 32, 0);
    }

    /// <summary>
    /// Spawns the floating damage text with a given style and position
    /// </summary>
    public void Spawn(string text, Vector3 pos, TextMotionStyle style = TextMotionStyle.Oscillate)
    {
        this.style = style;
        textMesh.text = text;
        transform.position = new Vector3(
            pos.x + Random.Range(game.TileSize / 4), 
            pos.y + game.TileSize / 4, 
            0);

        // Start the motion coroutine based on selected style
        StartCoroutine(style switch
        {
            TextMotionStyle.Float => Float(),
            TextMotionStyle.Oscillate => Oscillate(),
            TextMotionStyle.Bounce => Bounce(),
            _ => Float(),
        });
    }

    // Floats the text upward while fading out
    private IEnumerator Float()
    {
        float alpha = 1;
        Color color = ColorHelper.Solid.White;
        Vector3 startPos = transform.position;

        while (textMesh.color.a > 0)
        {
            alpha = Mathf.Max(alpha - Increment.Percent3, 0);
            if (alpha < 0.5f)
            {
                color.a = alpha;
                textMesh.color = color;
            }

            // Move upward
            transform.position = new Vector3(startPos.x, position.y + speed.y, 0);
            yield return Wait.For(Interval.OneTick);
        }
        Destroy(gameObject);
    }

    // Oscillates the text horizontally while rising and fading out
    private IEnumerator Oscillate()
    {
        float alpha = 1;
        Color color = ColorHelper.Solid.White;
        Vector3 startPos = transform.position;
        float timer = 0f, duration = 0.25f;

        while (textMesh.color.a > 0)
        {
            alpha = Mathf.Max(alpha - Increment.Percent3, 0);
            if (alpha < 0.5f)
            {
                color.a = alpha;
                textMesh.color = color;
            }

            timer += Time.deltaTime;
            float normalized = (timer % duration) / duration;
            float curve = riseCurve.Evaluate(normalized) * game.TileSize / 8;

            transform.position = new Vector3(startPos.x + curve, position.y + speed.y, 0);
            yield return Wait.For(Interval.OneTick);
        }
        Destroy(gameObject);
    }

    // Bounces the text and fades it out after the first bounce
    private IEnumerator Bounce()
    {
        float alpha = 1f;
        Color color = ColorHelper.Solid.White;
        Vector3 startPos = transform.position;

        float vY = game.TileSize * 6, gravity = -game.TileSize * 18f;
        float bounceDamping = 0.5f, groundY = startPos.y, bounceEnd = game.TileSize * 0.1f;
        float hFocus = game.TileSize * Increment.Percent33 * Random.Float(-1f, 1f);
        int bounceCount = 0;
        bool fadeStarted = false;

        while (alpha > 0)
        {
            vY += gravity * Time.deltaTime;
            Vector3 pos = transform.position;
            pos.y += vY * Time.deltaTime;
            if (bounceCount <= 3) pos.x += hFocus * Time.deltaTime;

            if (pos.y <= groundY)
            {
                pos.y = groundY;
                if (!fadeStarted) fadeStarted = true;
                if (Mathf.Abs(vY) < bounceEnd) vY = 0;
                else { vY = -vY * bounceDamping; bounceCount++; }
            }

            transform.position = pos;

            if (fadeStarted)
            {
                alpha = Mathf.Max(alpha - Increment.Percent3, 0);
                color.a = alpha;
                textMesh.color = color;
            }

            yield return Wait.For(Interval.OneTick);
        }
        Destroy(gameObject);
    }
}
