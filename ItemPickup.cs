using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // Assigned in the Unity Inspector


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (item == null)
            {
                Debug.LogError("Item is null on " + gameObject.name);
                return;
            }

            ThirdPersonController player = other.GetComponent<ThirdPersonController>();
            if (player != null)
            {
                player.PickupItem(item);
                Destroy(gameObject);
            }


        }
    }

}
