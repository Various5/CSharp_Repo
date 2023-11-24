using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public bool IsEmpty()
    {
        return itemData == null;
    }

    public void ClearSlot()
    {
        itemData = null;
        GetComponent<Image>().sprite = null;
        GetComponentInChildren<Text>().text = "";
    }

    public void OnDrop(PointerEventData eventData)
    {
        DragDropItem droppedItem = eventData.pointerDrag.GetComponent<DragDropItem>();
        if (droppedItem != null)
        {
            InventorySlot originalSlot = droppedItem.GetOriginalSlot();
            if (originalSlot != this)
            {
                InventoryUI.Instance.SwapItems(this, originalSlot);
            }
        }
    }
}
