using UnityEngine;
using VContainer;

public class LevelUpWindowUI : MonoBehaviour
{
    [field: SerializeField] public PlayerLevelSystem PlayerLevelSystem { get; private set; }
    [SerializeField] private LevelUpPanelsController _levelUpPanelsController;

    private WaveEndWindow _waveEndWindow;
    private WavesManager _wavesManager;

    [Inject]
    public void Construct(WaveEndWindow waveEndWindow, WavesManager wavesManager)
    {
        _waveEndWindow = waveEndWindow;
        _wavesManager = wavesManager;
    }

    private void Start()
    {
        _waveEndWindow.OnWindowHidden += OnWaveCompleted;
        Hide();
    }
    private void OnDestroy()
    {
        if (_waveEndWindow != null)
            _waveEndWindow.OnWindowHidden -= OnWaveCompleted;
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    private void OnWaveCompleted()
    {
        TryRerollPanels();
    }
    public void TryRerollPanels()
    {
        if(_wavesManager.IsAllWavesCompleted)
        {
            Hide();
            return;
        }
        if (PlayerLevelSystem.LevelSystem.SkillPoints <= 0)
        {
            Hide();
            return;
        }
        Show();
        _levelUpPanelsController.RerollPanels();
    }
}
