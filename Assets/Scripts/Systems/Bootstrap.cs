using System;
using VContainer.Unity;

public class Bootstrap : IStartable, IDisposable
{
    private readonly Player _player;
    private readonly WavesManager _wavesManager;
    private readonly EquipmentManager _equipmentManager;
    private readonly WeaponsHolder _weaponsHolder;
    private readonly PlayerDamageReceiver _playerDamageReceiver;
    private readonly PlayerAnimation _playerAnimation;
    private readonly UIBootstrap _UIBootstrap;
    private readonly StateMachine _stateMachine;
    private readonly StartingState _startingState;
    private readonly WaveState _waveState;
    private readonly WaveEndState _waveEndState;
    private readonly LevelUpState _levelUpState;
    private readonly ShopState _shopState;
    private readonly GameOverState _gameOverState;
    private readonly VictoryState _victoryState;

    public Bootstrap(
        Player player,
        WavesManager wavesManager,
        EquipmentManager equipmentManager,
        WeaponsHolder weaponsHolder,
        PlayerDamageReceiver playerDamageReceiver,
        PlayerAnimation playerAnimation,
        UIBootstrap UIBootstrap,
        StateMachine stateMachine,
        StartingState startingState,
        WaveState waveState,
        WaveEndState waveEndState,
        LevelUpState levelUpState,
        ShopState shopState,
        GameOverState gameOverState,
        VictoryState victoryState)
    {
        _player = player;
        _wavesManager = wavesManager;
        _equipmentManager = equipmentManager;
        _weaponsHolder = weaponsHolder;
        _playerDamageReceiver = playerDamageReceiver;
        _playerAnimation = playerAnimation;
        _UIBootstrap = UIBootstrap;
        _stateMachine = stateMachine;
        _startingState = startingState;
        _waveState = waveState;
        _waveEndState = waveEndState;
        _levelUpState = levelUpState;
        _shopState = shopState;
        _gameOverState = gameOverState;
        _victoryState = victoryState;
    }

    public void Start()
    {
        _player.Initialize();
        _playerDamageReceiver.Initialize(_player);
        _playerAnimation.Initialize(_player);
        _equipmentManager.Initialize(_player.Stats);
        _weaponsHolder.Initialize(_equipmentManager, _player, _wavesManager);
        _UIBootstrap.Initialize();

        _stateMachine.Register(_startingState);
        _stateMachine.Register(_waveState);
        _stateMachine.Register(_waveEndState);
        _stateMachine.Register(_levelUpState);
        _stateMachine.Register(_shopState);
        _stateMachine.Register(_gameOverState);
        _stateMachine.Register(_victoryState);
        _stateMachine.ChangeState<StartingState>();
    }
    public void Dispose()
    {
        _stateMachine.Dispose();
        _weaponsHolder.Dispose();
        _playerDamageReceiver.Dispose();
        _playerAnimation.Dispose();
        _UIBootstrap.Dispose();

    }
}
