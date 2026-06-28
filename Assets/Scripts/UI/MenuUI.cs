using UnityEngine;
using System;
using VContainer;

public class MenuUI : MonoBehaviour
{
    private Action _onPauseAction;
    private GameInput _gameInput;

    [Inject]
    public void Construct(GameInput gameInput)
    {
        _gameInput = gameInput;
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

    private void Start()
    {
        // Initialization handled in Initialize called by UIBootstrap
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
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
