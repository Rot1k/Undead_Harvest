using System;
using VContainer.Unity;

public class Bootstrap : IStartable, IDisposable
{
    private readonly PlayerStats _playerStats;
    private readonly PlayerHealthSystem _playerHealthSystem;
    private readonly PlayerLevelSystem _playerLevelSystem;
    private readonly WavesManager _wavesManager;
    private readonly EquipmentManager _equipmentManager;
    private readonly WeaponsHolder _weaponsHolder;
    private readonly PlayerMovement _playerMovement;
    private readonly PlayerDamageReceiver _playerDamageReceiver;
    private readonly PlayerAnimation _playerAnimation;
    private readonly UIBootstrap _UIBootstrap;

    public Bootstrap(
        PlayerStats playerStats,
        PlayerHealthSystem playerHealthSystem,
        PlayerLevelSystem playerLevelSystem,
        WavesManager wavesManager,
        EquipmentManager equipmentManager,
        WeaponsHolder weaponsHolder,
        PlayerMovement playerMovement,
        PlayerDamageReceiver playerDamageReceiver,
        PlayerAnimation playerAnimation,
        UIBootstrap UIBootstrap)
    {
        _playerStats = playerStats;
        _playerHealthSystem = playerHealthSystem;
        _playerLevelSystem = playerLevelSystem;
        _wavesManager = wavesManager;
        _equipmentManager = equipmentManager;
        _weaponsHolder = weaponsHolder;
        _playerMovement = playerMovement;
        _playerDamageReceiver = playerDamageReceiver;
        _playerAnimation = playerAnimation;
        _UIBootstrap = UIBootstrap;
    }

    public void Start()
    {
        _playerStats.Initialize();
        _playerHealthSystem.Initialize(_playerStats);
        _playerDamageReceiver.Initialize(_playerHealthSystem);
        _playerLevelSystem.Initialize(_playerStats);
        _playerMovement.Initialize(_playerHealthSystem);
        _playerAnimation.Initialize(_playerMovement, _playerHealthSystem);
        _equipmentManager.Initialize(_playerStats);
        _weaponsHolder.Initialize(_equipmentManager, _playerStats, _wavesManager, _playerMovement);
        _UIBootstrap.Initialize();
        _wavesManager.StartGame();
    }
    public void Dispose()
    {
        _playerDamageReceiver.Dispose();
        _playerAnimation.Dispose();
        _UIBootstrap.Dispose();

    }
}