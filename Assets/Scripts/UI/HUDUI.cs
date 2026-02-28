using UnityEngine;

public class HUDUI : MonoBehaviour
{
    private void Awake()
    {
        WavesManager.Instance.OnAllWavesCompleted += Hide;
        WavesManager.Instance.OnWaveStarted += Show;
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
