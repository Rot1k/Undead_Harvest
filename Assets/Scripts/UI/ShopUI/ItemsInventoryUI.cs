using NTC.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class ItemsInventoryUI : MonoBehaviour
{
    [SerializeField] private Transform _itemsParent;
    [SerializeField] private GameObject _itemSlotPrefab;
    [SerializeField] private ItemInfoController _itemInfoController;
    [SerializeField] private RarityConfigSO _rarityConfig;

    private Dictionary<PassiveItemSO, ItemInventorySlot> _slots = new();

    private EquipmentManager _equipmentManager;

    [Inject]
    public void Construct(EquipmentManager equipmentManager)
    {
        _equipmentManager = equipmentManager;
    }

    public void Initialize()
    {
        _equipmentManager.OnItemEquipped += SpawnItemUI;
        _equipmentManager.OnItemUnequipped += DespawnItemUI;
        SpawnStartItemsUI();
    }

    private void SpawnStartItemsUI()
    {
        foreach (var item in _equipmentManager.Items)
        {
            SpawnItemUI(item);
        }
    }

    public void Dispose()
    {
        if (_equipmentManager != null)
        {
            _equipmentManager.OnItemEquipped -= SpawnItemUI;
            _equipmentManager.OnItemUnequipped -= DespawnItemUI;
        }
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public void SpawnItemUI(PassiveItemInstance item)
    {
        Debug.Log($"Spawning item UI for {item.PassiveItemSO.name}");
        if (_slots.TryGetValue(item.PassiveItemSO, out var slot))
        {
            Debug.Log($"Item slot already exists for {item.PassiveItemSO.name}, pushing instance");
            slot.PushInstance(item);
            return;
        }

        var newSlot = NightPool.Spawn(_itemSlotPrefab, _itemsParent)
            .GetComponent<ItemInventorySlot>();

        newSlot.SetItem(item, _itemInfoController);
        newSlot.SlotOutline.effectColor = _rarityConfig.GetColor(item.PassiveItemSO.Rarity);
        _slots[item.PassiveItemSO] = newSlot;
    }

    private void DespawnItemUI(PassiveItemInstance item)
    {
        if (!_slots.TryGetValue(item.PassiveItemSO, out var slot))
            return;

        slot.PopInstance();

        if (slot.GetCount() <= 0)
        {
            _slots.Remove(item.PassiveItemSO);
            NightPool.Despawn(slot.gameObject);
        }
    }
}
