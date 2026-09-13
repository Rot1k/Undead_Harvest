using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class WaveEndWindow : MonoBehaviour
{

    public event Action OnWaveMessageHidden;
    public event Action OnFinalWaveMessageHidden;
    public event Action OnPlayerDeathMessageHidden;

    
    [SerializeField] private TextMeshProUGUI _windowText;
    [SerializeField] private float _textDuration = 2f;

    private Sequence _sequence;

    public void Initialize()
    {
        Hide();
    }

    public void Dispose()
    {
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public void ShowWaveCompleted(int waveNumber, bool isFinalWave)
    {
        ShowMessage(
            $"Wave {waveNumber} Completed!",
            () =>
            {
                if (isFinalWave)
                    OnFinalWaveMessageHidden?.Invoke();
                else
                    OnWaveMessageHidden?.Invoke();
            }
        );
    }

    public void ShowPlayerDied()
    {
        ShowMessage(
            "You Died!",
            () => OnPlayerDeathMessageHidden?.Invoke()
        );
    }


    private void ShowMessage(string fullText, Action onHiddenCallback)
    {
        _sequence?.Kill();

        gameObject.SetActive(true);
        _windowText.text = "";

        _sequence = DOTween.Sequence();
        _sequence.SetUpdate(true);

        _sequence
            .Append(CreateTypewriterTween(fullText, _textDuration))
            .AppendInterval(1f)
            .AppendCallback(() =>
            {
                Hide();
                onHiddenCallback?.Invoke();
            });
    }


    private Tween CreateTypewriterTween(string fullText, float duration)
    {
        int totalChars = fullText.Length;

        return DOTween.To(
            () => 0,
            value =>
            {
                int count = Mathf.Clamp(value, 0, totalChars);
                _windowText.text = fullText.Substring(0, count);
            },
            totalChars,
            duration
        ).SetEase(Ease.Linear);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
