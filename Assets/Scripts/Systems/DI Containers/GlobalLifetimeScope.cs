using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GlobalLifetimeScope : LifetimeScope
{
    [SerializeField] private GameInput _gameInput;
    [SerializeField] private MasterVolumeManager _masterVolumeManager;
    [SerializeField] private BackgroundMusicManager _backgroundMusicManager;
    [SerializeField] private SoundManager _soundManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_gameInput);
        builder.RegisterComponent(_masterVolumeManager);
        builder.RegisterComponent(_backgroundMusicManager);
        builder.RegisterComponent(_soundManager);
    }
}
