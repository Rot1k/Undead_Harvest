using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemsPoolSO", menuName = "Scriptable Objects/InventoryItemsPoolSO")]
public class InventoryItemsPoolSO: ScriptableObject
{
    public InventoryItemSO[] Items;
}
