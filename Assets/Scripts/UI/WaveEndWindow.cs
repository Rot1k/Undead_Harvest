using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using VContainer;

public class WaveEndWindow : MonoBehaviour
{

    public event Action OnWindowHidden;
    public event Action OnWindowHiddenAllWavesCompleted;
    public event Action OnWindowHiddenPlayerDead;

    [SerializeField] private PlayerHealthSystem _playerHealthSystem;
    [SerializeField] private TextMeshProUGUI _windowText;
    [SerializeField] private float _textDuration = 2f;

    private Sequence _sequence;

    private WavesManager _wavesManager;

    [Inject]
    public void Constuct(WavesManager wavesManager)
    {
        _wavesManager = wavesManager;
    }

    private void Start()
    {
        if(_wavesManager == null)
        {
            Debug.LogError("WavesManager instance is null!");
            return;
        }
        _wavesManager.OnWaveCompleted += OnWaveCompleted;
        _playerHealthSystem.HealthSystem.OnDead += OnPlayerDead;
        Hide();
    }

    private void OnDestroy()
    {
        if (_wavesManager != null)
            _wavesManager.OnWaveCompleted -= OnWaveCompleted;

        if (_playerHealthSystem != null && _playerHealthSystem.HealthSystem != null)
            _playerHealthSystem.HealthSystem.OnDead -= OnPlayerDead;
    }

    private void OnWaveCompleted()
    {
        ShowMessage(
            $"Wave {_wavesManager.CurrentWave + 1} Completed!",
            () =>
            {
                if (_wavesManager.IsAllWavesCompleted)
                    OnWindowHiddenAllWavesCompleted?.Invoke();
                else
                    OnWindowHidden?.Invoke();
            }
        );
    }

    private void OnPlayerDead(object sender, 
        EventArgs e)
    {
        ShowMessage(
            "You Died!",
            () => OnWindowHiddenPlayerDead?.Invoke()
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
