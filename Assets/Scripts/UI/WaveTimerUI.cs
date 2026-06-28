using System;
using System.Collections;
using TMPro;
using UnityEngine;
using VContainer;

public class WaveTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _waveTimerText;

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
    }

    private void Start()
    {
        // Initialization handled in Initialize called by UIBootstrap
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

    private void UpdateUI()
    {
        var wave = _wavesManager.GetCurrentWave();
        if (wave == null)
            return;
        int waveDuration = wave.WaveDuration;
        if (waveDuration <= 0f)
        {
            Hide();
            return;
        }
        gameObject.SetActive(true);
        StartCoroutine(Timer(waveDuration));
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private IEnumerator Timer(int seconds)
    {
        while (seconds > 0)
        {
            _waveTimerText.text = $"{seconds}";
            yield return new WaitForSeconds(1f);
            seconds--;
        }
        Hide();
    }
}
