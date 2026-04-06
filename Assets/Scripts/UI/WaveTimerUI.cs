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
    public void Constuct(WavesManager wavesManager)
    {
        _wavesManager = wavesManager;
    }

    private void Awake()
    {
        WavesManager.Instance.OnWaveStarted += UpdateUI;
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
    private void OnDestroy()
    {
        if (_wavesManager != null)
            _wavesManager.OnWaveStarted -= UpdateUI;
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
