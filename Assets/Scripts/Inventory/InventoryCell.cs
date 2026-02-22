using UnityEngine;

public class InventoryCell : MonoBehaviour
{
    public ItemControllerUI currentItem;

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public void SetItem(ItemControllerUI item)
    {
        currentItem = item;
    }

    public void Clear()
    {
        currentItem = null;
    }
}