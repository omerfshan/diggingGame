
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemControllerUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 offset;

    private InventoryCell currentCell;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
{
    if (currentCell != null)
    {
        currentCell.Clear();
        // InventoryManager.Instance.RemoveItem(this); // 👈 BURASI
        currentCell = null;
    }

    rectTransform.SetParent(canvas.transform); // önemli

    Vector2 localPoint;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas.transform as RectTransform,
        eventData.position,
        canvas.worldCamera,
        out localPoint
    );

    offset = rectTransform.anchoredPosition - localPoint;
}

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint
        ))
        {
            rectTransform.anchoredPosition = localPoint + offset;
        }
    }

   public void OnPointerUp(PointerEventData eventData)
{
    InventoryCell closestCell = null;
    float minDistance = float.MaxValue;

    foreach (InventoryCell cell in InventoryManager.Instance.allCells)
    {
        float distance = Vector2.Distance(
            rectTransform.position,
            cell.transform.position
        );

        if (distance < minDistance)
        {
            minDistance = distance;
            closestCell = cell;
        }
    }

    if (closestCell != null && closestCell.IsEmpty())
    {
        SnapToCell(closestCell);
    }
}

void SnapToCell(InventoryCell cell)
{
    if (currentCell != null)
        currentCell.Clear();

    currentCell = cell;
    cell.SetItem(this);

    rectTransform.SetParent(cell.transform);
    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
    rectTransform.pivot = new Vector2(0.5f, 0.5f);
    rectTransform.anchoredPosition = Vector2.zero;

    // InventoryManager.Instance.AddItem(this); // 👈 BURASI
}
}