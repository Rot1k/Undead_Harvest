using UnityEngine;
using System;

public partial class WeaponSO : InventoryItemSO
{
    public Guid Id { get; } = Guid.NewGuid();

    public GameObject Prefab;
    public Sprite MaskSprite;
    public float BaseDamage;
    public float RangeMultiplier;
    public StatType[] ScalingStats;
    public float BaseAttackSpeed;
    [Range(0, 1)]public float ScalingFactor;

    [Header("Upgrade")]
    [SerializeField] private WeaponSO _unionResult;
    public WeaponSO UnionResult => _unionResult;
    public bool CanUnion => _unionResult != null;

}
