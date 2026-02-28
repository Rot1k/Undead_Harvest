using System;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        GameScene,
        LoadingScene,
    }

    //public static event Action OnBeforeSceneLoad;
    public static event Action OnAfterSceneLoad;

    private static Scene _targetScene;

    public static void Load(Scene targetScene)
    {
        _targetScene = targetScene;

        //OnBeforeSceneLoad?.Invoke();

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(_targetScene.ToString());

        OnAfterSceneLoad?.Invoke();
    }
}
