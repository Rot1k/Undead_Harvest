using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "Scriptable Objects/PlayerStatsSO")]
public class PlayerStatsSO : ScriptableObject
{
    [Serializable]
    public class StatEntry
    {
        public string Name;
        public StatType Type;
        public float BaseValue;
        public Sprite Icon;
        public float BaseIncreaseAmount;
        public StatTag Tags;
    }

    [SerializeField] private List<StatEntry> _stats = new();
    private Dictionary<StatType, StatEntry> _statLookup;

    private void OnValidate()
    {
        _statLookup = new Dictionary<StatType, StatEntry>(_stats.Count);

        foreach (var entry in _stats)
        {
            if (!_statLookup.TryAdd(entry.Type, entry))
                Debug.LogWarning($"Duplicate stat type {entry.Type} in {name}");
        }
    }

    public float GetBaseValue(StatType type)
        => _statLookup.TryGetValue(type, out var entry) ? entry.BaseValue : 0f;

    public float GetBaseIncreaseAmount(StatType type)
        => _statLookup.TryGetValue(type, out var entry) ? entry.BaseIncreaseAmount : 0f;

    public string GetName(StatType type)
        => _statLookup.TryGetValue(type, out var entry) ? entry.Name : string.Empty;

    public Sprite GetIcon(StatType type)
        => _statLookup.TryGetValue(type, out var entry) ? entry.Icon : null;

    public List<StatEntry> Stats => _stats;
    public List<StatType> GetStatsByTag(StatTag tag)
    {
        List<StatType> result = new();

        foreach (var entry in _stats)
        {
            if (entry.Tags.HasFlag(tag))
                result.Add(entry.Type);
        }

        return result;
    }
}