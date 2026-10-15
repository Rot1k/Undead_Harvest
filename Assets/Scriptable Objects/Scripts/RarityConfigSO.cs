using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RarityConfigSO", menuName = "Scriptable Objects/RarityConfigSO")]
public class RarityConfigSO : ScriptableObject
{
    [System.Serializable]
    public class RarityEntry
    {
        public Rarity Rarity;
        [Min(0.01f)] public float StatMultiplier;
        public Color Color;
        [Range(0f, 100f)] public float DropChance;
    }
    public List<RarityEntry> Rarities;
    private Dictionary<Rarity, RarityEntry> _rarityDict;

    private void OnEnable()
    {
        BuildDictionary();
    }

    private void OnValidate()
    {
        if (Rarities == null)
            Rarities = new List<RarityEntry>();

        Rarities.RemoveAll(entry => entry == null);
        _rarityDict = null;

        if (Rarities.Count == 0)
            Debug.LogError($"{name} must contain at least one rarity entry.", this);
        else if (Rarities.TrueForAll(entry => entry.DropChance <= 0f))
            Debug.LogError($"{name} must contain a rarity with a positive DropChance.", this);
    }

    private void BuildDictionary()
    {
        _rarityDict = new Dictionary<Rarity, RarityEntry>();
        if (Rarities == null) return;

        foreach (var entry in Rarities)
        {
            if (entry != null)
                _rarityDict[entry.Rarity] = entry;
        }
    }
    private void EnsureDictionary()
    {
        if (_rarityDict != null) return;

        BuildDictionary();
    }

    public float GetMultiplier(Rarity rarity)
    {
        EnsureDictionary();
        return _rarityDict.TryGetValue(rarity, out var entry) ? entry.StatMultiplier : 1f;
    }
        

    public Color GetColor(Rarity rarity)
    {
        EnsureDictionary();
        return _rarityDict.TryGetValue(rarity, out var entry) ? entry.Color : Color.white;
    }


    public float GetDropChance(Rarity rarity)
    {
        EnsureDictionary();
        return _rarityDict.TryGetValue(rarity, out var entry) ? entry.DropChance : 0f;
    }
    
    public RarityEntry GetRandomEntry()
    {
        EnsureDictionary();

        if (Rarities == null || Rarities.Count == 0)
            return new RarityEntry { Rarity = Rarity.Common, StatMultiplier = 1f, DropChance = 1f };

        float total = 0f;
        foreach (var r in Rarities)
            total += r.DropChance;

        if (total <= 0f)
            return new RarityEntry { Rarity = Rarity.Common, StatMultiplier = 1f, DropChance = 1f };

        float roll = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var entry in Rarities)
        {
            cumulative += entry.DropChance;
            if (roll <= cumulative)
                return entry;
        }

        return Rarities[0];
    }
}
