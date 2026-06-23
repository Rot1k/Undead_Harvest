using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicManager : MonoBehaviour
{
    private const string PLAYER_PREFS_BACKGROUND_MUSIC_VOLUME = "BackgroundMusicVolume";

    [SerializeField] private AudioClip[] _backgroundMusicClips;
    [SerializeField] private float _delayBetweenTracks = 6f;
    [SerializeField] private float _delayBeforeFirstTrack = 2f;

    private AudioSource _audioSource;
    private List<int> _clipIndices = new List<int>();
    private int _currentIndex = 0;
    private Coroutine _trackCoroutine;
    public float Volume { get; private set; } = 1f;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        Loader.OnAfterSceneLoad += OnAfterSceneLoad;
        OnAfterSceneLoad();

        _audioSource.volume = Volume;
    }

    private void Start()
    {
        if (_backgroundMusicClips == null || _backgroundMusicClips.Length == 0)
        {
            Debug.LogWarning("No background music clips assigned.");
            return;
        }

        PrepareShuffledPlaylist();
        StartCoroutine(PlayMusicWithDelay());
    }


    private void OnDestroy()
    {
        if (_trackCoroutine != null)
        {
            StopCoroutine(_trackCoroutine);
        }

        Loader.OnAfterSceneLoad -= OnAfterSceneLoad;
    }

    private void PrepareShuffledPlaylist()
    {
        _clipIndices.Clear();
        for (int i = 0; i < _backgroundMusicClips.Length; i++)
        {
            _clipIndices.Add(i);
        }

        Shuffle(_clipIndices);
        _currentIndex = 0;
    }

    private void PlayNextTrack()
    {
        if (_clipIndices.Count == 0 || _backgroundMusicClips.Length == 0) return;

        if (_currentIndex >= _clipIndices.Count)
        {
            PrepareShuffledPlaylist();
        }

        int clipIndex = _clipIndices[_currentIndex];
        AudioClip clipToPlay = _backgroundMusicClips[clipIndex];
        _audioSource.clip = clipToPlay;
        _audioSource.Play();
        _currentIndex++;

        _trackCoroutine = StartCoroutine(WaitUntilTrackEnds(clipToPlay.length));
    }

    private IEnumerator PlayMusicWithDelay()
    {
        yield return new WaitForSeconds(_delayBeforeFirstTrack);
        PlayNextTrack();
    }

    private IEnumerator WaitUntilTrackEnds(float clipLength)
    {
        yield return new WaitForSeconds(clipLength + _delayBetweenTracks);
        PlayNextTrack();
    }

    private void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public void SetVolume(float volume)
    {
        Volume = Mathf.Clamp01(volume);

        if (_audioSource != null)
        {
            _audioSource.volume = Volume;
        }
        else
        {
            Debug.LogWarning("AudioSource not initialized.");
        }

        PlayerPrefs.SetFloat(PLAYER_PREFS_BACKGROUND_MUSIC_VOLUME, Volume);
        PlayerPrefs.Save();
    }
    private void OnAfterSceneLoad()
    {
        Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_BACKGROUND_MUSIC_VOLUME, 1f);
        if (_audioSource != null)
        {
            _audioSource.volume = Volume;
        }
    }
}
