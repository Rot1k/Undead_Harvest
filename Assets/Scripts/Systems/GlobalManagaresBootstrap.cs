using UnityEngine;

public class GlobalManagersBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (GameInput.Instance == null)
        {
            var go = new GameObject(typeof(GameInput).Name);
            go.AddComponent<GameInput>();
        }

        //if (BackgroundMusicManager.Instance == null)
        //{
        //    var go = new GameObject(typeof(BackgroundMusicManager).Name);
        //    go.AddComponent<BackgroundMusicManager>();
        //}
        //if (SoundManager.Instance == null)
        //{
        //    var go = new GameObject(typeof(SoundManager).Name);
        //    go.AddComponent<SoundManager>();
        //}
        //if (MasterVolumeManager.Instance == null)
        //{
        //    var go = new GameObject(typeof(MasterVolumeManager).Name);
        //    go.AddComponent<MasterVolumeManager>();
        //}

    }
}