using System;
using System.Collections;
using TMPro;
using UnityEngine;
using VContainer;

public class WaveTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _waveTimerText;

    private WavesManager _wavesManager;
    private Coroutine _timerCoroutine;

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
    public void Dispose()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }

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
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }

            Hide();
            return;
        }
        gameObject.SetActive(true);
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }

        _timerCoroutine = StartCoroutine(Timer(waveDuration));
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
        _timerCoroutine = null;
    }
}
