using UnityEngine;

public class ItemChest : InterableObject
{
    [SerializeField] Item item;
    [SerializeField] Inventory inventory;

    ItemChest itemChestPrefab;
    

    public void RandomItem()
    {
        item = new Item();


        
    }

}
