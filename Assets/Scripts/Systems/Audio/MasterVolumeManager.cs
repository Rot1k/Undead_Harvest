using UnityEngine;

public class MasterVolumeManager : MonoBehaviour
{
    public static MasterVolumeManager Instance { get; private set; }

    private const string PLAYER_PREFS_MASTER_VOLUME = "MasterVolume";

    public float Volume { get; private set; } = 1f;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Loader.OnAfterSceneLoad += OnAfterSceneLoad;
        OnAfterSceneLoad();

        AudioListener.volume = Volume;

    }

    public void SetVolume(float value)
    {
        Volume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(PLAYER_PREFS_MASTER_VOLUME, Volume);
        PlayerPrefs.Save();
        AudioListener.volume = Volume;
    }
    private void OnAfterSceneLoad()
    {
        Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MASTER_VOLUME, 1f);
    }
}
