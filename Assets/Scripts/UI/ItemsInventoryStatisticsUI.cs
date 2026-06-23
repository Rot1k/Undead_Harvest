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
    }
    private void OnEnable()
    {
        ShowStats();
    }
    private void ShowStats()
    {
        foreach (PassiveItemInstance passiveItem in _equipmentManager.Items)
        {
            _itemsInventory.SpawnItemUI(passiveItem);
        }
    }
}
