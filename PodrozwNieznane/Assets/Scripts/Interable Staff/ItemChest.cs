using UnityEngine;
using Random = System.Random;

public class ItemChest : InterableObject
{
    [SerializeField] EquippableItem item;

    private ItemList ItemList;

    ItemChest itemChestPrefab;

    private void Start()
    {
        ItemList = FindObjectOfType<ItemList>();
    }

    override
    public void FinallySomeoneFoundMe()
    {
        //RandomItemFromList();
        //Debug.Log("Your princess is in another, another castle. But take that "+ item.name +" maybe it will be helpful.");
    }


    public void RandomItemFromList()
    {
        item = new EquippableItem();
        Random rand = new Random();
        //item = ItemList.items[rand.Next(ItemList.items.Count)];

    }
    public void RandomItem()
    {

    }

}
