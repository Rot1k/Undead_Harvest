using UnityEngine;

public static class CombatSystem
{
    public static float CalculateDamage(PlayerStats playerStats, WeaponSO weaponStats)
    {
        float baseDamage = weaponStats.BaseDamage;
        float scalingDamage;
        if (weaponStats.ScalingStats != null && weaponStats.ScalingStats.Count > 0)
        {
            scalingDamage = 0f;
            foreach (var scalingEntry in weaponStats.ScalingStats)
            {
                float statValue = playerStats.Get(scalingEntry.ScalingStatType);
                scalingDamage += statValue * scalingEntry.ScalingFactor;
            }
        }
        else
        {
            scalingDamage = 0f;
        }
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
