using VContainer;
using VContainer.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLifetimeScope : LifetimeScope
{
    [SerializeField] private WavesManager _wavesManager;
    [SerializeField] private WaveEndWindow _waveEndWindow;
    [SerializeField] private PauseManager _pauseManager;
    [SerializeField] private EquipmentManager _equipmentManager;
    [SerializeField] private WeaponsHolder _weaponHolder;
    [SerializeField] private WalletManager _walletManager;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private PlayerHealthSystem _playerHealthSystem;
    [SerializeField] private PlayerDamageReceiver _playerDamageReceiver;
    [SerializeField] private PlayerLevelSystem _playerLevelSystem;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerAnimation _playerAnimation;
    [SerializeField] private UIBootstrap _UIBootstrap;
    protected override void Configure(IContainerBuilder builder)
    {

        //Systems
        builder.RegisterComponent(_wavesManager);
        builder.RegisterComponent(_waveEndWindow);
        builder.RegisterComponent(_pauseManager);
        builder.RegisterComponent(_equipmentManager);
        builder.RegisterComponent(_walletManager);
        builder.RegisterComponent(_playerStats);
        builder.RegisterComponent(_playerHealthSystem);
        builder.RegisterComponent(_playerDamageReceiver);
        builder.RegisterComponent(_playerLevelSystem);
        builder.RegisterComponent(_playerMovement);
        builder.RegisterComponent(_playerAnimation);
        builder.RegisterComponent(_weaponHolder);

        //UI
        builder.RegisterComponent(_UIBootstrap);

        //Services
        builder.Register<ShopService>(Lifetime.Singleton).As<IShopService>();


        builder.RegisterEntryPoint<Bootstrap>();

        builder.RegisterBuildCallback(container =>
        {
            Scene scene = gameObject.scene;

            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                container.InjectGameObject(rootGameObject);
            }
        });
    }

}
