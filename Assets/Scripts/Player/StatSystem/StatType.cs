public enum StatType
{
    MaxHealth,
    HealthRegen,
    AttackDamage, // Unified for melee and ranged
    MeleeDamage,
    RangedDamage,
    AttackSpeedMultiplier,
    AttackRange,
    MoveSpeed,
    CriticalChance,
    CriticalDamageMultiplier,
    ItemPickupRange,
    ExperienceMultiplier,
    GlobalEffectChanceMultiplier,
    BleedChance,
    PoisonChance,
    FreezeChance,
    BurnChance,
}

[System.Flags]
public enum StatTag
{
    None = 0,
    Primary = 1 << 0,
    Secondary = 1 << 1
}