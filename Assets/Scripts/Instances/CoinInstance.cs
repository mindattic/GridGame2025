using UnityEngine;

public class CoinInstance : MonoBehaviour
{
   //Quick Reference Properties
    protected float tileSize => GameManager.instance.tileSize;
    protected Vector3 tileScale => GameManager.instance.tileScale;
    protected CoinBar coinBar => GameManager.instance.coinBar;
    protected int totalCoins { get => GameManager.instance.totalCoins; set => GameManager.instance.totalCoins = value; }
    protected AudioManager audioManager => GameManager.instance.audioManager;



    //Fields
    [SerializeField] public AnimationCurve linearCurve;
    [SerializeField] public AnimationCurve slopeCurve;
    [SerializeField] public AnimationCurve sineCurve;
    private SpriteRenderer spriteRenderer;
    private ParticleSystem particles;
    private float scaleMultiplier = 0.05f;
    private float startDuration = 0.2f;
    private float moveDuration = 0.6f;
    private float timeElapsed = 0.0f;
    private Vector3 start;
    private Vector3 end;
    private CoinState state;
    private float t;
    private float x;
    private float y;
    private float z;
    AnimationCurve cX;
    AnimationCurve cY;

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
    public Quaternion rotation
    {
        get => gameObject.transform.rotation;
        set => gameObject.transform.rotation = value;
    }
    public Vector3 scale
    {
        get => gameObject.transform.localScale;
        set => gameObject.transform.localScale = value;
    }


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        particles = GetComponent<ParticleSystem>();

        transform.localScale = tileScale * scaleMultiplier;
    }

    public void Spawn(Vector3 position)
    {
        start = position.RandomizeOffset(tileSize * 0.25f);
        end = position.RandomizeOffset(tileSize * 1.5f);
        timeElapsed = 0;
        startDuration += Random.Float(0, 0.2f);
        moveDuration += Random.Float(0, 0.2f);
        cX = RandomCurve();
        cY = RandomCurve();
        transform.position = start;
        state = CoinState.Start;
    }

    private AnimationCurve RandomCurve()
    {
        var r = Random.Int(1, 3);
        if (r == 1) return linearCurve;
        if (r == 2) return slopeCurve;
        return sineCurve;
    }


    public void Update()
    {

        switch (state)
        {
            case CoinState.Start:
                t = Mathf.Clamp01(timeElapsed / startDuration);
                x = Mathf.Lerp(start.x, end.x, cX.Evaluate(t));
                y = Mathf.Lerp(start.y, end.y, cY.Evaluate(t));
                z = transform.position.z;
                transform.position = new Vector3(x, y, z);
                if (timeElapsed >= startDuration)
                {
                    timeElapsed = 0;
                    start = transform.position;
                    end = coinBar.GetIconWorldPosition();
                    state = CoinState.Move;
                }
                break;

            case CoinState.Move:
                t = Mathf.Clamp01(timeElapsed / moveDuration);
                x = Mathf.Lerp(start.x, end.x, sineCurve.Evaluate(t));
                y = Mathf.Lerp(start.y, end.y, sineCurve.Evaluate(t));
                z = transform.position.z;
                transform.position = new Vector3(x, y, z);
                if (timeElapsed >= moveDuration)
                {
                    timeElapsed = 0;
                    state = CoinState.Stop;
                }
                break;

            case CoinState.Stop:
                spriteRenderer.enabled = false;
                particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                totalCoins++;
                coinBar.value.text = totalCoins.ToString("D7");
                audioManager.Play($"Move{Random.Int(1, 6)}");
                state = CoinState.Destroy;
                break;

            case CoinState.Destroy:
                Destroy(gameObject);
                break;
        }


        timeElapsed += Time.deltaTime;

    }
}
