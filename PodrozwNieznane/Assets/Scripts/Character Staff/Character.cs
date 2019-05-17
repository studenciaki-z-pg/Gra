using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public int HP = 0;
    public CharacterStats Strength = new CharacterStats(1);
    public CharacterStats Intelligence = new CharacterStats(1);
    public CharacterStats Agility = new CharacterStats(1);
    public CharacterStats Vitality = new CharacterStats(1);
    public CharacterStats Dexterity = new CharacterStats(1);
    public CharacterStats Luck = new CharacterStats(1);

    [SerializeField] Inventory inventory;
    [SerializeField] EquimentPanel equipmentPanel;
    [SerializeField] StatPanel[] statPanel;

    private int Player = -1;


    private void OnValidate()
    {
        for (int i = 0; i < statPanel.Length; i++)
        {
            statPanel[i].SetStats(Strength, Intelligence, Agility, Vitality, Dexterity, Luck);
            statPanel[i].UpdateStatValues();
        }
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        for (int i = 0; i < statPanel.Length; i++)
        {
            statPanel[i].SetStats(Strength, Intelligence, Agility, Vitality, Dexterity, Luck);
            statPanel[i].UpdateStatValues();
        }

        inventory.OnItemRightClickedEvent += EquipFromInventory;
        equipmentPanel.OnItemRightClickedEvent += UnequipFromEquipment;
    }

    public void TurnVisible(int player)
    {
        if(gameObject.active)
        {
            gameObject.SetActive(false);
        } else
        {
            if(Player != player)
            {
                Player = player;
                ChangePlayer();
            }
            gameObject.SetActive(true);
        }
    }

    private void ChangePlayer()
    {

    }
    
    private void EquipFromInventory(Item item)
    {
        if(item is EquippableItem)
        {
            Equip((EquippableItem)item);
        }
    }

    private void UnequipFromEquipment(Item item)
    {
        if (item is EquippableItem)
        {
            Unequip((EquippableItem)item);
        }
    }

    public void Equip(EquippableItem item)
    {
        if(inventory.RemoveItem(item))
        {
            EquippableItem previousItem;
            if(equipmentPanel.AddItem(item, out previousItem))
            {
                if(previousItem != null)
                {
                    inventory.AddItem(previousItem);
                    previousItem.Unequip(this);
                    for (int i = 0; i < statPanel.Length; i++)
                        statPanel[i].UpdateStatValues();
                }
                item.Equip(this);
                for(int i=0; i<statPanel.Length; i++)
                    statPanel[i].UpdateStatValues();
            }
            else
            {
                inventory.AddItem(item);
            }
        }
    }

    public void Unequip(EquippableItem item)
    {
        if(!inventory.IsFull() && equipmentPanel.RemoveItem(item))
        {
            item.Unequip(this);
            for (int i = 0; i < statPanel.Length; i++)
                statPanel[i].UpdateStatValues();
            inventory.AddItem(item);
        }
    }
}
