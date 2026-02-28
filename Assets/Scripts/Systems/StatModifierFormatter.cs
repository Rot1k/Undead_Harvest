public static class StatModifierFormatter
{
    public static string Format(StatModifier modifier)
    {
        return modifier.Type switch
        {
            ModifierType.Flat =>
                FormatFlat(modifier.Value),

            ModifierType.AddPercent =>
                FormatAddPercent(modifier.Value),

            ModifierType.Multiplier =>
                FormatMultiplier(modifier.Value),

            _ => string.Empty
        };
    }
    private static string FormatFlat(float value)
    {
        string sign = value >= 0 ? "+" : "";
        string color = value >= 0 ? "#55FF55" : "#FF5555";

        return $"<color={color}>{sign}{value:0.##}</color>";
    }
    private static string FormatAddPercent(float value)
    {
        string sign = value >= 0 ? "+" : "";
        string color = value >= 0 ? "#55FF55" : "#FF5555";

        return $"<color={color}>{sign}{value:P0}</color>";
    }
    private static string FormatMultiplier(float value)
    {
        string color = value >= 1f ? "#55FF55" : "#FF5555";
        return $"<color={color}>x{value:0.##}</color>";
    }
}