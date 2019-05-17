using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class CharacterStats
{
    public float BaseValue;

    public float Value {
        get
        {
            if (!isActual || BaseValue != lastBaseValue)
            {
                lastBaseValue = BaseValue;
                _value = CalculateFinalValue();
                isActual = true;
            }
            return _value;
        }
        }

    private bool isActual = false;
    private float _value;
    private float lastBaseValue = float.MinValue;

    private readonly List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    public CharacterStats(float baseValue)
    {
        BaseValue = baseValue;
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }

    public void AddModifier(StatModifier mod)
    {
        statModifiers.Add(mod);
        statModifiers.Sort(CompareModifierOrder);
        isActual = false;
    }

    public int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
            return -1;
        else if (a.Order > b.Order)
            return 1;
        else
            return 0;
    }

    public bool RemoveModifier(StatModifier mod)
    {
        if(statModifiers.Remove(mod))
        {
            isActual = false;
            return true;
        }
        return false;

    }

    public bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemoved = false;
        for (int i = statModifiers.Count -1; i >= 0; i--)
        {
            if (statModifiers[i].Source == source)
            {
                statModifiers.RemoveAt(i);
                isActual = false;
                didRemoved = true;
            }
        }
        return didRemoved;
    }
    private float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0;

        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];
            if (mod.Type == StatModType.Flat)
                finalValue += mod.Value;
            else if (mod.Type == StatModType.PercentMult)
                finalValue *= 1 + mod.Value;
            else if (mod.Type == StatModType.PercentAdd)
            {
                sumPercentAdd += mod.Value;
                if (i + 1 >= statModifiers.Count || statModifiers[i+1].Type != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }

        }
        return (float)Math.Round(finalValue, 4);
    }

}
