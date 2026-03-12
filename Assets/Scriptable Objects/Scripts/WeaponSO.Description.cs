using System.Collections.Generic;
using System.Text;
using UnityEngine;

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
        string onHitEffectsPart = BuildOnHitEffectsString();
        if (!string.IsNullOrEmpty(onHitEffectsPart))
        {
            sb.AppendLine();
            sb.AppendLine(onHitEffectsPart);
        }

        return sb.ToString();
    }

    public Dictionary<string, string> GetDescriptionParams(PlayerStatsSO playerStatsSO)
    {
        return new Dictionary<string, string>
        {
            { "damage", BaseDamage.ToString("0.##") },
            { "attackSpeed", BaseAttackSpeed.ToString("0.##") },
            { "range", RangeMultiplier.ToString("0.##") },
        };
    }
    private string GetScalingStatIcon(ScalingStatsEntry scalingStatsEntry)
    {
        return $"<sprite name={scalingStatsEntry.ScalingStatType} Icon>";
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
            sb.Append(GetScalingStatIcon(entry));

            sb.Append($"{entry.ScalingFactor:P0}");
        }

        sb.Append(")");
        return sb.ToString();
    }
    private string BuildOnHitEffectsString()
    {
        if (OnHitEffects == null || OnHitEffects.Count == 0)
            return string.Empty;
        StringBuilder sb = new();
        sb.AppendLine("<color=#F5DC82>On Hit:</color>");
        foreach (var effectData in OnHitEffects)
        {
            sb.AppendLine($"- <color=#{ColorUtility.ToHtmlStringRGB(effectData.Effect.Color)}>{effectData.Effect.Name}</color> ({effectData.Chance:P0} chance)");
        }
        return sb.ToString();
    }

}
