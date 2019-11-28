using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] Text ItemNameText;
    [SerializeField] Text ItemSlotText;
    [SerializeField] Text ItemStatText;

    private StringBuilder stringBuilder = new StringBuilder();

    private void OnValidate()
    {
        gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ShowTooltip(EquippableItem item)
    {
        ItemNameText.text = item.ItemName;
        ItemSlotText.text = item.equipmentType.ToString();

        stringBuilder.Length = 0;
        AddStat(item.StrengthBonus, "Strength");
        AddStat(item.IntelligenceBonus, "Intelligence");
        AddStat(item.AgilityBonus, "Agility");
        AddStat(item.VitalityBonus, "Vitality");

        ItemStatText.text = stringBuilder.ToString();

        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    private void AddStat(float value, string statName)
    {
        if(value != 0)
        {
            if (stringBuilder.Length > 0)
                stringBuilder.AppendLine();
            if (value > 0)
                stringBuilder.Append("+");
            stringBuilder.Append(value);
            stringBuilder.Append(" ");
            stringBuilder.Append(statName);
        }
    }

}
