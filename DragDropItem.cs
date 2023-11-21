using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private InventorySlot originalSlot;
    private ThirdPersonController controller;
    private Image itemImage; // Image of the item being dragged
    private Canvas canvas;
    private GameObject itemPrefab;
    public RectTransform inventoryPanel;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalSlot = GetComponentInParent<InventorySlot>();
        controller = FindObjectOfType<ThirdPersonController>();
        itemImage = GetComponent<Image>();
        canvas = FindCanvasInParent();
    }

    public void SetItemPrefab(GameObject prefab)
    {
        itemPrefab = prefab;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemImage.raycastTarget = false; // Disable raycast so it doesn't interfere with drop
        canvasGroup.blocksRaycasts = false; // Allow events to pass through the dragged item
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemImage.raycastTarget = true; // Re-enable raycast
        canvasGroup.blocksRaycasts = true;

        // Use a raycast to check if the end drag position is over the inventory UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;

            // Check if the GameObject we're over is part of the inventory UI
            if (hitObject != null && hitObject.transform.IsChildOf(inventoryPanel.transform))
            {
                // It's over the inventory UI, so handle as a regular item move within the inventory
                if (hitObject.GetComponent<InventorySlot>() != null)
                {
                    InventorySlot newSlot = hitObject.GetComponent<InventorySlot>();
                    DroppedOnSlot(newSlot);
                }
                else
                {
                    // Snap back to the original slot if it's not dropped on a valid slot
                    rectTransform.anchoredPosition = originalSlot.GetComponent<RectTransform>().anchoredPosition;
                }
            }
            else
            {
                // Snap back to the original slot if dropped on some other UI element that's not the inventory
                rectTransform.anchoredPosition = originalSlot.GetComponent<RectTransform>().anchoredPosition;
            }
        }
        else
        {
            // Dropped outside any UI element, including the inventory UI
            DropItemOutsideInventory();
        }
    }

    private Canvas FindCanvasInParent()
    {
        return GetComponentInParent<Canvas>();
    }

    public void DroppedOnSlot(InventorySlot newSlot)
    {
        if (originalSlot != newSlot)
        {
            controller.SwapItems(originalSlot.slotIndex, newSlot.slotIndex);
            originalSlot = newSlot; // Update the original slot reference
        }

        rectTransform.anchoredPosition = newSlot.GetComponent<RectTransform>().anchoredPosition;
    }

    private void DropItemOutsideInventory()
    {
        // Assuming itemPrefab is correctly set
        if (itemPrefab != null)
        {
            Vector3 dropPosition = controller.transform.position + controller.transform.forward; // Drop in front of the player
            Instantiate(itemPrefab, dropPosition, Quaternion.identity);
        }

        controller.RemoveItemFromInventory(originalSlot.slotIndex);
    }
}
