using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event Action OnPause;
    public event Action OnCancel;

    public Vector2 MovementVector => _playerInputActions.Player.Move.ReadValue<Vector2>();

    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.UI.Pause.performed += OnPausePerformed;
        _playerInputActions.UI.Cancel.performed += OnCancelPerformed;
    }

    private void OnPausePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPause?.Invoke();
    }
    private void OnCancelPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCancel?.Invoke();
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
    }
    private void OnDisable()
    {
        DisableAllControls();
    }

    private void OnDestroy()
    {
        if (_playerInputActions == null)
        {
            return;
        }

        _playerInputActions.UI.Pause.performed -= OnPausePerformed;
        _playerInputActions.UI.Cancel.performed -= OnCancelPerformed;
        _playerInputActions.Dispose();
    }

    public void DisableAllControls()
    {
        if (_playerInputActions == null)
        {
            return;
        }

        _playerInputActions.Disable();
    }
}
