using UnityEngine;

[CreateAssetMenu(fileName = "SlowEffectModifierSO", menuName = "Scriptable Objects/EffectModifierSO/SlowEffectModifierSO")]
public class SlowEffectModifierSO : EffectModifierSO
{
    [Range(0, 100)] public float SlowPercentAmount = 10;
    public override void OnApply(StatusEffectInstance instance, Enemy target)
    {
        target.ApplyModifier(StatType.MoveSpeed, ModifierType.Multiplier, 1f - (SlowPercentAmount / 100f));
    }
    public override void OnExpire(StatusEffectInstance instance, Enemy target)
    {
        target.ApplyModifier(StatType.MoveSpeed, ModifierType.Multiplier, 1f / (1f - (SlowPercentAmount / 100f)));
    }
}
