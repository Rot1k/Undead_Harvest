using UnityEngine;
using VContainer;

public class WeaponsStatisticsUI : MonoBehaviour
{
    [SerializeField] private WeaponsInventoryUI _weaponsInventoryUI;

    private EquipmentManager _equipmentManager ;

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
        for (int i = 0; i < _equipmentManager.MaxWeapons; i++)
        {
            _weaponsInventoryUI.UpdateUI(i, _equipmentManager.GetWeapon(i));
        }
    }
}
