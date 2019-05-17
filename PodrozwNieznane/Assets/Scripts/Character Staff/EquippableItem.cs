using UnityEngine;

public enum EquipmentType
{
    Helmet,
    Chest,
    Gloves,
    Boots,
    Weapon1,
    Weapon2,
    Accessory1,
    Accessory2,
}


[CreateAssetMenu]
public class EquippableItem : Item
{
    public int StrengthBonus;
    public int InelligenceBonus;
    public int AgilityBonus;
    public int VitalityBonus;
    public int DexterityBonus;
    public int LuckBonus;
    [Space]
    public float StrengthPercentBonus;
    public float IntelligencePercentBonus;
    public float AgilityPercentBonus;
    public float VitalityPercentBonus;
    public float DexterityPercentBonus;
    public float LuckPercentBonus;
    [Space]
    public EquipmentType equipmentType;
}
