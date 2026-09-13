using System;

public abstract class GameFlowState : State
{
    protected readonly WavesManager WavesManager;
    protected readonly Player Player;

    protected GameFlowState(WavesManager wavesManager, Player player)
    {
        WavesManager = wavesManager;
        Player = player;
    }
}

public sealed class StartingState : GameFlowState
{
    public StartingState(WavesManager wavesManager, Player player)
        : base(wavesManager, player)
    {
    }

    public override void Enter()
    {
        ChangeState<WaveState>();
        WavesManager.StartGame();
    }
}

public sealed class WaveState : GameFlowState
{
    public WaveState(WavesManager wavesManager, Player player)
        : base(wavesManager, player)
    {
    }

    public override void Enter()
    {
        WavesManager.OnWaveCompleted += OnWaveCompleted;
        WavesManager.OnAllWavesCompleted += OnAllWavesCompleted;
        Player.HealthSystem.OnDead += OnPlayerDead;

        if (WavesManager.CurrentWave > 0)
            WavesManager.StartNextWave();
    }

    public override void Exit()
    {
        WavesManager.OnWaveCompleted -= OnWaveCompleted;
        WavesManager.OnAllWavesCompleted -= OnAllWavesCompleted;
        Player.HealthSystem.OnDead -= OnPlayerDead;
    }

    private void OnWaveCompleted()
    {
        ChangeState<WaveEndState>();
    }

    private void OnAllWavesCompleted()
    {
        ChangeState<WaveEndState>();
    }

    private void OnPlayerDead(object sender, EventArgs e)
    {
        ChangeState<GameOverState>();
    }
}

public sealed class WaveEndState : GameFlowState
{
    private readonly WaveEndWindow _waveEndWindow;

    public WaveEndState(
        WavesManager wavesManager,
        Player player,
        WaveEndWindow waveEndWindow)
        : base(wavesManager, player)
    {
        _waveEndWindow = waveEndWindow;
    }

    public override void Enter()
    {
        _waveEndWindow.OnWaveMessageHidden += OnWaveMessageHidden;
        _waveEndWindow.OnFinalWaveMessageHidden += OnFinalWaveMessageHidden;

        _waveEndWindow.ShowWaveCompleted(
            WavesManager.CurrentWave,
            WavesManager.IsAllWavesCompleted);
    }

    public override void Exit()
    {
        _waveEndWindow.OnWaveMessageHidden -= OnWaveMessageHidden;
        _waveEndWindow.OnFinalWaveMessageHidden -= OnFinalWaveMessageHidden;
    }

    private void OnWaveMessageHidden()
    {
        if (Player.Level.LevelSystem.SkillPoints > 0)
            ChangeState<LevelUpState>();
        else
            ChangeState<ShopState>();
    }

    private void OnFinalWaveMessageHidden()
    {
        ChangeState<VictoryState>();
    }
}

public sealed class LevelUpState : GameFlowState
{
    private readonly LevelUpWindowUI _levelUpWindow;

    public LevelUpState(
        WavesManager wavesManager,
        Player player,
        LevelUpWindowUI levelUpWindow)
        : base(wavesManager, player)
    {
        _levelUpWindow = levelUpWindow;
    }

    public override void Enter()
    {
        _levelUpWindow.OnCompleted += OnCompleted;
        _levelUpWindow.Show();
    }

    public override void Exit()
    {
        _levelUpWindow.OnCompleted -= OnCompleted;
        _levelUpWindow.Hide();
    }

    private void OnCompleted()
    {
        ChangeState<ShopState>();
    }
}

public sealed class ShopState : GameFlowState
{
    private readonly ShopUI _shopUI;
    private readonly HUDUI _hudUI;

    public ShopState(
        WavesManager wavesManager,
        Player player,
        ShopUI shopUI,
        HUDUI hudUI)
        : base(wavesManager, player)
    {
        _shopUI = shopUI;
        _hudUI = hudUI;
    }

    public override void Enter()
    {
        _hudUI.Hide();
        _shopUI.OnClosed += OnClosed;
        _shopUI.Show();
    }

    public override void Exit()
    {
        _shopUI.OnClosed -= OnClosed;
        _hudUI.Show();
    }

    private void OnClosed()
    {
        ChangeState<WaveState>();
    }
}

public sealed class GameOverState : GameFlowState
{
    private readonly WaveEndWindow _waveEndWindow;
    private readonly WinLoseWindowUI _winLoseWindow;

    public GameOverState(
        WavesManager wavesManager,
        Player player,
        WaveEndWindow waveEndWindow,
        WinLoseWindowUI winLoseWindow)
        : base(wavesManager, player)
    {
        _waveEndWindow = waveEndWindow;
        _winLoseWindow = winLoseWindow;
    }

    public override void Enter()
    {
        WavesManager.StopCurrentWave();
        _waveEndWindow.OnPlayerDeathMessageHidden += OnPlayerDeathMessageHidden;
        _waveEndWindow.ShowPlayerDied();
    }

    public override void Exit()
    {
        _waveEndWindow.OnPlayerDeathMessageHidden -= OnPlayerDeathMessageHidden;
    }

    private void OnPlayerDeathMessageHidden()
    {
        _winLoseWindow.ShowLose();
    }
}

public sealed class VictoryState : GameFlowState
{
    private readonly WinLoseWindowUI _winLoseWindow;

    public VictoryState(
        WavesManager wavesManager,
        Player player,
        WinLoseWindowUI winLoseWindow)
        : base(wavesManager, player)
    {
        _winLoseWindow = winLoseWindow;
    }

    public override void Enter()
    {
        _winLoseWindow.ShowWin();
    }
}
