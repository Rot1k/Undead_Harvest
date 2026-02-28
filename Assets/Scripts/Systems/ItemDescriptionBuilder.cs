using UnityEngine;

public static class ItemDescriptionBuilder
{
    public static string Build(InventoryItemSO item)
    {
        if (item is not IDescribableItem describable)
            return string.Empty;

        string text = describable.GetDescriptionTemplate();

        foreach (var pair in describable.GetDescriptionParams())
        {
            text = text.Replace($"{{{pair.Key}}}", pair.Value);
        }

        return text;
    }
}
