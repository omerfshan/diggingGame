using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 8;
    public int gridHeight = 6;
    public float cellSize = 120f;

    [Header("References")]
    public InventoryCell[] cells;
    private RectTransform rect;
    public InventoryCell[,] InventoryCells;
    public List<ItemControllerUI> items;
    void Awake()
{
    rect = GetComponent<RectTransform>();
    items = new List<ItemControllerUI>();
}

    void Start()
    {
        InitializeGrid();
    }

   void InitializeGrid()
{
    if (cells == null || cells.Length == 0)
        return;

    InventoryCells = new InventoryCell[gridWidth, gridHeight];

    int index = 0;

    for (int y = 0; y < gridHeight; y++)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (index >= cells.Length)
            {
                Debug.LogError("Not enough cells assigned!");
                continue; // return değil!
            }

            InventoryCells[x, y] = cells[index];

            if (InventoryCells[x, y] != null)
            {
                InventoryCells[x, y].SetEmpty();
                InventoryCells[x, y].is_filled = false;
            }

            index++;
        }
    }
}

    public void AddList(ItemControllerUI item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
        }
    }
    public void RemoveList(ItemControllerUI item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }

    public bool ScreenToGrid(Vector2 screenPos, out int gx, out int gy)
    {
        gx = -1;
        gy = -1;

        Canvas canvas = GetComponentInParent<Canvas>();
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            screenPos,
            cam,
            out Vector2 localPos
        );

        float totalWidth = gridWidth * cellSize;
        float totalHeight = gridHeight * cellSize;

        float px = localPos.x + totalWidth * 0.5f;
        float py = totalHeight * 0.5f - localPos.y;

        gx = Mathf.FloorToInt(px / cellSize);
        gy = Mathf.FloorToInt(py / cellSize);

        return gx >= 0 && gy >= 0 && gx < gridWidth && gy < gridHeight;
    }

   

  public Vector2 GridToPos(int gx, int gy, int w, int h)
{
    float totalWidth = gridWidth * cellSize;
    float totalHeight = gridHeight * cellSize;

    float x = -totalWidth * 0.5f + gx * cellSize + (w * cellSize) * 0.5f;
    float y =  totalHeight * 0.5f - gy * cellSize - (h * cellSize) * 0.5f;

    return new Vector2(x, y);
}

   

    public bool CanPlace(int gx, int gy, ItemControllerUI item)
    {
        if (gx < 0 || gy < 0)
            return false;

        if (gx + item.width > gridWidth ||
            gy + item.height > gridHeight)
            return false;

        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (!item.IsCellInShape(x, y))
                    continue;

                if (InventoryCells[gx + x, gy + y].is_filled)
                    return false;
            }
        }

        return true;
    }

   

    public void FillArea(int gx, int gy, ItemControllerUI item)
    {
        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (!item.IsCellInShape(x, y))
                    continue;

                InventoryCells[gx + x, gy + y].is_filled = true;
                InventoryCells[gx + x, gy + y].SetFilled();
            }
        }
    }

    public void ClearArea(int gx, int gy, ItemControllerUI item)
    {
        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (!item.IsCellInShape(x, y))
                    continue;

                InventoryCells[gx + x, gy + y].is_filled = false;
                InventoryCells[gx + x, gy + y].SetEmpty();
            }
        }
    }

    public void ClearAllHover()
{
    for (int y = 0; y < gridHeight; y++)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (InventoryCells[x, y] != null)
                InventoryCells[x, y].Refresh();
        }
    }
}

    public void HighlightArea(int gx, int gy, ItemControllerUI item)
{
    ClearAllHover();

    bool canPlace = CanPlace(gx, gy, item);

    for (int x = 0; x < item.width; x++)
    {
        for (int y = 0; y < item.height; y++)
        {
            if (!item.IsCellInShape(x, y))
                continue;

            int tx = gx + x;
            int ty = gy + y;

            // 🔥 BOUNDS CHECK
            if (tx < 0 || ty < 0 || tx >= gridWidth || ty >= gridHeight)
                continue;

            if (canPlace)
                InventoryCells[tx, ty].SetValid();
            else
                InventoryCells[tx, ty].SetInvalid();
        }
    }
}
}