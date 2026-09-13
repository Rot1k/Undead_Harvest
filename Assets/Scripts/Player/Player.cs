using UnityEngine;
using VContainer;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private PlayerStatsSO _baseStats;
    [SerializeField] private StatusEffectsListSO _effects;

    private Rigidbody2D _rigidbody;
    private WavesManager _wavesManager;
    private GameInput _gameInput;
    private SoundManager _soundManager;
    private bool _isCoreInitialized;
    private PlayerMovement _movement;
    private PlayerLevelSystem _level;
    private PlayerHealthSystem _health;

    public PlayerStats Stats { get; private set; }
    public PlayerMovement Movement => _movement;
    public PlayerLevelSystem Level => _level;
    public PlayerHealthSystem Health => _health;
    public HealthSystem HealthSystem => _health.HealthSystem;

    [Inject]
    public void Construct(WavesManager wavesManager, GameInput gameInput, SoundManager soundManager)
    {
        _rigidbody ??= GetComponent<Rigidbody2D>();
        _wavesManager = wavesManager;
        _gameInput = gameInput;
        _soundManager = soundManager;
        InitializeCore();
        _level?.SetSoundManager(_soundManager);
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        InitializeCore();
    }

    public void Initialize()
    {
        InitializeCore();

        if (_movement != null)
            return;

        _movement = new PlayerMovement(_rigidbody, _sprite, Stats, _wavesManager, _gameInput, _health);
    }

    private void InitializeCore()
    {
        if (_isCoreInitialized)
            return;

        _rigidbody ??= GetComponent<Rigidbody2D>();
        Stats = new PlayerStats(_baseStats, _effects);
        _health = new PlayerHealthSystem(Stats);
        _level = new PlayerLevelSystem(Stats, _soundManager);
        _isCoreInitialized = true;
    }

    private void FixedUpdate()
    {
        Movement?.Tick();
    }

    private void Update()
    {
        Health?.Tick(Time.deltaTime);
    }

    private void OnDestroy()
    {
        _movement?.Dispose();
        _health?.Dispose();
        _level?.Dispose();
        Stats?.Dispose();
    }
}
