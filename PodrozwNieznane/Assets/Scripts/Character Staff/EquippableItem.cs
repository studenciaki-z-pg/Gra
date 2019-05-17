﻿using UnityEngine;

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
    public int IntelligenceBonus;
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
        if (DexterityBonus != 0)
            c.Dexterity.AddModifier(new StatModifier(DexterityBonus, StatModType.Flat, this));
        if (LuckBonus != 0)
            c.Luck.AddModifier(new StatModifier(LuckBonus, StatModType.Flat, this));

        if (StrengthPercentBonus != 0)
            c.Strength.AddModifier(new StatModifier(StrengthPercentBonus, StatModType.PercentMult, this));
        if (IntelligencePercentBonus != 0)
            c.Intelligence.AddModifier(new StatModifier(IntelligencePercentBonus, StatModType.PercentMult, this));
        if (AgilityPercentBonus != 0)
            c.Agility.AddModifier(new StatModifier(AgilityPercentBonus, StatModType.PercentMult, this));
        if (VitalityPercentBonus != 0)
            c.Vitality.AddModifier(new StatModifier(VitalityPercentBonus, StatModType.PercentMult, this));
        if (DexterityPercentBonus != 0)
            c.Dexterity.AddModifier(new StatModifier(DexterityPercentBonus, StatModType.PercentMult, this));
        if (LuckPercentBonus != 0)
            c.Luck.AddModifier(new StatModifier(LuckPercentBonus, StatModType.PercentMult, this));
    }

    public void Unequip(Character c)
    {
        c.Strength.RemoveAllModifiersFromSource(this);
        c.Intelligence.RemoveAllModifiersFromSource(this);
        c.Agility.RemoveAllModifiersFromSource(this);
        c.Vitality.RemoveAllModifiersFromSource(this);
        c.Dexterity.RemoveAllModifiersFromSource(this);
        c.Luck.RemoveAllModifiersFromSource(this);
    }
}
