using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // Assign this in the inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ThirdPersonController player = other.GetComponent<ThirdPersonController>();
            if (player != null)
            {
                player.PickupItem(item);
                Destroy(gameObject); // Remove the item from the scene
            }
        }
    }

}