using UnityEngine;

public class ItemChest : InterableObject
{
    [SerializeField] Item item;

    ItemChest itemChestPrefab;

    override
    public void FinallySomeoneFoundMe()
    {

        Debug.Log("Your princess is in another, another castle. But take that gem maybe it will be helpful.");
    }


    public void RandomItem()
    {
        item = new Item();


        
    }

}
