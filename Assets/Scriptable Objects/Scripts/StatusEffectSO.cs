using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectSO", menuName = "Scriptable Objects/StatusEffectSO")]
public class StatusEffectSO : ScriptableObject
{
    public string Id;
    public string Name;
    public StatType LinkedChanceStat;
    [Min(0.01f)] public float Duration;
    [Min(0.01f)] public float TickInterval;

    public bool IsStackable;
    [Min(1)] public int MaxStacks;

    public List<EffectModifierSO> Modifiers;
    public Color Color;

    private void OnValidate()
    {
        if (Modifiers == null)
            Modifiers = new List<EffectModifierSO>();
        else
            Modifiers.RemoveAll(modifier => modifier == null);
    }
}
