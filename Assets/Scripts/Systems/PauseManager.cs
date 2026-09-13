using UnityEngine;
using VContainer;

public class PauseManager : MonoBehaviour
{

    private Player _player;

    public bool IsWaveEnded { get; private set; } = false;
    public bool IsPlayerPaused { get; private set; } = false;

    public bool IsPaused => (IsWaveEnded || IsPlayerPaused);

    private WavesManager _wavesManager;
    private GameInput _gameInput;
   
    [Inject]
    public void Construct(WavesManager wavesManager, GameInput gameInput, Player player)
    {
        _wavesManager = wavesManager;
        _gameInput = gameInput;
        _player = player;
    }

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void Start()
    {
        if (_wavesManager == null || _gameInput == null || _player == null)
        {
            Debug.LogError("PauseManager dependencies are not assigned.");
            return;
        }

        _wavesManager.OnWaveStarted += OnWaveStarted;
        _wavesManager.OnWaveCompleted += OnWaveEnded;
        _wavesManager.OnAllWavesCompleted += OnWaveEnded;
        _player.HealthSystem.OnDead += OnPlayerDied;
        _gameInput.OnPause += OnPausePressed;
    }

    private void OnDestroy()
    {
        if (_wavesManager != null)
        {
            _wavesManager.OnWaveStarted -= OnWaveStarted;
            _wavesManager.OnWaveCompleted -= OnWaveEnded;
            _wavesManager.OnAllWavesCompleted -= OnWaveEnded;
        }

        if (_player != null)
        {
            _player.HealthSystem.OnDead -= OnPlayerDied;
        }

        if (_gameInput != null)
        {
            _gameInput.OnPause -= OnPausePressed;
        }
    }

    private void OnWaveStarted()
    {
        IsWaveEnded = false;
        UpdatePauseState();
    }

    private void OnWaveEnded()
    {
        IsWaveEnded = true;
        UpdatePauseState();
    }

    private void OnPlayerDied(object sender, System.EventArgs e)
    {
        IsPlayerPaused = true;
        UpdatePauseState();
    }

    private void OnPausePressed()
    {
        IsPlayerPaused = !IsPlayerPaused;
        UpdatePauseState();
    }

    private void UpdatePauseState()
    {
        if (IsPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }
    public void SetPlayerPaused(bool value)
    {
        IsPlayerPaused = value;
        UpdatePauseState();
    }
    public void SetWaveEnded(bool value)
    {
        IsWaveEnded = value;
        UpdatePauseState();
    }

}
