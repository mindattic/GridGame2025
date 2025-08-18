using Assets.Helpers;
using UnityEngine;

public class PointerManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public bool IsTouchInsideScreenBounds
    {
        get
        {
            Vector2 vp = UnitConversionHelper.Screen.ToViewport(Input.mousePosition);
            return vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f;
        }
    }


    //Method which is used for initialization tasks that need to occur before the game starts 
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Update()
    {
        // Unity always returns a Vector3 for Input.mousePosition, so no need for null check
        Vector2 screenPos = Input.mousePosition;

        GameManager.instance.touchPosition2D = screenPos;

        if (IsTouchInsideScreenBounds && Camera.main != null)
        {
            // Convert to world on Z=0 plane (or whichever Z-plane your gameplay lives on)
            GameManager.instance.touchPosition3D = UnitConversionHelper.Screen.ToWorld(screenPos, 0f);
        }
    }


}
