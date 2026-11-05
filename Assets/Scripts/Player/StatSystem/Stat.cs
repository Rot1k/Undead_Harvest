using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
public class Stat : IDisposable
{
    public StatType Type { get; private set; }
    public float BaseValue { get; private set; }

    private readonly List<StatModifier> _modifiers = new();
    private readonly ReactiveProperty<float> _currentValue;
    private float _cachedValue;

    public ReadOnlyReactiveProperty<float> CurrentValueStream => _currentValue;

    public Stat(StatType type, float baseValue)
    {
        Type = type;
        BaseValue = baseValue;
        _currentValue = new ReactiveProperty<float>(0f);
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
        if (mod == null)
            return;

        _modifiers.Add(mod);
        Recalculate();
    }

    public void RemoveModifiersFromSource(Guid sourceId)
    {
        _modifiers.RemoveAll(m => m.SourceId == sourceId);
        Recalculate();
    }

    public void Dispose()
    {
        _currentValue.Dispose();
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

        value *= 1f + addPercent;
        value *= multiplier;

        float newValue = value;

        if (Math.Abs(newValue - _cachedValue) > 0.0001f)
        {
            _cachedValue = newValue;
            _currentValue.Value = CurrentValue;
        }
    }

    private bool CanBeNegative()
    {
        return Type == StatType.AttackDamage || Type == StatType.MeleeDamage || Type == StatType.RangedDamage;
    }
}