using UnityEngine;
using System.Collections.Generic;

public partial class WeaponSO : InventoryItemSO
{
    [System.Serializable]
    public class WeaponEffectData
    {
        public StatusEffectSO Effect;
        [Range(0, 1)] public float Chance;
    }
    [System.Serializable]
    public class ScalingStatsEntry
    {
        public StatType ScalingStatType;
        [Min(0)]
        public float ScalingFactor;
    }

    public GameObject Prefab;
    public Sprite MaskSprite;
    [Min(0)]
    public float BaseDamage;
    [Min(0.01f)]
    public float RangeMultiplier;
    [Min(0.01f)]
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

    protected virtual void OnValidate()
    {
        if (_scalingStats == null)
            _scalingStats = new List<ScalingStatsEntry>();
        else
            _scalingStats.RemoveAll(entry => entry == null);

        if (_onHitEffects == null)
            _onHitEffects = new List<WeaponEffectData>();
        else
            _onHitEffects.RemoveAll(effect => effect == null || effect.Effect == null);

        if (_unionResult == this)
            _unionResult = null;
    }

}
