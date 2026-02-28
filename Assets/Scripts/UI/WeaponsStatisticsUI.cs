using UnityEngine;

public class WeaponsStatisticsUI : MonoBehaviour
{
    [SerializeField] private WeaponsInventoryUI _weaponsInventoryUI;
    private void OnEnable()
    {
        ShowStats();
    }
    private void ShowStats()
    {
        for (int i = 0; i < EquipmentManager.Instance.MaxWeapons; i++)
        {
            _weaponsInventoryUI.UpdateUI(i, EquipmentManager.Instance.GetWeapon(i));
        }
    }
}
