using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject inventoryPanel;
    public GameObject weaponSlotUI;
    public GameObject inventorySlotPrefab;
    public Transform weaponSlot1;
    public Transform weaponSlot2;
    private const int MaxSlots = 12;
    private InventorySlot[] inventorySlots;
    public static InventoryUI Instance { get; private set; }

    void Start()
    {
        if (inventorySlots == null || inventorySlots.Length == 0)
        {
            InitializeInventorySlots();
        }
    }

    void InitializeInventorySlots()
    {
        inventorySlots = new InventorySlot[MaxSlots];
        for (int i = 0; i < MaxSlots; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryPanel.transform);
            InventorySlot inventorySlotComponent = slot.GetComponent<InventorySlot>();
            DragDropItem dragDropItem = slot.GetComponentInChildren<DragDropItem>();

            if (inventorySlotComponent != null && dragDropItem != null)
            {
                inventorySlotComponent.slotIndex = i;
                dragDropItem.inventoryPanel = inventoryPanel.GetComponent<RectTransform>();
                inventorySlots[i] = inventorySlotComponent;
            }
        }
    }

    public void UpdateInventory(List<Item> inventory)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            Image slotImage = slot.GetComponent<Image>();
            Text slotText = slot.GetComponentInChildren<Text>();
            
            if (inventory == null)
            {
                Debug.LogError("Inventory list is null.");
                return;
            }
            if (i < inventory.Count)
            {
                Item item = inventory[i];
                slot.SetItemData(item); // Store item data in slot
                slotImage.sprite = item.itemImage;
                slotText.text = item.isStackable ? item.stackSize.ToString() : "";

                DragDropItem dragDropItem = slot.GetComponentInChildren<DragDropItem>();
                if (dragDropItem != null)
                {
                    dragDropItem.SetItemPrefab(item.itemPrefab);
                }
            }
            else
            {
                slotImage.sprite = null;
                slotText.text = "";
            }
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Call this method to update weapon slots separately
    public void UpdateWeaponSlots(Item weapon1, Item weapon2)
    {
        Image weaponSlot1Image = weaponSlot1.GetComponent<Image>();
        Image weaponSlot2Image = weaponSlot2.GetComponent<Image>();

        weaponSlot1Image.sprite = weapon1 ? weapon1.itemImage : null;
        weaponSlot2Image.sprite = weapon2 ? weapon2.itemImage : null;
        SetupDragDrop(weaponSlot1, weapon1);
        SetupDragDrop(weaponSlot2, weapon2);
        // If you have more slots for armor and other equipment, update them here
    }
    public void DropItemOnWeaponSlot(Item item, Transform weaponSlot)
    {
        if (item.weaponType == WeaponType.Pistol || item.weaponType == WeaponType.Shotgun || item.weaponType == WeaponType.Rifle)
        {
            // Handle the logic for equipping the weapon to the player
            // Update the weapon slot UI
        }
        // Add additional conditions for other weapon types like Melee
    }
    void SetupInventorySlotUI(Item item, InventorySlot slot)
    {
        DragDropItem dragDropItem = slot.GetComponent<DragDropItem>();
        if (dragDropItem != null)
        {
            dragDropItem.SetItemPrefab(item.itemPrefab);
        }
    }

    void SetupDragDrop(Transform slotTransform, Item item)
    {
        DragDropItem dragDropItem = slotTransform.GetComponentInChildren<DragDropItem>();
        if (dragDropItem != null)
        {
            dragDropItem.SetItemPrefab(item.itemPrefab);
        }
    }
    // Implement drag and drop functionality
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Handle beginning of drag event
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Handle during drag event
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Handle end of drag event
    }
}
