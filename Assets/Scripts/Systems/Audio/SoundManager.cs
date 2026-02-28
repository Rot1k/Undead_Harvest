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
    public static SoundManager Instance { get; private set; }

    private const string PLAYER_PREFS_SFX_VOLUME = "SFXVolume";

    [SerializeField] private SoundList[] _audioClips;

    private AudioSource _audioSource;
    public float Volume {get; private set;} = 1f;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Loader.OnAfterSceneLoad += OnAfterSceneLoad;
        OnAfterSceneLoad();

        _audioSource.volume = Volume;
    }

    public static void PlaySound(SoundType soundType, float volume = 1)
    {
        if (Instance == null || Instance._audioClips == null || Instance._audioClips.Length == 0)
        {
            Debug.LogWarning("SoundManager not initialized or no audio clips assigned.");
            return;
        }
        int index = (int)soundType;
        if (index < 0 || index >= Instance._audioClips.Length)
        {
            Debug.LogWarning($"Invalid sound type: {soundType}");
            return;
        }
        AudioClip[] clips = Instance._audioClips[index].AudioClips;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        Instance._audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.101f);

        Instance._audioSource.PlayOneShot(randomClip, volume);
    }
    public void SetVolume(float volume)
    {
        if (_audioSource != null)
        {
            _audioSource.volume = volume;
        }

        PlayerPrefs.SetFloat(PLAYER_PREFS_SFX_VOLUME, volume);
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
    }
}

[Serializable] 
public struct SoundList
{
    public AudioClip[] AudioClips { get => _audioClips; }
    [HideInInspector] public string Name;
    [SerializeField] private AudioClip[] _audioClips;
}