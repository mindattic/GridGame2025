using System.Collections;
using TMPro;
using UnityEngine;

public class TooltipInstance : MonoBehaviour
{
    #region Properties
    protected float tileSize => GameManager.instance.tileSize;
    #endregion

    //Fields
    SpriteRenderer spriteRenderer;
    TextMeshPro textMesh;
    Vector2 offset;

    #region Components

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

    #endregion

    //Method which is used for initialization tasks that need to occur before the game starts 
    void Awake()
    {

        spriteRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        textMesh = gameObject.transform.GetChild(1).GetComponent<TextMeshPro>();
        offset = new Vector2(tileSize / 2, 0);
    }

    public void Spawn(string text, Vector3 position)
    {
        textMesh.text = text;
        var x = position.x + offset.x;
        var y = position.y + offset.y;
        this.position = new Vector3(x, y, 0);
        this.scale = Geometry.Tile.Relative.Scale(3, 1, 1);

        IEnumerator _()
        {
            //Before:
            float alpha = 1f;
            Color color = ColorHelper.Solid.White;
            textMesh.color = color;
            spriteRenderer.color = color;

            //During:
            yield return Wait.For(Interval.FourSeconds);
            while (textMesh.color.a > 0)
            {
                alpha -= Increment.OnePercent;
                alpha = Mathf.Max(alpha, 0);
                color.a = alpha;
                textMesh.color = color;
                spriteRenderer.color = color;
                yield return Wait.For(Interval.OneTick);
            }

            //After:
            textMesh.color = ColorHelper.Transparent.White;
            spriteRenderer.color = ColorHelper.Transparent.White;
        }

        StartCoroutine(_());
    }

}
