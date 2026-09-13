using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class SceneLifetimeScope : LifetimeScope
{
    [SerializeField] private WavesManager _wavesManager;
    [SerializeField] private WaveEndWindow _waveEndWindow;
    [SerializeField] private PauseManager _pauseManager;
    [SerializeField] private EquipmentManager _equipmentManager;
    [SerializeField] private WeaponsHolder _weaponHolder;
    [SerializeField] private WalletManager _walletManager;
    [SerializeField] private Player _player;
    [SerializeField] private PlayerDamageReceiver _playerDamageReceiver;
    [SerializeField] private PlayerAnimation _playerAnimation;
    [SerializeField] private UIBootstrap _UIBootstrap;
    protected override void Configure(IContainerBuilder builder)
    {

        //Systems
        builder.RegisterComponent(_wavesManager);
        builder.RegisterComponent(_waveEndWindow);
        builder.RegisterComponent(_player);
        builder.RegisterComponent(_equipmentManager);
        builder.RegisterComponent(_pauseManager);
        builder.RegisterComponent(_walletManager);
        builder.RegisterComponent(_playerDamageReceiver);
        builder.RegisterComponent(_playerAnimation);
        builder.RegisterComponent(_weaponHolder);

        //UI
        builder.RegisterComponent(_UIBootstrap);
        builder.RegisterComponentInHierarchy<ShopUI>();
        builder.RegisterComponentInHierarchy<LevelUpWindowUI>();
        builder.RegisterComponentInHierarchy<WinLoseWindowUI>();
        builder.RegisterComponentInHierarchy<HUDUI>();

        //Services
        builder.Register<ShopService>(Lifetime.Singleton).As<IShopService>();
        builder.Register<StateMachine>(Lifetime.Singleton);
        builder.Register<StartingState>(Lifetime.Singleton);
        builder.Register<WaveState>(Lifetime.Singleton);
        builder.Register<WaveEndState>(Lifetime.Singleton);
        builder.Register<LevelUpState>(Lifetime.Singleton);
        builder.Register<ShopState>(Lifetime.Singleton);
        builder.Register<GameOverState>(Lifetime.Singleton);
        builder.Register<VictoryState>(Lifetime.Singleton);


        builder.RegisterBuildCallback(container =>
        {
            container.InjectGameObject(_player.gameObject);

            foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (root != _player.gameObject)
                    container.InjectGameObject(root);
            }
        });

        builder.RegisterEntryPoint<Bootstrap>();
    }

}
