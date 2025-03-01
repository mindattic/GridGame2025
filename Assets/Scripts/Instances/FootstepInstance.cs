using System.Collections;
using UnityEngine;

public class FootstepInstance : MonoBehaviour
{
    //Quick Reference Properties
    protected Vector3 tileScale => GameManager.instance.tileScale;
    protected ResourceManager resourceManager => GameManager.instance.resourceManager;

    //Inernal properties
    public string Name
    {
        get => name;
        set => Name = value;
    }

    public Transform parent
    {
        get => gameObject.transform.parent;
        set => gameObject.transform.SetParent(value, true);
    }

    public Vector3 Position
    {
        get => gameObject.transform.position;
        set => gameObject.transform.position = value;
    }

    public Quaternion Rotation
    {
        get => gameObject.transform.rotation;
        set => gameObject.transform.rotation = value;
    }




    public Sprite sprite
    {
        get => spriteRenderer.sprite;
        set => spriteRenderer.sprite = value;
    }


    //Fields
    float Duration;
    SpriteRenderer spriteRenderer;


    //Method which is used for initialization tasks that need to occur before the game starts 
    private void Awake()
    {
        transform.localScale = tileScale / 2;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Duration = Interval.OneSecond * 10;

    }

     public void Spawn(Vector3 position, Quaternion rotation, bool isRightFoot)
    {
        this.Position = position;
        this.Rotation = rotation;
        spriteRenderer.flipX = !isRightFoot;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return Wait.For(Duration);

        float alpha = spriteRenderer.color.a;
        spriteRenderer.color = new Color(1, 1, 1, alpha);

        while (alpha > 0)
        {
            alpha -= Increment.OnePercent;
            alpha = Mathf.Max(alpha, 0f);
            spriteRenderer.color = new Color(1, 1, 1, alpha);

            yield return Wait.For(Interval.TenTicks);
        }

        Destroy(this.gameObject);
    }

}
