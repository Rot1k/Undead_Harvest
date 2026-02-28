using UnityEngine;

public abstract class EffectModifierSO : ScriptableObject
{
    public virtual void OnApply(StatusEffectInstance instance, Enemy target) { }

    public virtual void OnTick(StatusEffectInstance instance, Enemy target, int stacks) { }

    public virtual void OnExpire(StatusEffectInstance instance, Enemy target) { }
}
