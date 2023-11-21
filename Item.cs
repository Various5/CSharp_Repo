using UnityEngine;
using static ThirdPersonController;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public int itemID;
    public GameObject itemPrefab;
    public bool isStackable;
    public int stackSize;
    public Sprite itemImage; // Sprite to represent the item in UI
    public ItemType itemType; // Existing field for item type
    public WeaponType weaponType; // New field for weapon type
    public int currencyValue; // New field for currency value

    // Use method (Implement functionality based on item type)
    public void Use()
    {
        // Example: If it's a consumable, apply its effect
        if (itemType == ItemType.Consumable)
        {
            Debug.Log($"{itemName} consumed!");
            // Apply effects here
        }
    }
}

[System.Serializable]
public enum ItemType
{
    Weapon,
    Consumable,
    Armor,
    // Add more types as needed
}
public enum WeaponType
{
    None,
    Pistol,
    Shotgun,
    Rifle,
    Sword,
    Axe,
    // Other types as needed
}