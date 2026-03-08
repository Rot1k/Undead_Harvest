using System;
using System.Collections.Generic;
using System.Text;

public partial class PassiveItemSO : IDescribableItem
{
    public string GetDescriptionTemplate()
    {
        StringBuilder stringBuilder = new();

        for (int i = 0; i < Modifiers.Count; i++)
            stringBuilder.AppendLine($"{{mod_{i}}}\n");

        return stringBuilder.ToString();
    }

    public Dictionary<string, string> GetDescriptionParams(PlayerStatsSO playerStatsSO)
    {
        var dictionary = new Dictionary<string, string>();

        for (int i = 0; i < Modifiers.Count; i++)
        {
            var modifier = Modifiers[i];
            string valueText = StatModifierFormatter.Format(
                new StatModifier(Guid.Empty, modifier.modifierType, modifier.value, modifier.priority)
            );

            dictionary.Add(
                $"mod_{i}",
                $"{valueText} {playerStatsSO.GetName(modifier.statType)}"
            );
        }

        return dictionary;
    }
}
