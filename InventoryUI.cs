using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject inventorySlotPrefab;
    private const int MaxSlots = 12;

    public void UpdateInventory(List<Item> inventory)
    {
        // Clear existing inventory slots
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Create slots for each item in inventory
        for (int i = 0; i < MaxSlots; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryPanel.transform);

            // Assign the slot index
            InventorySlot inventorySlotComponent = slot.GetComponent<InventorySlot>();
            if (inventorySlotComponent != null)
            {
                inventorySlotComponent.slotIndex = i;
            }

            Image slotImage = slot.GetComponent<Image>();
            Text slotText = slot.GetComponentInChildren<Text>();

            if (i < inventory.Count)
            {
                // Slot for an item in the inventory
                Item item = inventory[i];
                slotImage.sprite = item.itemImage; // Assuming itemImage is the sprite to display
                slotText.text = item.isStackable ? item.stackSize.ToString() : "";
            }
            else
            {
                // Empty slot
                slotImage.sprite = null; // Or a default 'empty' sprite if you have one
                slotText.text = "";
            }
        }
    }
}
