using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StatTooltip : MonoBehaviour
{
    [SerializeField] Text StatNameText;
    [SerializeField] Text StatModifiersLabelText;
    [SerializeField] Text StatModifiersText;

    private StringBuilder stringBuilder = new StringBuilder();

    private void OnValidate()
    {
       gameObject.SetActive(false);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    public void ShowTooltip(CharacterStats stat, string statName)
    {
        StatNameText.text = GetStatTopText(stat, statName);

        StatModifiersText.text = GetStatModifiersText(stat);

        gameObject.SetActive(true);
    }

    private string GetStatModifiersText(CharacterStats stat)
    {
        stringBuilder.Length = 0;

        foreach(StatModifier mod in stat.StatModifiers)
        {
            if (mod.Value > 0)
                stringBuilder.Append("+");
            stringBuilder.Append(mod.Value);

            EquippableItem item = mod.Source as EquippableItem;

            if(item != null)
            {
                stringBuilder.Append(" ");
                stringBuilder.AppendLine(item.ItemName);
            }
            else
            {
                Debug.LogError("Problem z brakiem używalności - StatToolTip Class - GetStatModufuersText");
            }
        }


        return stringBuilder.ToString();
    }

    private string GetStatTopText(CharacterStats stat, string statName)
    {
        stringBuilder.Length = 0;
        stringBuilder.Append(statName);
        stringBuilder.Append(" ");
        stringBuilder.Append(stat.Value);
        stringBuilder.Append(" (");
        stringBuilder.Append(stat.BaseValue);

        if (stat.Value-stat.BaseValue != 0)
        {
            if (stat.Value > stat.BaseValue)
                stringBuilder.Append("+");

            stringBuilder.Append(stat.Value - stat.BaseValue);
        }
        stringBuilder.Append(")");

        return stringBuilder.ToString();
    }
}

