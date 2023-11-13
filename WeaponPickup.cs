using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab; // Prefab of the weapon to add to inventory
    public bool isMeleeWeapon;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure your player GameObject has the tag "Player"
        {
            FirstPersonController playerController = other.GetComponent<FirstPersonController>();
            if (playerController != null)
            {
                playerController.PickupWeapon(weaponPrefab, isMeleeWeapon);
                Destroy(gameObject); // Destroy the pickup item
            }
        }
    }
}
