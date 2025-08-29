//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//[DisallowMultipleComponent]
//[RequireComponent(typeof(RectTransform))]
//public sealed class OverworldMapInstance : MonoBehaviour,
//    IPointerDownHandler, IInitializePotentialDragHandler,
//    IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
//{
//    [SerializeField] public ScrollRect target;

//    private void Awake()
//    {
//        // Ensure the Map is raycastable so events reach this proxy
//        var g = GetComponent<Graphic>();
//        if (g == null) g = gameObject.AddComponent<Image>(); // invisible receiver
//        g.raycastTarget = true;
//        if (g is Image img && img.sprite == null) img.color = new Color(1f, 1f, 1f, 0.0001f);
//    }

//    public void OnPointerDown(PointerEventData e)                { if (target != null) target.OnPointerDown(e); }
//    public void OnInitializePotentialDrag(PointerEventData e)    { if (target != null) target.OnInitializePotentialDrag(e); }
//    public void OnBeginDrag(PointerEventData e)                  { if (target != null) target.OnBeginDrag(e); }
//    public void OnDrag(PointerEventData e)                       { if (target != null) target.OnDrag(e); }
//    public void OnEndDrag(PointerEventData e)                    { if (target != null) target.OnEndDrag(e); }
//    public void OnScroll(PointerEventData e)                     { if (target != null) target.OnScroll(e); }
//}
