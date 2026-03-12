using UnityEngine;

[CreateAssetMenu(fileName = "PercentDamageEffectModifierSO", menuName = "Scriptable Objects/EffectModifierSO/PercentDamageEffectModifierSO")]
public class PercentDamageEffectModifierSO : EffectModifierSO
{
    [Range(0, 100)] public float PercentDamageAmount = 5;

    public override void OnTick(StatusEffectInstance instance, Enemy target, int stackCount)
    {
        if (target is BossEnemy) return;

        target.HealthSystem.Damage(Mathf.RoundToInt(target.HealthSystem.HealthMax * PercentDamageAmount * stackCount / 100));
    }
}
    