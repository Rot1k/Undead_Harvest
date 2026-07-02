using UnityEngine;
using TMPro;
using VContainer;
public class WaveCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _waveText;
    private WavesManager _wavesManager;

    [Inject]
    public void Construct(WavesManager wavesManager)
    {
        _wavesManager = wavesManager;
    }

    public void Initialize()
    {
        if (_wavesManager != null)
            _wavesManager.OnWaveStarted += UpdateUI;

        if (_wavesManager != null)
            UpdateUI();
    }
    private void Start()
    {
        // Initialization handled in StartGame called by UIBootstrap.
    }

    public void Dispose()
    {
        if (_wavesManager != null)
            _wavesManager.OnWaveStarted -= UpdateUI;
    }

    private void OnDestroy()
    {
        Dispose();
    }

    private void OnEnable()
    {
        if(_wavesManager != null)
            UpdateUI();
    }
    private void UpdateUI()
    {
        _waveText.text = $"Wave {_wavesManager.CurrentWave + 1}";
    }
}
