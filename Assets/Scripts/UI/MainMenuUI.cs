using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private SettingsUI _settingsUI;

    private void Awake()
    {
        _playButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.GameScene);
        });
        _settingsButton.onClick.AddListener(() =>
        {
            _settingsUI.Show();
        });
        _quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

    }

}
