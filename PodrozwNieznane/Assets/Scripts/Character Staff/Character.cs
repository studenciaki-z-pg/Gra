using UnityEngine;
using UnityEngine.UI;
public class Character : MonoBehaviour
{
    
    public CharacterStats Strength = new CharacterStats(10); //siłą
    public CharacterStats Intelligence = new CharacterStats(10); //inteligencja
    public CharacterStats Agility = new CharacterStats(10); //zręczność
    public CharacterStats Vitality = new CharacterStats(10); //wytrzymałość ~~ punkty ruchu

    [SerializeField] Inventory inventory;
    [SerializeField] EquimentPanel equipmentPanel;
    [SerializeField] StatPanel[] statPanel;
    [SerializeField] Button[] statButtons;

    [SerializeField] Sprite activeSprite;
    [SerializeField] Sprite inactiveSprite;
    
    private int PlayerLevel = 1;
    private int AvailableSkillPoints = 5;
    private int AssignedSkillPoints = 0;

    private void OnValidate()
    {
        for (int i = 0; i < statPanel.Length; i++)
        {
            statPanel[i].SetStats(Strength, Intelligence, Agility, Vitality);
            statPanel[i].UpdateStatValues();
        }
        if(AvailableSkillPoints < 1)
            foreach(Button b in statButtons)
            {
                b.enabled = false;
                b.gameObject.SetActive(false);
                //b.image.sprite = inactiveSprite;
            }
        foreach (StatPanel panel in statPanel)
        {
            panel.SetCharacterLevel(PlayerLevel);
        }
        gameObject.SetActive(false);

    }

    public void BonusSkillPoints(int value)
    {
        AvailableSkillPoints += value;
    }

    public void SetCharacterName(string newName)
    {
        foreach (StatPanel panel in statPanel)
        {
            panel.SetCharacterName(newName);
        }
    }

    public int getLevel()
    {
        return PlayerLevel;
    }

    public void LevelUp()
    {
        GameManager.instance.LevelUp();
        PlayerLevel++;
        AvailableSkillPoints += 5;
        foreach (Button b in statButtons)
        {
            //b.image.sprite = activeSprite;
            b.gameObject.SetActive(true);
            b.enabled = true;
        }
        foreach (StatPanel panel in statPanel)
        {
            panel.SetCharacterLevel(PlayerLevel);
        }
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    private void Awake()
    {
        for (int i = 0; i < statPanel.Length; i++)
        {
            statPanel[i].SetStats(Strength, Intelligence, Agility, Vitality);
            statPanel[i].UpdateStatValues();
        }

        inventory.OnItemRightClickedEvent += EquipFromInventory;
        equipmentPanel.OnItemRightClickedEvent += UnequipFromEquipment;
    }

    public void TurnVisible()
    {
        //czy otwarte eq? tak, to zamknij, nie to otworz
        if(gameObject.active)
        {
            gameObject.SetActive(false);
            GameManager.instance.hexGameUI.Highlighting(true);
            GameManager.instance.Interacting = false;

        } else
        {
            GameManager.instance.Interacting = true;
            GameManager.instance.hexGameUI.Highlighting(false);
            gameObject.SetActive(true);
        }
    }

    public int SpeedValue()
    {
        return 5 + (int)(this.Vitality.Value / 5);
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
    public void AddStrengthPoint()
    {
        if (AvailableSkillPoints > 0)
        {
            AvailableSkillPoints--;
            AssignedSkillPoints++;
            Strength.BaseValue++;
            for (int i = 0; i < statPanel.Length; i++)
            {
                statPanel[i].UpdateStatValues();
            }
            if(AvailableSkillPoints == 0)
                foreach (Button b in statButtons)
                {
                    //b.image.sprite = inactiveSprite;
                    b.gameObject.SetActive(false);
                    b.enabled = false;
                }
        }
    }
    public void AddIntelligencePoint()
    {
        if (AvailableSkillPoints > 0)
        {
            AvailableSkillPoints--;
            AssignedSkillPoints++;
            Intelligence.BaseValue++;
            for (int i = 0; i < statPanel.Length; i++)
                statPanel[i].UpdateStatValues();
            if (AvailableSkillPoints == 0)
                foreach (Button b in statButtons)
                {
                    //b.image.sprite = inactiveSprite;
                    b.gameObject.SetActive(false);
                    b.enabled = false;
                }
        }
    }
    public void AddAgilityPoint()
    {
        if (AvailableSkillPoints > 0)
        {
            AvailableSkillPoints--;
            AssignedSkillPoints++;
            Agility.BaseValue++;
            for (int i = 0; i < statPanel.Length; i++)
                statPanel[i].UpdateStatValues();
            if (AvailableSkillPoints == 0)
                foreach (Button b in statButtons)
                {
                    //b.image.sprite = inactiveSprite;
                    b.gameObject.SetActive(false);
                    b.enabled = false;
                }
        }
    }
    public void AddVitalityPoint()
    {
        if (AvailableSkillPoints > 0)
        {
            AvailableSkillPoints--;
            AssignedSkillPoints++;
            Vitality.BaseValue++;
            for (int i = 0; i < statPanel.Length; i++)
                statPanel[i].UpdateStatValues();
            if (AvailableSkillPoints == 0)
                foreach (Button b in statButtons)
                {
                    //b.image.sprite = inactiveSprite;
                    b.gameObject.SetActive(false);
                    b.enabled = false;
                }
        }
    }

}
