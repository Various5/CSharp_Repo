using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class DragDropItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject itemPrefab;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private InventorySlot originalSlot;
    private ThirdPersonController controller;
    private Image itemImage; // Image of the item being dragged
    private Canvas canvas;
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
        if (originalSlot.IsEmpty())
        {
            return;
        }
        itemImage.raycastTarget = false;
        canvasGroup.blocksRaycasts = false;
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
        itemImage.raycastTarget = true;
        canvasGroup.blocksRaycasts = true;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;
            if (hitObject != null && hitObject.transform.IsChildOf(inventoryPanel.transform))
            {
                InventorySlot newSlot = hitObject.GetComponent<InventorySlot>();
                if (newSlot != null)
                {
                    DroppedOnSlot(newSlot);
                }
                else
                {
                    SnapBackToOriginalSlot();
                }
            }
            else
            {
                SnapBackToOriginalSlot();
            }
        }
        else
        {
            DropItemOutsideInventory();
        }
    }

    private void SnapBackToOriginalSlot()
    {
        rectTransform.anchoredPosition = originalSlot.GetComponent<RectTransform>().anchoredPosition;
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

        // Smoothly snap to the new slot
        StartCoroutine(SmoothSnapToSlot(newSlot.GetComponent<RectTransform>().anchoredPosition));
    }

    private IEnumerator SmoothSnapToSlot(Vector2 targetPosition)
    {
        float time = 0;
        Vector2 startPosition = rectTransform.anchoredPosition;
        float duration = 0.2f; // Duration for snapping

        while (time < duration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }

    public InventorySlot GetOriginalSlot()
    {
        return originalSlot;
    }

    private void DropItemOutsideInventory()
    {
        Vector3 dropPosition = controller.transform.position + controller.transform.forward;
        Instantiate(itemPrefab, dropPosition, Quaternion.identity);
        controller.RemoveItemFromInventory(originalSlot.slotIndex);
    }
}
