using UnityEngine;
using UnityEngine.EventSystems;

public class ItemControllerUI :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Core")]
    public RectTransform rect;
    public Canvas canvas;

    [Header("Shape Data")]
    public int[] shape;
    public int width = 1;
    public int height = 1;

    private Vector2 originalAnchoredPos;
    private Transform originalParent;

    private InventoryGrid currentGrid;
    private InventoryGrid lastGrid;
    private InventoryGrid previousHoverGrid;

    private int lastGX = -1;
    private int lastGY = -1;

    private Camera cam;

    // =========================
    // INIT
    // =========================

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;
    }

    // =========================
    // BEGIN DRAG
    // =========================

  public void OnBeginDrag(PointerEventData eventData)
{
    originalParent = transform.parent;
    originalAnchoredPos = rect.anchoredPosition;

    transform.SetParent(canvas.transform, false);
    rect.localScale = Vector3.one;
    rect.SetAsLastSibling();

    if (lastGrid != null && lastGX != -1)
    {
        lastGrid.ClearArea(lastGX, lastGY, this);
        lastGrid.RemoveList(this); // 🔥 BURASI
    }
}

    // =========================
    // DRAG
    // =========================

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            cam,
            out Vector2 localPoint
        );

        rect.anchoredPosition = localPoint;

        InventoryGrid newGrid = GetGridUnderPointer(eventData.position);

        if (previousHoverGrid != null && previousHoverGrid != newGrid)
        {
            previousHoverGrid.ClearAllHover();
        }

        currentGrid = newGrid;
        previousHoverGrid = newGrid;

        if (currentGrid != null)
        {
            HandleGridHighlight(currentGrid, eventData.position);
        }
    }

    // =========================
    // END DRAG
    // =========================

    public void OnEndDrag(PointerEventData eventData)
    {
        if (previousHoverGrid != null)
        {
            previousHoverGrid.ClearAllHover();
            previousHoverGrid = null;
        }

        if (currentGrid != null)
        {
            if (currentGrid.ScreenToGrid(eventData.position, out int gx, out int gy))
            {
                // 🔥 OFFSET YOK
                if (currentGrid.CanPlace(gx, gy, this))
                {
                    PlaceItem(currentGrid, gx, gy);
                    return;
                }
            }
        }

        if (lastGrid != null && lastGX != -1)
        {
            lastGrid.FillArea(lastGX, lastGY, this);
        }

        ReturnToOriginal();
    }

    // =========================
    // PLACE
    // =========================

  private void PlaceItem(InventoryGrid grid, int gx, int gy)
{
    // Eğer başka griddeyse oradan sil
    if (lastGrid != null && lastGrid != grid)
    {
        lastGrid.RemoveList(this);
    }

    transform.SetParent(grid.transform, false);
    rect.localScale = Vector3.one;

    rect.anchoredPosition = grid.GridToPos(gx, gy, width, height);

    grid.FillArea(gx, gy, this);

    grid.AddList(this); // 🔥 BURASI ÖNEMLİ

    lastGrid = grid;
    lastGX = gx;
    lastGY = gy;
}

    private void ReturnToOriginal()
    {
        transform.SetParent(originalParent, false);
        rect.localScale = Vector3.one;
        rect.anchoredPosition = originalAnchoredPos;
    }

    // =========================
    // SHAPE CHECK
    // =========================

    public bool IsCellInShape(int x, int y)
    {
        int index = y * width + x;
        return index >= 0 && index < shape.Length && shape[index] == 1;
    }

    // =========================
    // GRID DETECTION
    // =========================

    private InventoryGrid GetGridUnderPointer(Vector2 screenPos)
    {
        InventoryGrid[] grids =
            FindObjectsByType<InventoryGrid>(FindObjectsSortMode.None);

        foreach (var grid in grids)
        {
            RectTransform gridRect = grid.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(
                gridRect,
                screenPos,
                cam))
            {
                return grid;
            }
        }

        return null;
    }

    // =========================
    // HIGHLIGHT
    // =========================

    private void HandleGridHighlight(InventoryGrid grid, Vector2 pointerPos)
    {
        if (grid.ScreenToGrid(pointerPos, out int gx, out int gy))
        {
            grid.HighlightArea(gx, gy, this);
        }
    }
}