using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{

    public event Action<PassiveItemInstance> OnItemEquipped;
    public event Action<PassiveItemInstance> OnItemUnequipped;
    public event Action<int, WeaponSO> OnWeaponEquipped;
    public event Action<int, WeaponSO> OnWeaponUnequipped;

    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private PassiveItemSO[] _startingItems;
    [SerializeField] private WeaponSO[] _startingWeapons;

    public readonly List<PassiveItemInstance> Items = new();

    [SerializeField] private int _maxWeapons = 6;
    public int MaxWeapons => _maxWeapons;
    public PlayerStats PlayerStats => _playerStats;

    private WeaponSO[] _weapons;

    private void Awake()
    {
        _weapons = new WeaponSO[_maxWeapons];
    }
    public void OnInit()
    {
        foreach (var item in _startingItems)
        {
            AddItem(item);
        }
        for (int i = 0; i < _startingWeapons.Length && i < _maxWeapons; i++)
        {
            EquipWeapon(_startingWeapons[i], i);
        }
    }
    public void AddItem(PassiveItemSO itemSO)
    {
        var instance = new PassiveItemInstance(itemSO);
        Items.Add(instance);

        foreach (var mod in itemSO.Modifiers)
        {
            var modifier = new StatModifier(
                instance.InstanceId,
                mod.modifierType,
                mod.value,
                mod.priority
            );

            _playerStats.ApplyModifier(mod.statType, modifier);
        }

        OnItemEquipped?.Invoke(instance);
    }
    public void RemovePassiveItem(PassiveItemInstance instance)
    {
        foreach (var mod in (instance.PassiveItemSO).Modifiers)
        {
            _playerStats.RemoveModifiersFromSource(
                mod.statType,
                instance.InstanceId
            );
        }

        Items.Remove(instance);
        OnItemUnequipped?.Invoke(instance);
    }
    public void EquipWeapon(WeaponSO weapon, int slot)
    {
        if (slot < 0 || slot >= _maxWeapons)
        {
            Debug.LogError("Invalid weapon slot");
            return;
        }
        _weapons[slot] = weapon;
        Debug.Log("Weapon equipped in slot " + slot);
        OnWeaponEquipped?.Invoke(slot, weapon);
    }
    public void UnequipWeapon(int slot)
    {
        if (slot < 0 || slot >= _maxWeapons)
        {
            Debug.LogError("Invalid weapon slot");
            return;
        }
        _weapons[slot] = null;
        OnWeaponUnequipped?.Invoke(slot, null);
    }
    public void ReplaceWeapon(int slot, WeaponSO newWeapon)
    {
        var oldWeapon = _weapons[slot];

        if (oldWeapon != null)
            UnequipWeapon(slot);

        EquipWeapon(newWeapon, slot);
    }
    public void Union(InventorySlot inventorySlot)
    {
        if (inventorySlot is not WeaponInventorySlot weaponSlot)
            return;

        WeaponSO weapon = GetWeapon(weaponSlot.SlotIndex);
        int secondWeaponIndex = FindSecondWeaponIndex(weapon, weaponSlot.SlotIndex);
        if (secondWeaponIndex == -1)
            return;

        if (!weapon.CanUnion)
            return;

        UnequipWeapon(secondWeaponIndex);
        ReplaceWeapon(weaponSlot.SlotIndex, weapon.UnionResult);
    }
    public WeaponSO GetWeapon(int slot)
    {
        if (slot < 0 || slot >= _maxWeapons)
        {
            Debug.LogError("Invalid weapon slot");
            return null;
        }
        return _weapons[slot];
    }
    public int GetFirstEmptyWeaponSlot()
    {
        for (int i = 0; i < _maxWeapons; i++)
        {
            if (_weapons[i] == null)
            {
                return i;
            }
        }
        return -1; // No empty slots
    }
    public int FindWeaponIndex(WeaponSO weapon)
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (_weapons[i] == weapon)
                return i;
        }
        return -1;
    }
    public int FindSecondWeaponIndex(WeaponSO weapon, int excludeIndex)
    {
        for (int i = 0; i < _weapons.Length; i++)
        {
            if (i == excludeIndex)
                continue;

            if (_weapons[i] == weapon)
                return i;
        }

        return -1;
    }


}
