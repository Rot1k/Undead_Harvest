using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WinLoseWindowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private PlayerHealthSystem _playerHealthSystem;

    private void Awake()
    {
        gameObject.SetActive(false);

        WaveEndWindow.Instance.OnWindowHiddenAllWavesCompleted += OnAllWavesCompleted;
        WaveEndWindow.Instance.OnWindowHiddenPlayerDead += OnPlayerDead;

        _mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        _restartButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.GameScene);
        });
    }

    private void OnDestroy()
    {
        if (WaveEndWindow.Instance != null)
        {
            WaveEndWindow.Instance.OnWindowHiddenAllWavesCompleted -= OnAllWavesCompleted;
            WaveEndWindow.Instance.OnWindowHiddenPlayerDead -= OnPlayerDead;
        }

    }

    private void OnAllWavesCompleted()
    {
        _resultText.text = "CONGRATULATION!";
        SoundManager.PlaySound(SoundType.ONWIN);
        gameObject.SetActive(true);
    }

    private void OnPlayerDead()
    {
        _resultText.text = "YOU LOSE!";
        SoundManager.PlaySound(SoundType.ONLOSE);
        gameObject.SetActive(true);
    }
}
