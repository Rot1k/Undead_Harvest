using NTC.Pool;
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

    private void Start()
    {
        _equipmentManager.OnItemEquipped += SpawnItemUI;
        _equipmentManager.OnItemUnequipped += DespawnItemUI;
    }

    private void OnDestroy()
    {
        if (_equipmentManager == null) return;

        _equipmentManager.OnItemEquipped -= SpawnItemUI;
        _equipmentManager.OnItemUnequipped -= DespawnItemUI;
    }

    public void SpawnItemUI(PassiveItemInstance item)
    {
        if (_slots.TryGetValue(item.PassiveItemSO, out var slot))
        {
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
