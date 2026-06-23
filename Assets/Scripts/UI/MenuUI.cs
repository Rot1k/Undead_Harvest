using UnityEngine;
using System;
using VContainer;

public class MenuUI : MonoBehaviour
{
    private Action _onPauseAction;
    private GameInput _gameInput;

    [Inject]
    private void Construct(GameInput gameInput)
    {
        _gameInput = gameInput;
    }

    private void Start()
    {
        _onPauseAction = ToggleMenu;

        if (_gameInput != null)
        {
            _gameInput.OnPause += _onPauseAction;
        }

        Hide();
    }

    private void OnDestroy()
    {
        if (_gameInput != null)
        {
            _gameInput.OnPause -= _onPauseAction;
        }
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
