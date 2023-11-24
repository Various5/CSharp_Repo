using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject weaponSlotUI;
    public GameObject inventorySlotPrefab;
    public Transform weaponSlot1;
    public Transform weaponSlot2;
    private const int MaxSlots = 16;
    private const int MaxWeaponSlots = 2;
    private InventorySlot[] inventorySlots;
    private InventorySlot[] weaponSlots;

    void Start()
    {
        InitializeInventorySlots();
        InitializeWeaponSlots();
    }
    public static InventoryUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void InitializeInventorySlots()
    {
        inventorySlots = new InventorySlot[MaxSlots];
        for (int i = 0; i < MaxSlots; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryPanel.transform);
            InventorySlot inventorySlotComponent = slot.GetComponent<InventorySlot>();
            DragDropItem dragDropItem = slot.GetComponentInChildren<DragDropItem>();

            inventorySlotComponent.slotIndex = i;
            dragDropItem.inventoryPanel = inventoryPanel.GetComponent<RectTransform>();
            inventorySlots[i] = inventorySlotComponent;
        }
    }

    void InitializeWeaponSlots()
    {
        weaponSlots = new InventorySlot[MaxWeaponSlots];

        for (int i = 0; i < MaxWeaponSlots; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, weaponSlotUI.transform);
            InventorySlot weaponSlotComponent = slot.GetComponent<InventorySlot>();
            DragDropItem dragDropItem = slot.GetComponentInChildren<DragDropItem>();

            weaponSlotComponent.slotIndex = i;
            dragDropItem.inventoryPanel = weaponSlotUI.GetComponent<RectTransform>();
            weaponSlots[i] = weaponSlotComponent;
        }
    }

    public void UpdateInventory(List<Item> inventory)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            Image slotImage = slot.GetComponent<Image>();
            Text slotText = slot.GetComponentInChildren<Text>();

            if (i < inventory.Count)
            {
                Item item = inventory[i];
                slot.SetItemData(item);
                slotImage.sprite = item.itemImage;
                slotText.text = item.isStackable ? item.stackSize.ToString() : "";
            }
            else
            {
                slot.ClearSlot();
            }
        }
    }

    public void SwapItems(InventorySlot slot1, InventorySlot slot2)
    {
        Item tempItem = slot1.GetItemData();
        slot1.SetItemData(slot2.GetItemData());
        slot2.SetItemData(tempItem);
    }
}
