using UnityEngine;
using TMPro;
public class WaveCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _waveText;

    private void Awake()
    {
        WavesManager.Instance.OnWaveStarted += UpdateUI;
    }
    private void OnDestroy()
    {
        if (WavesManager.Instance != null)
            WavesManager.Instance.OnWaveStarted -= UpdateUI;
    }
    private void OnEnable()
    {
        if(WavesManager.Instance != null)
            UpdateUI();
    }
    private void UpdateUI()
    {
        _waveText.text = $"Wave {WavesManager.Instance.CurrentWave + 1}";
    }
}
