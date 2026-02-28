using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private PlayerHealthSystem _playerHealthSystem;

    public bool IsWaveEnded { get; private set; } = false;
    public bool IsPlayerPaused { get; private set; } = false;

    public bool IsPaused => (IsWaveEnded || IsPlayerPaused);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        Time.timeScale = 1f;

        if (WavesManager.Instance == null)
        {
            Debug.LogError("WavesManager instance is null in PauseManager.");
            return;
        }
        WavesManager.Instance.OnWaveStarted += () =>
        {
            IsWaveEnded = false;
            UpdatePauseState();
        };
        WavesManager.Instance.OnWaveCompleted += () =>
        {
            IsWaveEnded = true;
            UpdatePauseState();
        };
        WavesManager.Instance.OnAllWavesCompleted += () =>
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