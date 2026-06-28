using UnityEngine;
using VContainer;

public class StatsTableUI : MonoBehaviour
{
    private PlayerStatsSO _statsSO;

    private StatUI[] _statUIs;

    private PlayerStats _playerStats;

    private void Awake()
    {
        _statUIs = GetComponentsInChildren<StatUI>(true);
    }

    public void Initialize(PlayerStats playerStats)
    {
        _playerStats = playerStats;
        _statsSO = _playerStats?.StatsSO;
        Refresh();
    }
    private void Refresh()
    {
        if (_playerStats == null) Debug.LogWarning($"{name}: PlayerStats not assigned.");
        if (_statsSO == null) Debug.LogWarning($"{name}: PlayerStatsSO not assigned.");

        // Ensure we have stat UI references even if Initialize was called before Awake
        if (_statUIs == null || _statUIs.Length == 0)
            _statUIs = GetComponentsInChildren<StatUI>(true);

        if (_statUIs == null || _statUIs.Length == 0)
            return;

        foreach (StatUI statUI in _statUIs)
        {
            statUI.Initialize(_playerStats, _statsSO);
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
