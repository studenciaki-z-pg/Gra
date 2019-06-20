using UnityEngine;

public class ItemChest : InterableObject
{
    [SerializeField] Item item;
    [SerializeField] ItemList items;

    ItemChest itemChestPrefab;

    private void Start()
    {
        
    }

    override
    public void FinallySomeoneFoundMe()
    {

        Debug.Log("Your princess is in another, another castle. But take that dżem maybe it will be helpful.");
    }


    public void RandomItem()
    {
        item = new Item();


        
    }

}
