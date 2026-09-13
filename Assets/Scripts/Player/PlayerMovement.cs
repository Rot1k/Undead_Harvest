using UnityEngine;

public class PlayerMovement
{
    private readonly PlayerStats _playerStats;
    private readonly Rigidbody2D _rigidbody;
    private readonly SpriteRenderer _sprite;
    private readonly WavesManager _wavesManager;
    private readonly GameInput _gameInput;
    private readonly PlayerHealthSystem _playerHealthSystem;

    private float _moveSpeed => _playerStats.Get(StatType.MoveSpeed);
    private Vector2 _input;
    private bool _isDead = false;

    public bool IsWalking => _input != Vector2.zero;

    public PlayerMovement(
        Rigidbody2D rigidbody,
        SpriteRenderer sprite,
        PlayerStats playerStats,
        WavesManager wavesManager,
        GameInput gameInput,
        PlayerHealthSystem playerHealthSystem)
    {
        _rigidbody = rigidbody;
        _sprite = sprite;
        _playerStats = playerStats;
        _wavesManager = wavesManager;
        _gameInput = gameInput;
        _playerHealthSystem = playerHealthSystem;

        _isDead = false;
        _rigidbody.simulated = true;

        _playerHealthSystem.HealthSystem.OnDead += OnDead;
        _wavesManager.OnWaveStarted += OnWaveStarted;

    }
    public void Dispose()
    {
        _playerHealthSystem.HealthSystem.OnDead -= OnDead;
        _wavesManager.OnWaveStarted -= OnWaveStarted;
    }
    private void OnDead(object sender, System.EventArgs e)
    {
        _isDead = true;
        _rigidbody.simulated = false;
    }

    public void Tick()
    {
        if (_isDead) return;
        Move();
    }

    private void Move()
    {
        _input = _gameInput.MovementVector;

        if (_input.x != 0)
        {
            if (_sprite != null)
                _sprite.flipX = _input.x < 0;
        }

        _rigidbody.linearVelocity = _input.normalized * _moveSpeed;
    }
    private void OnWaveStarted()
    {
        _rigidbody.gameObject.SetActive(true);
        _rigidbody.transform.position = Vector3.zero;
    }

    public Vector2 GetInput()
    {
        return _input;
    }
}
