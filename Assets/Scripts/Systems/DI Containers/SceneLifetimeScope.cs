using VContainer;
using VContainer.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLifetimeScope : LifetimeScope
{
    [SerializeField] private WavesManager _wavesManager;
    [SerializeField] private WaveEndWindow _waveEndWindow;
    [SerializeField] private PauseManager _pauseManager;
    [SerializeField] private EquipmentManager _equipmentManager;
    [SerializeField] private WalletManager _walletManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_wavesManager);
        builder.RegisterComponent(_waveEndWindow);
        builder.RegisterComponent(_pauseManager);
        builder.RegisterComponent(_equipmentManager);
        builder.RegisterComponent(_walletManager);

        builder.RegisterBuildCallback(container =>
        {
            Scene scene = gameObject.scene;

            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                container.InjectGameObject(rootGameObject);
            }
        });
    }

}
