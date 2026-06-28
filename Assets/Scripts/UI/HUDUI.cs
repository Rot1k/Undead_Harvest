using UnityEngine;
using VContainer;

public class HUDUI : MonoBehaviour
{
    private WavesManager _wavesManager;

    [Inject]
    public void Construct(WavesManager wavesManager)
    {
        _wavesManager = wavesManager;
    }

    public void Initialize()
    {
        if (_wavesManager != null)
        {
            _wavesManager.OnAllWavesCompleted += Hide;
            _wavesManager.OnWaveStarted += Show;
        }
    }

    private void Start()
    {
        // Initialization handled in Initialize called by UIBootstrap
    }

    public void Dispose()
    {
        if (_wavesManager == null)
            return;

        _wavesManager.OnAllWavesCompleted -= Hide;
        _wavesManager.OnWaveStarted -= Show;
    }

    private void OnDestroy()
    {
        Dispose();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
