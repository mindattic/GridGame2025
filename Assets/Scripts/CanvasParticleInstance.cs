using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class CanvasParticleInstance : MonoBehaviour
{
    private float xRotationSpeed;
    private float yRotationSpeed;
    private float zRotationSpeed;
    private float horizontalSpeed;
    private float fallSpeed;
    private float lifetime;
    private RectTransform rectTransform;

    public void Initialize(float rotationSpeed, float horizontalSpeed, float fallSpeed, float lifetime)
    {
        this.xRotationSpeed = Random.Float(0, rotationSpeed);
        this.yRotationSpeed = 0;
        this.zRotationSpeed = rotationSpeed;
        this.horizontalSpeed = horizontalSpeed;
        this.fallSpeed = fallSpeed;
        this.lifetime = lifetime;
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(MoveAndDestroy());
    }

    private IEnumerator MoveAndDestroy()
    {
        float elapsedTime = 0f;
        while (elapsedTime < lifetime)
        {
            rectTransform.anchoredPosition += new Vector2(
                horizontalSpeed * Time.deltaTime,
                -fallSpeed * Time.deltaTime);

            rectTransform.Rotate(
                xRotationSpeed * Time.deltaTime,
                yRotationSpeed * Time.deltaTime,
                zRotationSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}