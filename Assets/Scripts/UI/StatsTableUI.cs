using UnityEngine;

public class StatsTableUI : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private PlayerStatsSO _statsSO;

    private StatUI[] _statUIs;

    private void Awake()
    {
        _statUIs = GetComponentsInChildren<StatUI>(true);
    }

    private void Start()
    {
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }
    private void Refresh()
    {
        if (_playerStats == null && EquipmentManager.Instance != null)
        {
            _playerStats = EquipmentManager.Instance.PlayerStats;
        }
        if (_playerStats == null) Debug.LogWarning($"{name}: PlayerStats not assigned.");
        if (_statsSO == null) Debug.LogWarning($"{name}: PlayerStatsSO not assigned.");

        foreach (StatUI statUI in _statUIs)
        {
            statUI.Initialize(_playerStats, _statsSO);
        }
    }
}