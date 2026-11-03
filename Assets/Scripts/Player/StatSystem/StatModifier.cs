using System;
public enum ModifierType { Flat, AddPercent, Multiplier }
public class StatModifier
{
    public Guid SourceId { get; }
    public ModifierType Type { get; }
    public float Value { get; }
    public StatModifier(Guid sourceId, ModifierType type, float value)
    {
        SourceId = sourceId;
        Type = type;
        if (type == ModifierType.AddPercent)
        {
            Value = value / 100;
        }
        else
            Value = value;
    }
}
