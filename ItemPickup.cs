using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // Assigned in the Unity Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && item != null)
        {
            ThirdPersonController player = other.GetComponent<ThirdPersonController>();
            if (player != null)
            {
                player.PickupItem(item);
                Destroy(gameObject);
            }
        }
    }


}

