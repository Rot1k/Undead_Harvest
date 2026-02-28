using UnityEngine;

[CreateAssetMenu(fileName = "FlatDamageEffectModifierSO", menuName = "Scriptable Objects/EffectModifierSO/FlatDamageEffectModifierSO")]
public class FlatDamageEffectModifierSO : EffectModifierSO
{
    public int FlatDamageAmount = 10;

    public override void OnTick(StatusEffectInstance instance, Enemy target, int stackCount)
    {
        target.HealthSystem.Damage(FlatDamageAmount * stackCount);
    }
}
