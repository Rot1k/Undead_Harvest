using UnityEngine;
using VContainer;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private PlayerHealthSystem _playerHealthSystem;

    public bool IsWaveEnded { get; private set; } = false;
    public bool IsPlayerPaused { get; private set; } = false;

    public bool IsPaused => (IsWaveEnded || IsPlayerPaused);

    private WavesManager _wavesManager;

    [Inject]
    public void Constuct(WavesManager wavesManager)
    {
        _wavesManager = wavesManager;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        Time.timeScale = 1f;

        if (_wavesManager == null)
        {
            Debug.LogError("WavesManager instance is null in PauseManager.");
            return;
        }
        _wavesManager.OnWaveStarted += () =>
        {
            IsWaveEnded = false;
            UpdatePauseState();
        };
        _wavesManager.OnWaveCompleted += () =>
        {
            IsWaveEnded = true;
            UpdatePauseState();
        };
        _wavesManager.OnAllWavesCompleted += () =>
        {
            IsWaveEnded = true;
            UpdatePauseState();
        };
        _playerHealthSystem.HealthSystem.OnDead += (sender, e) =>
        {
            IsPlayerPaused = true;
            UpdatePauseState();
        };
    }

    private void Start()
    {
        GameInput.Instance.OnPause += () =>
        {
            IsPlayerPaused = !IsPlayerPaused;
            UpdatePauseState();
        };
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