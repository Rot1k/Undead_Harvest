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

    private void Start()
    {
        if (_resumeButton != null)
        {
            _resumeButton.onClick.AddListener(() => 
            {
                _menuUI.Hide();
                _pauseManager.SetPlayerPaused(false);
            });
        }

        if (_mainMenuButton != null)
        {
            _mainMenuButton.onClick.AddListener(() => 
            {
                Loader.Load(Loader.Scene.MainMenuScene);
            });
        }

        // MASTER
        if (_masterVolumeSlider != null && _masterVolumeManager != null)
        {
            _masterVolumeSlider.SetValueWithoutNotify(_masterVolumeManager.Volume);

            _masterVolumeSlider.onValueChanged.AddListener(value =>
            {
                _masterVolumeManager.SetVolume(value);
            });
        }

        // MUSIC
        if (_musicVolumeSlider != null && _backgroundMusicManager != null)
        {

            _musicVolumeSlider.SetValueWithoutNotify(_backgroundMusicManager.Volume);

            _musicVolumeSlider.onValueChanged.AddListener(value =>
            {
                _backgroundMusicManager.SetVolume(value);
            });
        }

        // SFX
        if (_soundVolumeSlider != null && _soundManager != null)
        {

            _soundVolumeSlider.SetValueWithoutNotify(_soundManager.Volume);

            _soundVolumeSlider.onValueChanged.AddListener(value =>
            {
                _soundManager.SetVolume(value);
            });
        }
    }
}
