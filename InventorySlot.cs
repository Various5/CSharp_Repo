using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    private Item itemData;

    public void SetItemData(Item item)
    {
        itemData = item;
    }

    public Item GetItemData()
    {
        return itemData;
    }

    public void OnDrop(PointerEventData eventData)
    {
        DragDropItem droppedItem = eventData.pointerDrag.GetComponent<DragDropItem>();
        if (droppedItem != null)
        {
            droppedItem.DroppedOnSlot(this);
        }
    }

    public int GetSlotIndex()
    {
        return slotIndex;
    }
}
