using UnityEngine;

public class ItemsInventoryStatisticsUI : MonoBehaviour
{
    [SerializeField] private ItemsInventoryUI _itemsInventory;
    private void OnEnable()
    {
        ShowStats();
    }
    private void ShowStats()
    {
        foreach (PassiveItemInstance passiveItem in EquipmentManager.Instance.Items)
        {
            _itemsInventory.SpawnItemUI(passiveItem);
        }
    }
}
