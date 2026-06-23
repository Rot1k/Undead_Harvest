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
    private void Start()
    {
        _wavesManager.OnAllWavesCompleted += Hide;
        _wavesManager.OnWaveStarted += Show;
    }
    private void OnDestroy()
    {
        if (_wavesManager == null)
            return;

        _wavesManager.OnAllWavesCompleted -= Hide;
        _wavesManager.OnWaveStarted -= Show;
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
