using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _waveTimerText;

    private void Awake()
    {
        WavesManager.Instance.OnWaveStarted += UpdateUI;
    }
    private void UpdateUI()
    {
        var wave = WavesManager.Instance.GetCurrentWave();
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
        if (WavesManager.Instance != null)
            WavesManager.Instance.OnWaveStarted -= UpdateUI;
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
