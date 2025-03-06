using UnityEngine;
using UnityEngine.EventSystems;

public class PointerManager : MonoBehaviour, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Camera mainCamera;

    public bool IsTouchInsideScreenBounds =>
        Input.mousePosition.x >= 0
        && Input.mousePosition.x <= Screen.width
        && Input.mousePosition.y > 0
        && Input.mousePosition.x <= Screen.height;

    //Method which is used for initialization tasks that need to occur before the game starts 
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        mainCamera = Camera.main;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        //GameManager.db.touchPosition2D = Input.mousePosition;
        //GameManager.db.touchPosition3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
        Vector2 cubeRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D cubeHit = Physics2D.Raycast(cubeRay, Vector2.zero);

        if (cubeHit)
        {
            //Debug.Log("We hit " + cubeHit.collider.name);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("OnPointerClick");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

    }

    public void Update()
    {
        if (Input.mousePosition == null)
            return;

        GameManager.instance.touchPosition2D = Input.mousePosition;
        if (IsTouchInsideScreenBounds && mainCamera != null)
            GameManager.instance.touchPosition3D = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

}
