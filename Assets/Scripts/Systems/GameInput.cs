using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event Action OnPause;
    public event Action OnCancel;

    public Vector2 MovementVector => _playerInputActions.Player.Move.ReadValue<Vector2>();

    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

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
    public void DisableAllControls()
    {
        if (_playerInputActions == null)
            return;

        _playerInputActions.Disable();
    }
}
