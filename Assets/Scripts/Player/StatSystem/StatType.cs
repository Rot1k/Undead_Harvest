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
    CriticalChance, // 0 - 100
    CriticalDamageMultiplier,
    ItemPickupRange,
    ExperienceMultiplier,
    GlobalEffectChanceMultiplier,
    BleedChance, // 0 - 100
    PoisonChance, // 0 - 100
    FreezeChance, // 0 - 100
    BurnChance, // 0 - 100
}

[System.Flags]
public enum StatTag
{
    None = 0,
    Primary = 1 << 0,
    Secondary = 1 << 1
}