using UnityEngine;
using TMPro;
using UnityEngine.UI;
using VContainer;

public class WinLoseWindowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;

    private SoundManager _soundManager;


    [Inject]
    public void Construct(SoundManager soundManager)
    {
        _soundManager = soundManager;
    }

    public void Initialize()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        _restartButton.onClick.AddListener(OnRestartButtonClicked);
    }

    public void Dispose()
    {
        if (_mainMenuButton != null)
            _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);

        if (_restartButton != null)
            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
    }

    private void OnDestroy()
    {
        Dispose();
    }

    private void OnMainMenuButtonClicked()
    {
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    private void OnRestartButtonClicked()
    {
        Loader.Load(Loader.Scene.GameScene);
    }

    public void ShowWin()
    {
        _resultText.text = "CONGRATULATION!";
        _soundManager.PlaySound(SoundType.ONWIN);
        gameObject.SetActive(true);
    }

    public void ShowLose()
    {
        _resultText.text = "YOU LOSE!";
        _soundManager.PlaySound(SoundType.ONLOSE);
        gameObject.SetActive(true);
    }
}
