using UnityEngine;
using System;

public class MenuUI : MonoBehaviour
{
    private Action _onPauseAction;

    private void Awake()
    {
        _onPauseAction = ToggleMenu;
        GameInput.Instance.OnPause += _onPauseAction;

        Hide();
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnPause -= _onPauseAction;
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
