using UnityEngine;
public class WeaponInventorySlot : InventorySlot
{
    [SerializeField] private int _slotIndex;
    public int SlotIndex => _slotIndex;
}
