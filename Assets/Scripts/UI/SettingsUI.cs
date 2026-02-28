using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _soundVolumeSlider;

    private void Awake()
    {
        _closeButton.onClick.AddListener(Hide);

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

        Hide();
    }

    private void OnEnable()
    {
        if (GameInput.Instance != null)
            GameInput.Instance.OnCancel += Hide;
    }

    private void OnDisable()
    {
        if (GameInput.Instance != null)
            GameInput.Instance.OnCancel -= Hide;
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
