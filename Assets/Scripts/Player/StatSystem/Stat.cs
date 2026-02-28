using System;
using System.Collections.Generic;
using UnityEngine;
public class Stat
{
    public event Action<float> OnValueChanged;

    public StatType Type { get; private set; }
    public float BaseValue { get; private set; }

    private readonly List<StatModifier> _modifiers = new();
    private float _cachedValue;

    public Stat(StatType type, float baseValue)
    {
        Type = type;
        BaseValue = baseValue;
        Recalculate();
    }

    public float CurrentValue
    {
        get
        {
            float value = _cachedValue;

            if (!float.IsFinite(value))
                value = 0f;

            if (!CanBeNegative())
                value = Mathf.Max(0f, value);

            return value;
        }
    }

    public void SetBase(float baseValue)
    {
        if (!CanBeNegative() && baseValue < 0f)
        {
            Debug.LogWarning($"Attempted to set negative base value for {Type}. Clamped to 0.");
            baseValue = 0f;
        }

        BaseValue = baseValue;
        Recalculate();
    }

    public void AddModifier(StatModifier mod)
    {
        _modifiers.Add(mod);
        Recalculate();
    }

    public void RemoveModifiersFromSource(Guid sourceId)
    {
        _modifiers.RemoveAll(m => m.SourceId == sourceId);
        Recalculate();
    }

    private void Recalculate()
    {
        float value = BaseValue;
        float addPercent = 0f;
        float multiplier = 1f;

        foreach (var m in _modifiers)
        {
            switch (m.Type)
            {
                case ModifierType.Flat:
                    value += m.Value;
                    break;
                case ModifierType.AddPercent:
                    addPercent += m.Value;
                    break;
                case ModifierType.Multiplier:
                    multiplier *= m.Value;
                    break;
            }
        }

        float newValue = value + (value * addPercent) * multiplier;

        if (Math.Abs(newValue - _cachedValue) > 0.0001f)
        {
            _cachedValue = newValue;
            OnValueChanged?.Invoke(_cachedValue);
        }
    }

    private bool CanBeNegative()
    {
        return Type == StatType.AttackDamage || Type == StatType.MeleeDamage || Type == StatType.RangedDamage;
    }
}