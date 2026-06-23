using System;
using UnityEngine;

public enum SoundType
{
    HIT,
    SHOOT,
    BUY,
    MENU,
    ONDEAD,
    ONWIN,
    ONLOSE,
    LEVELUP,
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SFX_VOLUME = "SFXVolume";

    [SerializeField] private SoundList[] _audioClips;

    private AudioSource _audioSource;

    public float Volume { get; private set; } = 1f;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        Loader.OnAfterSceneLoad += OnAfterSceneLoad;
        OnAfterSceneLoad();

        _audioSource.volume = Volume;
    }

    private void OnDestroy()
    {
        Loader.OnAfterSceneLoad -= OnAfterSceneLoad;
    }

    public void PlaySound(SoundType soundType, float volume = 1)
    {
        if (_audioClips == null || _audioClips.Length == 0)
        {
            Debug.LogWarning("SoundManager not initialized or no audio clips assigned.");
            return;
        }

        int index = (int)soundType;
        if (index < 0 || index >= _audioClips.Length)
        {
            Debug.LogWarning($"Invalid sound type: {soundType}");
            return;
        }

        AudioClip[] clips = _audioClips[index].AudioClips;
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"No audio clips assigned for sound type: {soundType}");
            return;
        }

        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        _audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.101f);

        _audioSource.PlayOneShot(randomClip, volume);
    }

    public void SetVolume(float volume)
    {
        Volume = Mathf.Clamp01(volume);

        if (_audioSource != null)
        {
            _audioSource.volume = Volume;
        }

        PlayerPrefs.SetFloat(PLAYER_PREFS_SFX_VOLUME, Volume);
        PlayerPrefs.Save();
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref _audioClips, names.Length);
        for (int i = 0; i < names.Length; i++)
        {
            _audioClips[i].Name = names[i];
        }
    }
#endif
    private void OnAfterSceneLoad()
    {
        Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SFX_VOLUME, 1f);
        if (_audioSource != null)
        {
            _audioSource.volume = Volume;
        }
    }
}

[Serializable] 
public struct SoundList
{
    public AudioClip[] AudioClips { get => _audioClips; }
    [HideInInspector] public string Name;
    [SerializeField] private AudioClip[] _audioClips;
}
