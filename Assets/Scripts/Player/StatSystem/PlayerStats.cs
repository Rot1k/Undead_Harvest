using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

public sealed class PlayerStats : IDisposable
{
    private readonly PlayerStatsSO _baseStats;
    private readonly StatusEffectsListSO _effects;

    private readonly Dictionary<StatType, Stat> _stats = new();
    private readonly Dictionary<StatType, Action<float>> _statHandlers = new();
    private readonly Dictionary<StatType, ReactiveProperty<float>> _statValues = new();

    public PlayerStatsSO StatsSO => _baseStats;

    public PlayerStats(PlayerStatsSO baseStats, StatusEffectsListSO effects)
    {
        _baseStats = baseStats;
        _effects = effects;
        Initialize();
    }

    public void Initialize()
    {
        Dispose();

        if (_baseStats == null)
        {
            Debug.LogWarning("Player base stats are not configured.");
            return;
        }

        if (_baseStats.Stats == null)
        {
            Debug.LogWarning("Player base stats list is not configured.");
            return;
        }

        foreach (var entry in _baseStats.Stats)
        {
            if (entry == null)
                continue;

            var stat = new Stat(entry.Type, entry.BaseValue); // Stat now knows its own type
            var statValue = new ReactiveProperty<float>(stat.CurrentValue);
            Action<float> handler = _ => statValue.Value = stat.CurrentValue;
            stat.OnValueChanged += handler;

            _stats[entry.Type] = stat;
            _statHandlers[entry.Type] = handler;
            _statValues[entry.Type] = statValue;
        }
    }

    public void Dispose()
    {
        foreach (var kv in _statHandlers)
        {
            if (_stats.TryGetValue(kv.Key, out var stat))
            {
                stat.OnValueChanged -= kv.Value;
            }
        }

        foreach (var statValue in _statValues.Values)
        {
            statValue.Dispose();
        }

        _statHandlers.Clear();
        _stats.Clear();
        _statValues.Clear();
    }

    public void SetBaseStat(StatType type, float value)
    {
        if (_stats.TryGetValue(type, out var stat))
        {
            stat.SetBase(value);
        }
        else
        {
            Debug.LogWarning($"Stat {type} not found.");
        }
    }

    public Guid ApplyModifier(StatType type, StatModifier modifier)
    {
        if (_stats.TryGetValue(type, out var stat))
        {
            stat.AddModifier(modifier);
            return modifier.SourceId;
        }

        Debug.LogWarning($"Stat {type} not found.");
        return Guid.Empty;
    }

    public void RemoveModifiersFromSource(StatType type, Guid sourceId)
    {
        if (_stats.TryGetValue(type, out var stat))
        {
            stat.RemoveModifiersFromSource(sourceId);
        }
        else
        {
            Debug.LogWarning($"Stat {type} not found.");
        }
    }

    public float Get(StatType type)
    {
        if (_stats.TryGetValue(type, out var stat))
            return stat.CurrentValue;

        throw new KeyNotFoundException($"Stat {type} not found on Player.");
    }

    public ReadOnlyReactiveProperty<float> Observe(StatType type)
    {
        if (_statValues.TryGetValue(type, out var statValue))
            return statValue;

        throw new KeyNotFoundException($"Stat {type} not found on Player.");
    }

    public void TryApplyEffects(Enemy enemy)
    {
        if (enemy == null || _effects == null || _effects.StatusEffects == null)
            return;

        foreach (var effect in _effects.StatusEffects)
        {
            if (effect == null ||
                !_stats.TryGetValue(effect.LinkedChanceStat, out var chanceStat) ||
                !_stats.TryGetValue(StatType.GlobalEffectChanceMultiplier, out var chanceMultiplierStat))
            {
                continue;
            }

            float finalChance = chanceStat.CurrentValue / 100f * chanceMultiplierStat.CurrentValue;
            if (UnityEngine.Random.value <= finalChance)
            {
                enemy.ApplyEffect(effect);
            }
        }
    }
}
