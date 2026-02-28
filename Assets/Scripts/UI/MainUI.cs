using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private MenuUI _menuUI;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _soundVolumeSlider;

    private void Awake()
    {
        if (_resumeButton != null)
            _resumeButton.onClick.AddListener(() => 
            {
                _menuUI.Hide();
                PauseManager.Instance.SetPlayerPaused(false);
            });
        if (_mainMenuButton != null)
            _mainMenuButton.onClick.AddListener(() => 
            {
                Loader.Load(Loader.Scene.MainMenuScene);
            });

        // MASTER
        if (_masterVolumeSlider != null && MasterVolumeManager.Instance != null)
        {


            _masterVolumeSlider.SetValueWithoutNotify(MasterVolumeManager.Instance.Volume);

            _masterVolumeSlider.onValueChanged.AddListener(value =>
            {
                MasterVolumeManager.Instance.SetVolume(value);
            });
        }

        // MUSIC
        if (_musicVolumeSlider != null && BackgroundMusicManager.Instance != null)
        {

            _musicVolumeSlider.SetValueWithoutNotify(BackgroundMusicManager.Instance.Volume);

            _musicVolumeSlider.onValueChanged.AddListener(value =>
            {
                BackgroundMusicManager.Instance.SetVolume(value);
            });
        }

        // SFX
        if (_soundVolumeSlider != null && SoundManager.Instance != null)
        {

            _soundVolumeSlider.SetValueWithoutNotify(SoundManager.Instance.Volume);

            _soundVolumeSlider.onValueChanged.AddListener(value =>
            {
                SoundManager.Instance.SetVolume(value);
            });
        }
    }
}
