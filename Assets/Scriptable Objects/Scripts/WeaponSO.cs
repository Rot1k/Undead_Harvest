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

    public GameObject Prefab;
    public Sprite MaskSprite;
    public float BaseDamage;
    public float RangeMultiplier;
    public StatType[] ScalingStats;
    public float BaseAttackSpeed;
    [Range(0, 1)] public float ScalingFactor;

    [Header("Upgrade")]
    [SerializeField] private WeaponSO _unionResult;
    public WeaponSO UnionResult => _unionResult;
    public bool CanUnion => _unionResult != null;

    [Header("Effects On Hit")]
    [SerializeField] private List<WeaponEffectData> _onHitEffects;
    public IReadOnlyList<WeaponEffectData> OnHitEffects => _onHitEffects;
}
