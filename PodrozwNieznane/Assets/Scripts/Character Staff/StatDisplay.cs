using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] StatTooltip statTooltip;
    [SerializeField] Text nameText;
    [SerializeField] Text valueText;

    public CharacterStats Stat
    {
        get { return _stat; }
        set
        {
            _stat = value;
            UpdateStatValue();
        }
    }

    public string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            NameText.text = _name;
        }
    }

    private void OnValidate()
    {
        Text[] texts = GetComponentsInChildren<Text>();
        NameText = texts[0];
        valueText = texts[1];

        if (statTooltip == null)
            statTooltip = FindObjectOfType<StatTooltip>();
    }

    public Text NameText { get => nameText; set => nameText = value; }

    private string _name;
    private CharacterStats _stat;

    public void OnPointerEnter(PointerEventData eventData)
    {
        statTooltip.ShowTooltip(Stat, Name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        statTooltip.HideTooltip();
    }

    public void UpdateStatValue()
    {
        valueText.text = _stat.Value.ToString();

    }
}
