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
    private void Start()
    {
        _wavesManager.OnWaveStarted += UpdateUI;
        UpdateUI();
    }
    private void OnDestroy()
    {
        if (_wavesManager != null)
            _wavesManager.OnWaveStarted -= UpdateUI;
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
