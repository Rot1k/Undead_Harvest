using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemsPoolSO", menuName = "Scriptable Objects/InventoryItemsPoolSO")]
public class InventoryItemsPoolSO: ScriptableObject
{
    [SerializeField] private InventoryItemSO[] _items;

    public InventoryItemSO[] Items => _items;

    private void OnValidate()
    {
        _items = _items
            .Where(i => i != null)
            .Distinct()
            .ToArray();
    }
}
