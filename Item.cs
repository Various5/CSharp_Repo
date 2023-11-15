using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public int itemID;
    public GameObject itemPrefab;
    public bool isStackable;
    public int stackSize;
    public Sprite itemImage; // Sprite to represent the item in the UI

    public Item(string name, int id, GameObject prefab, bool stackable, int stack = 0, Sprite image = null)
    {
        itemName = name;
        itemID = id;
        itemPrefab = prefab;
        isStackable = stackable;
        stackSize = stack;
        itemImage = image;
    }
}
