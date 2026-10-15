using UnityEngine;
using System;
using VContainer;

public class MenuUI : MonoBehaviour
{
    private Action _onPauseAction;
    private GameInput _gameInput;
    private PauseManager _pauseManager;

    [Inject]
    public void Construct(GameInput gameInput, PauseManager pauseManager)
    {
        _gameInput = gameInput;
        _pauseManager = pauseManager;
    }

    public void Initialize()
    {
        _onPauseAction = ToggleMenu;
        if (_gameInput != null)
        {
            _gameInput.OnPause += _onPauseAction;
        }

        Hide();
    }

    public void Dispose()
    {
        if (_gameInput != null)
        {
            _gameInput.OnPause -= _onPauseAction;
        }
    }

    private void OnDestroy()
    {
        Dispose();
    }

    private void ToggleMenu()
    {
        if (gameObject.activeSelf)
            Hide();
        else
            Show();
    }

    public void Hide()
    {
        if (gameObject.activeSelf)
            _pauseManager?.Unpause();

        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (!gameObject.activeSelf)
            _pauseManager?.Pause();

        gameObject.SetActive(true);
    }
}
