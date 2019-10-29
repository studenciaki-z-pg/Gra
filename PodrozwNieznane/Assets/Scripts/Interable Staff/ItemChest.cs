using UnityEngine;
using Random = System.Random;

public class ItemChest : InterableObject
{
    [SerializeField] EquippableItem item;

    ItemChest itemChestPrefab;

    private void Start()
    {
        item = RandomItemFromList();
    }

    override
    public void FinallySomeoneFoundMe()
    {
        Debug.Log("Your princess is in another, another castle. But take that "+ item.name +" maybe it will be helpful.");
        GameManager.instance.players[GameManager.instance.activePlayer].Character.GetInventory().AddItem(item);
    }


    public EquippableItem RandomItemFromList()
    {
        EquippableItem luckyOne = new EquippableItem();
        Random rand = new Random();
        luckyOne = GameManager.instance.ListOfItems[rand.Next(GameManager.instance.ListOfItems.Length)];
        return luckyOne;
    }

}
