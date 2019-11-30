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
        ItemSlotText.text = item.equipmentType.GetEquipmentTypeName();//.ToString();

        stringBuilder.Length = 0;
        AddStat(item.StrengthBonus, "Siła");
        AddStat(item.IntelligenceBonus, "Inteligencja");
        AddStat(item.AgilityBonus, "Zręczność");
        AddStat(item.VitalityBonus, "Szybkość");

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
