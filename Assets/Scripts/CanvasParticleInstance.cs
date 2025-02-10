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
    private RectTransform rectTransform;

    public void Initialize(float rotationSpeed, float horizontalSpeed, float fallSpeed)
    {
        this.xRotationSpeed = 0; // Random.Float(0, rotationSpeed);
        this.yRotationSpeed = Random.Float(0, rotationSpeed);
        this.zRotationSpeed = rotationSpeed;
        this.horizontalSpeed = horizontalSpeed;
        this.fallSpeed = fallSpeed;
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(MoveAndDestroy());
    }

    private IEnumerator MoveAndDestroy()
    {

        while (rectTransform.anchoredPosition.x < Screen.width)
        {
            rectTransform.anchoredPosition += new Vector2(
                horizontalSpeed * Time.deltaTime,
                -fallSpeed * Time.deltaTime);

            rectTransform.Rotate(
                xRotationSpeed * Time.deltaTime,
                yRotationSpeed * Time.deltaTime,
                zRotationSpeed * Time.deltaTime);

            yield return null;
        }
        Destroy(gameObject);
    }
}