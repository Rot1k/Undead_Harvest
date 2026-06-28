using UnityEngine;
using VContainer;

public class ItemsInventoryStatisticsUI : MonoBehaviour
{
    [SerializeField] private ItemsInventoryUI _itemsInventory;

    private EquipmentManager _equipmentManager;

    [Inject]
    public void Construct(EquipmentManager equipmentManager)
    {
        _equipmentManager = equipmentManager;
        if (_equipmentManager != null)
            ShowStats();
    }
    private void OnEnable()
    {
        if (_equipmentManager != null)
            ShowStats();
    }
    private void ShowStats()
    {
        if (_equipmentManager == null || _itemsInventory == null)
            return;

        foreach (PassiveItemInstance passiveItem in _equipmentManager.Items)
        {
            _itemsInventory.SpawnItemUI(passiveItem);
        }
    }
}
