using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class MainMenuLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
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
