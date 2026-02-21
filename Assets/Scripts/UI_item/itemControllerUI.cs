using UnityEngine;
using UnityEngine.EventSystems;

public class itemControllerUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 offset;

    void Awake()
    {
        rectTransform=GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

    }
    public void OnPointerDown(PointerEventData eventData)
    {
       Vector2 localPoint;
       RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas.transform as RectTransform,
        eventData.position,
        canvas.worldCamera,
        out localPoint
       );
       offset=rectTransform.anchoredPosition-localPoint;
    }
    public void OnDrag(PointerEventData eventData)
    {
         Vector2 localPoint;
         if( RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out localPoint
            ))
        {
            rectTransform.anchoredPosition=localPoint+offset;
        }
       
    }

   

    public void OnPointerUp(PointerEventData eventData)
    {
       Debug.Log($"{eventData} bırakıldıu");
    }
}
