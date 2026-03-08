using UnityEngine;

public static class ItemDescriptionBuilder
{
    public static string Build(InventoryItemSO item, PlayerStatsSO playerStatsSO)
    {
        if (item is not IDescribableItem describable)
            return string.Empty;

        string text = describable.GetDescriptionTemplate();

        foreach (var pair in describable.GetDescriptionParams(playerStatsSO))
        {
            text = text.Replace($"{{{pair.Key}}}", pair.Value);
        }

        return text;
    }
}
