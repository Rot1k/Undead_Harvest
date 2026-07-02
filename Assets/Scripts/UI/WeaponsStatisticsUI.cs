using UnityEngine;
using VContainer;

public class WeaponsStatisticsUI : MonoBehaviour
{
    [SerializeField] private WeaponsInventoryUI _weaponsInventoryUI;

    private EquipmentManager _equipmentManager;

    [Inject]
    public void Construct(EquipmentManager equipmentManager)
    {
        _equipmentManager = equipmentManager;
    }
    public void Initialize()
    {
        ShowStats();
    }

    private void OnEnable()
    {
        if (_equipmentManager != null)
        {
            ShowStats();
        }
    }
    private void ShowStats()
    {
        for (int i = 0; i < _equipmentManager.MaxWeapons; i++)
        {
            _weaponsInventoryUI.UpdateUI(i, _equipmentManager.GetWeapon(i));
        }
    }

    public void Dispose()
    {
    }

    private void OnDestroy()
    {
        Dispose();
    }
}
