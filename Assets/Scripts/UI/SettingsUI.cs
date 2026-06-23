using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _soundVolumeSlider;

    private GameInput _gameInput;
    private MasterVolumeManager _masterVolumeManager;
    private SoundManager _soundManager;
    private BackgroundMusicManager _backgroundMusicManager;

    [Inject]
    public void Construct(GameInput gameInput,
                          MasterVolumeManager masterVolumeManager,
                          SoundManager soundManager,
                          BackgroundMusicManager backgroundMusicManager
                         )
    {
        _gameInput = gameInput;
        _masterVolumeManager = masterVolumeManager;
        _soundManager = soundManager;
        _backgroundMusicManager = backgroundMusicManager;
    }

    private void Start()
    {
        _closeButton.onClick.AddListener(Hide);

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

        Hide();
    }

    private void OnEnable()
    {
        if (_gameInput != null)
        {
            _gameInput.OnCancel += Hide;
        }
    }

    private void OnDisable()
    {
        if (_gameInput != null)
        {
            _gameInput.OnCancel -= Hide;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
