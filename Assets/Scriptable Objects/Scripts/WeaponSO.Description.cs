using System.Collections.Generic;
using System.Text;

public partial class WeaponSO : IDescribableItem
{
    public string GetDescriptionTemplate()
    {
        StringBuilder sb = new();

        sb.Append("<color=#F5DC82>Damage</color>: <color=#D12B1F>{damage}</color>");
        string scalingPart = BuildScalingString();
        if (!string.IsNullOrEmpty(scalingPart))
        {
            sb.Append(scalingPart);
            sb.Append(" ");
        }
        sb.Append("\n\n");
        sb.AppendLine("<color=#F5DC82>Attack Speed</color>: <color=#29E635>{attackSpeed}</color>\n");
        sb.AppendLine("<color=#F5DC82>Range</color>: <color=#2694D4>{range}x</color>\n\n");

        if (!string.IsNullOrEmpty(SpecialDescription))
        {
            sb.AppendLine();
            sb.AppendLine(SpecialDescription);
        }

        return sb.ToString();
    }

    public Dictionary<string, string> GetDescriptionParams()
    {
        return new Dictionary<string, string>
        {
            { "damage", BaseDamage.ToString("0.##") },
            { "attackSpeed", BaseAttackSpeed.ToString("0.##") },
            { "range", RangeMultiplier.ToString("0.##") },
        };
    }
    private string BuildScalingString()
    {
        if (ScalingStats == null || ScalingStats.Count == 0)
            return string.Empty;

        StringBuilder sb = new();
        sb.Append("(");

        for (int i = 0; i < ScalingStats.Count; i++)
        {
            var entry = ScalingStats[i];

            sb.Append("+");
            sb.Append($"<sprite name={entry.ScalingStatType} Icon>");

            sb.Append($"{entry.ScalingFactor:P0}");
        }

        sb.Append(")");
        return sb.ToString();
    }

}
