using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectSO", menuName = "Scriptable Objects/StatusEffectSO")]
public class StatusEffectSO : ScriptableObject
{
    public string Id;
    public string Name;
    public StatType LinkedChanceStat;
    public float Duration;
    public float TickInterval;

    public bool IsStackable;
    public int MaxStacks;

    public List<EffectModifierSO> Modifiers;

    public GameObject VFX;
    public AudioClip ApplySound;
    public Color Color;
}
