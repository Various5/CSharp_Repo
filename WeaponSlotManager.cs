using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotManager : MonoBehaviour
{
    [SerializeField] private Image primaryWeaponSlot;
    [SerializeField] private Image secondaryWeaponSlot;
    // Additional UI elements as needed, e.g., Text for ammo count

    // Call this method to update the primary weapon slot
    public void UpdatePrimaryWeaponSlot(Weapon weapon)
    {
        if (weapon != null)
        {
            primaryWeaponSlot.sprite = weapon.weaponSprite;
            // If you have text for ammo count or weapon name
            // primaryWeaponSlotText.text = weapon.ammoCount.ToString();
            // You can also enable or change other UI elements here
        }
        else
        {
            // Handle the case when there is no primary weapon
            primaryWeaponSlot.sprite = null; // Assign a default or empty sprite
            // primaryWeaponSlotText.text = "";
        }
    }

    // Call this method to update the secondary weapon slot
    public void UpdateSecondaryWeaponSlot(Weapon weapon)
    {
        if (weapon != null)
        {
            secondaryWeaponSlot.sprite = weapon.weaponSprite;
            // Update other UI elements as needed
        }
        else
        {
            // Handle the case when there is no secondary weapon
            secondaryWeaponSlot.sprite = null; // Assign a default or empty sprite
        }
    }
}

// Example weapon class
public class Weapon
{
    public Sprite weaponSprite;
    public int ammoCount;
    // Add other weapon properties as needed
}
