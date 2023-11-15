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
            Image slotImage = slot.GetComponent<Image>();
            Text slotText = slot.GetComponentInChildren<Text>();

            if (i < inventory.Count)
            {
                // Slot for an item in the inventory
                Item item = inventory[i];
                slotImage.sprite = item.itemImage;
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
