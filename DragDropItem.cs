using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private InventorySlot originalSlot;
    private ThirdPersonController controller; // Reference to inventory controller

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalSlot = GetComponentInParent<InventorySlot>();
        controller = FindObjectOfType<ThirdPersonController>(); // Find the inventory controller
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Canvas canvas = FindCanvasInParent();
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // Adjust for canvas scale
        }
    }

    private Canvas FindCanvasInParent()
    {
        return GetComponentInParent<Canvas>();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (originalSlot == null)
        {
            Debug.LogError("originalSlot is null in OnEndDrag");
            return; // Prevent NullReferenceException
        }

        if (controller == null)
        {
            Debug.LogError("controller is null in OnEndDrag");
            return; // Prevent NullReferenceException
        }
        
        RectTransform slotRectTransform = originalSlot.GetComponent<RectTransform>();
        if (slotRectTransform != null)
        {
            rectTransform.anchoredPosition = slotRectTransform.anchoredPosition; // Snap to the slot
        }
        else
        {
            Debug.LogError("RectTransform on originalSlot is null");
        }

    }

        public void DroppedOnSlot(InventorySlot newSlot)
    {
        if (originalSlot == newSlot)
        {
            // Item dropped back to its original slot
            rectTransform.anchoredPosition = originalSlot.GetComponent<RectTransform>().anchoredPosition;
        }
        else
        {
            // Swap logic with the new slot
            controller.SwapItems(originalSlot.GetSlotIndex(), newSlot.GetSlotIndex());
            originalSlot = newSlot; // Update the original slot reference
            rectTransform.anchoredPosition = newSlot.GetComponent<RectTransform>().anchoredPosition;
        }
    }
}
