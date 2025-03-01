using System.Collections;
using TMPro;
using UnityEngine;

public class DamageTextInstance : MonoBehaviour
{
   //Quick Reference Properties
    protected float tileSize => GameManager.instance.tileSize;
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

    public TextMeshPro textMesh;
    public Vector3 speed;
    [SerializeField] AnimationCurve riseCurve;

    //Method which is used for initialization tasks that need to occur before the game starts 
    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        speed = new Vector3(tileSize, tileSize / 32, 0);
    }

    public void Spawn(string text, Vector3 position)
    {
        textMesh.text = text;
        var x = position.x;
        var y = position.y + tileSize / 4;
        transform.position = new Vector3(x, y, 0);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
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

}
