using UnityEngine;

public enum EquipmentType
{
    Helmet,
    Chest,
    //Gloves,
    Boots,
    Weapon1,
    Weapon2,
    Accessory1,
    Accessory2,
}

public static class Extension
{
    public static string GetEquipmentTypeName(this EquipmentType type)
    {
        //Tu jest trochę magii, bo zwykły ToString() bierze nazwę enuma, który nie może mieć spacji
        //A chcemy mieć ładne spacje i nie chcemy psuć angielskich nazw enuma
        switch (type)
        {
            case EquipmentType.Helmet: return "Hełm";
            case EquipmentType.Chest: return "Duży przedmiot";
            case EquipmentType.Boots: return "Buty";
            case EquipmentType.Weapon1: return "Pirwsza Broń";
            case EquipmentType.Weapon2: return "Druga Broń";
            case EquipmentType.Accessory1: return "Akcesorium po prawej";
            case EquipmentType.Accessory2: return "Akcesorium po lewej";
        }
        return string.Empty;
    }
}


[CreateAssetMenu]
public class EquippableItem : Item
{
    public int StrengthBonus;
    public int IntelligenceBonus;
    public int AgilityBonus;
    public int VitalityBonus;
    [Space]
    public float StrengthPercentBonus;
    public float IntelligencePercentBonus;
    public float AgilityPercentBonus;
    public float VitalityPercentBonus;
    [Space]
    public EquipmentType equipmentType;

    public void Equip(Character c)
    {
        if (StrengthBonus != 0)
            c.Strength.AddModifier(new StatModifier(StrengthBonus, StatModType.Flat, this));
        if (IntelligenceBonus != 0)
            c.Intelligence.AddModifier(new StatModifier(IntelligenceBonus, StatModType.Flat, this));
        if (AgilityBonus != 0)
            c.Agility.AddModifier(new StatModifier(AgilityBonus, StatModType.Flat, this));
        if (VitalityBonus != 0)
            c.Vitality.AddModifier(new StatModifier(VitalityBonus, StatModType.Flat, this));

        if (StrengthPercentBonus != 0)
            c.Strength.AddModifier(new StatModifier(StrengthPercentBonus, StatModType.PercentMult, this));
        if (IntelligencePercentBonus != 0)
            c.Intelligence.AddModifier(new StatModifier(IntelligencePercentBonus, StatModType.PercentMult, this));
        if (AgilityPercentBonus != 0)
            c.Agility.AddModifier(new StatModifier(AgilityPercentBonus, StatModType.PercentMult, this));
        if (VitalityPercentBonus != 0)
            c.Vitality.AddModifier(new StatModifier(VitalityPercentBonus, StatModType.PercentMult, this));
    }

    public void Unequip(Character c)
    {
        c.Strength.RemoveAllModifiersFromSource(this);
        c.Intelligence.RemoveAllModifiersFromSource(this);
        c.Agility.RemoveAllModifiersFromSource(this);
        c.Vitality.RemoveAllModifiersFromSource(this);
    }
}
