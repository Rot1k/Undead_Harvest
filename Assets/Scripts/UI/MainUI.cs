using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class MainUI : MonoBehaviour
{
    [SerializeField] private MenuUI _menuUI;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _soundVolumeSlider;

    private PauseManager _pauseManager;
    private MasterVolumeManager _masterVolumeManager;
    private SoundManager _soundManager;
    private BackgroundMusicManager _backgroundMusicManager;

    [Inject]
    public void Construct(PauseManager pauseManager,
                          MasterVolumeManager masterVolumeManager,
                          SoundManager soundManager,
                          BackgroundMusicManager backgroundMusicManager
                         )
    {
        _pauseManager = pauseManager;
        _masterVolumeManager = masterVolumeManager;
        _soundManager = soundManager;
        _backgroundMusicManager = backgroundMusicManager;
    }

    public void Initialize()
    {
        if (_resumeButton != null)
        {
            _resumeButton.onClick.AddListener(OnResumeButtonClicked);
        }

        if (_mainMenuButton != null)
        {
            _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        if (_masterVolumeSlider != null && _masterVolumeManager != null)
        {
            _masterVolumeSlider.SetValueWithoutNotify(_masterVolumeManager.Volume);
            _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }

        if (_musicVolumeSlider != null && _backgroundMusicManager != null)
        {
            _musicVolumeSlider.SetValueWithoutNotify(_backgroundMusicManager.Volume);
            _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (_soundVolumeSlider != null && _soundManager != null)
        {
            _soundVolumeSlider.SetValueWithoutNotify(_soundManager.Volume);
            _soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
        }
    }

    public void Dispose()
    {
        if (_resumeButton != null)
            _resumeButton.onClick.RemoveListener(OnResumeButtonClicked);

        if (_mainMenuButton != null)
            _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);

        if (_masterVolumeSlider != null)
            _masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);

        if (_musicVolumeSlider != null)
            _musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);

        if (_soundVolumeSlider != null)
            _soundVolumeSlider.onValueChanged.RemoveListener(OnSoundVolumeChanged);
    }

    private void OnDestroy()
    {
        Dispose();
    }

    private void OnResumeButtonClicked()
    {
        _menuUI.Hide();
        _pauseManager.SetPlayerPaused(false);
    }

    private void OnMainMenuButtonClicked()
    {
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    private void OnMasterVolumeChanged(float value)
    {
        _masterVolumeManager.SetVolume(value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        _backgroundMusicManager.SetVolume(value);
    }

    private void OnSoundVolumeChanged(float value)
    {
        _soundManager.SetVolume(value);
    }
}
