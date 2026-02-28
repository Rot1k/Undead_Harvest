using UnityEngine;

public static class CombatSystem
{
    public static float CalculateDamage(PlayerStats playerStats, WeaponSO weaponStats)
    {
        float baseDamage = weaponStats.BaseDamage;
        float scalingDamage = playerStats.Get(weaponStats.ScalingStats[0]) * weaponStats.ScalingFactor;
        float attackDamage = playerStats.Get(StatType.AttackDamage);
        float totalDamage = baseDamage + scalingDamage + attackDamage;

        float critChance = playerStats.Get(StatType.CriticalChance);
        float critMultiplier = playerStats.Get(StatType.CriticalDamageMultiplier);
        if(Random.Range(0f, 100f) < critChance)
        {
            totalDamage *= critMultiplier;
            Debug.Log($"Critical Hit! Damage: {totalDamage}");
        }
        return totalDamage;
    }
}
