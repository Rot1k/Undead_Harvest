using UnityEngine;
using VContainer;

[RequireComponent(typeof(PlayerStats), typeof(Rigidbody2D), typeof(PlayerHealthSystem))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerStats _playerStats;
    private float _moveSpeed => _playerStats.Get(StatType.MoveSpeed);

    [SerializeField] private SpriteRenderer _sprite;

    private PlayerHealthSystem _playerHealthSystem;
    private Rigidbody2D _rigidbody;
    private Vector2 _input;
    private bool _isDead = false;

    public bool IsWalking => _input != Vector2.zero;

    private WavesManager _wavesManager;

    [Inject]
    public void Constuct(WavesManager wavesManager)
    {
        _wavesManager = wavesManager;
    }
    private void Awake()
    {
        _playerHealthSystem = GetComponent<PlayerHealthSystem>();
        _playerStats = GetComponent<PlayerStats>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _wavesManager.OnWaveStarted += OnWaveStarted;
    }
    private void OnEnable()
    {
        _playerHealthSystem.HealthSystem.OnDead += OnDead;
    }
    private void OnDead(object sender, System.EventArgs e)
    {
        //_isDead = true;
        //_rigidbody.simulated = false;
        //_gameInput.DisableAllControls();
    }

    private void FixedUpdate()
    {
        if (_isDead) return;
        Move();
    }

    private void OnDisable()
    {
        _rigidbody.linearVelocity = Vector2.zero;
        _playerHealthSystem.HealthSystem.OnDead -= OnDead;
    }

    private void Move()
    {
        _input = GameInput.Instance.MovementVector;

        if (_input.x != 0)
        {
            _sprite.flipX = _input.x < 0;
        }

        _rigidbody.linearVelocity = _input.normalized * _moveSpeed;
    }
    private void OnWaveStarted()
    {
        gameObject.SetActive(true);
        gameObject.transform.position = Vector3.zero;
    }

    public Vector2 GetInput()
    {
        return _input;
    }
}