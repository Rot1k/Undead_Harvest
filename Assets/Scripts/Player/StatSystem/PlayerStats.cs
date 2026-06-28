using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public event Action<StatType, float> OnStatChanged;
    
    [SerializeField] private PlayerStatsSO _baseStats;
    [SerializeField] private StatusEffectsListSO _effects;

    private readonly Dictionary<StatType, Stat> _stats = new();
    private readonly Dictionary<StatType, Action<float>> _statHandlers = new();

    public PlayerStatsSO StatsSO { get { return _baseStats; } }

    public void Initialize()
    {
        if (_baseStats == null)
        {
            Debug.LogWarning($"[{name}] baseStatsSet is null on PlayerStats.");
            return;
        }

        foreach (var entry in _baseStats.Stats)
        {
            var stat = new Stat(entry.Type, entry.BaseValue); // Stat now knows its own type
            Action<float> handler = (newValue) => OnStatChanged?.Invoke(entry.Type, newValue);
            stat.OnValueChanged += handler;

            _stats[entry.Type] = stat;
            _statHandlers[entry.Type] = handler;
        }
    }

    private void OnDestroy()
    {
        foreach (var kv in _statHandlers)
        {
            if (_stats.TryGetValue(kv.Key, out var stat))
            {
                stat.OnValueChanged -= kv.Value;
            }
        }
        _statHandlers.Clear();
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

        throw new KeyNotFoundException($"Stat {type} not found on {name}.");
    }
    public void TryApplyEffects(Enemy enemy)
    {
        foreach (var effect in _effects.StatusEffects)
        {
            float finalChance = Get(effect.LinkedChanceStat) / 100f * Get(StatType.GlobalEffectChanceMultiplier);
            if (UnityEngine.Random.value <= finalChance)
            {
                enemy.ApplyEffect(effect);
            }
        }
    }
}
