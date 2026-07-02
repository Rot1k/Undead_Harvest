using UnityEngine;

public class UIBootstrap : MonoBehaviour
{
    [SerializeField] private HealthBarUI _healthBarUI;
    [SerializeField] private ExpBarUI _expBarUI;
    [SerializeField] private ShopUI _shopUI;
    [SerializeField] private StatsTableUI[] _statsTables;
    [SerializeField] private WeaponsInventoryUI[] _weaponsInventoryUIs;
    [SerializeField] private WeaponsStatisticsUI[] _weaponsStatisticsUIs;
    [SerializeField] private ItemsInventoryUI[] _itemsInventoryUIs;

    // Additional UI elements to be StartGame explicitly
    [SerializeField] private MenuUI _menuUI;
    [SerializeField] private MainUI _mainUI;
    [SerializeField] private WaveTimerUI _waveTimerUI;
    [SerializeField] private WaveCounterUI _waveCounterUI;
    [SerializeField] private WalletUI[] _walletUIs;
    [SerializeField] private HUDUI _hudUI;
    [SerializeField] private BossHealthBarUI _bossHealthBarUI;
    [SerializeField] private LevelUpWindowUI _levelUpWindowUI;
    [SerializeField] private WaveEndWindow _waveEndWindow;
    [SerializeField] private WinLoseWindowUI _winLoseWindowUI;

    public void Initialize()
    {
        _healthBarUI.Initialize();
        _expBarUI.Initialize();

        foreach (var table in _statsTables) table.Initialize();
        foreach (var ui in _weaponsInventoryUIs) ui.Initialize();
        foreach (var ui in _weaponsStatisticsUIs) ui.Initialize();
        foreach (var ui in _itemsInventoryUIs) ui.Initialize();
        _shopUI.Initialize();
        _menuUI.Initialize();
        _mainUI.Initialize();
        _waveTimerUI.Initialize();
        _waveCounterUI.Initialize();
        foreach (var ui in _walletUIs) ui.Initialize();
        _hudUI.Initialize();
        _bossHealthBarUI.Initialize();
        _levelUpWindowUI.Initialize();
        _waveEndWindow.Initialize();
        _winLoseWindowUI.Initialize();
    }
    public void Dispose()
    {
        _healthBarUI.Dispose();
        _expBarUI.Dispose();
        _shopUI.Dispose();

        foreach (var table in _statsTables)
        {
            table.Dispose();
        }

        foreach (var ui in _weaponsInventoryUIs)
        {
            ui.Dispose();
        }

        foreach (var ui in _weaponsStatisticsUIs)
        {
            ui.Dispose();
        }

        foreach (var ui in _itemsInventoryUIs)
        {
            ui.Dispose();
        }

        _menuUI.Dispose();
        _mainUI.Dispose();
        _waveTimerUI.Dispose();
        _waveCounterUI.Dispose();

        foreach (var ui in _walletUIs)
        {
            ui.Dispose();
        }

        _hudUI.Dispose();
        _bossHealthBarUI.Dispose();
        _levelUpWindowUI.Dispose();
        _waveEndWindow.Dispose();
        _winLoseWindowUI.Dispose();
    }
}
