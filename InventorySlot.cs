using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int slotIndex; // Make sure this is correctly assigned or calculated

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
