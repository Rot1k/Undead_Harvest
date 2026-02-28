using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RarityConfigSO", menuName = "Scriptable Objects/RarityConfigSO")]
public class RarityConfigSO : ScriptableObject
{
    [System.Serializable]
    public class RarityEntry
    {
        public Rarity Rarity;
        public float StatMultiplier;
        public Color Color;
        [Range(0f, 100f)] public float DropChance;
    }
    public List<RarityEntry> Rarities;
    private Dictionary<Rarity, RarityEntry> _rarityDict;

    private void OnEnable()
    {
        _rarityDict = new Dictionary<Rarity, RarityEntry>();
        foreach (var entry in Rarities)
            _rarityDict[entry.Rarity] = entry;
    }
    private void EnsureDictionary()
    {
        if (_rarityDict != null) return;

        _rarityDict = new Dictionary<Rarity, RarityEntry>();
        foreach (var entry in Rarities)
            _rarityDict[entry.Rarity] = entry;
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

        float total = 0f;
        foreach (var r in Rarities)
            total += r.DropChance;

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
