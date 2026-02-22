using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<ItemControllerUI> Items = new List<ItemControllerUI>();
    public InventoryCell[] allCells;

    void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemControllerUI item)
    {
        if (!Items.Contains(item))
            Items.Add(item);
    }

    public void RemoveItem(ItemControllerUI item)
    {
        if (Items.Contains(item))
            Items.Remove(item);
    }
}