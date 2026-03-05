using UnityEngine;
using System;
using System.Collections.Generic;

public partial class WeaponSO : InventoryItemSO
{
    public Guid Id { get; } = Guid.NewGuid();

    [Serializable]
    public class WeaponEffectData
    {
        public StatusEffectSO Effect;
        [Range(0, 1)] public float Chance;
    }
    [Serializable]
    public class ScalingStatsEntry
    {
        public StatType ScalingStatType;
        public float ScalingFactor;
    }

    public GameObject Prefab;
    public Sprite MaskSprite;
    public float BaseDamage;
    public float RangeMultiplier;
    public float BaseAttackSpeed;

    [Header("Upgrade")]
    [SerializeField] private WeaponSO _unionResult;
    public WeaponSO UnionResult => _unionResult;
    public bool CanUnion => _unionResult != null;

    [Header("ScalingStats")]
    [SerializeField] private List<ScalingStatsEntry> _scalingStats;
    public IReadOnlyList<ScalingStatsEntry> ScalingStats => _scalingStats;

    [Header("Effects On Hit")]
    [SerializeField] private List<WeaponEffectData> _onHitEffects;
    public IReadOnlyList<WeaponEffectData> OnHitEffects => _onHitEffects;

}
