using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemsPoolSO", menuName = "Scriptable Objects/InventoryItemsPoolSO")]
public class InventoryItemsPoolSO: ScriptableObject
{
    [SerializeField] private InventoryItemSO[] _items;

    public InventoryItemSO[] Items => _items;

    private void OnValidate()
    {
        if (_items == null)
        {
            _items = System.Array.Empty<InventoryItemSO>();
            return;
        }

        _items = _items
            .Where(i => i != null)
            .Distinct()
            .ToArray();
    }
}
