using NTC.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
[RequireComponent(typeof(StatusEffectsManager))]
public class Enemy : MonoBehaviour, ISpawnable, IDamageable
{
    public event Action<Enemy> OnDied;


    [SerializeField] protected EnemyStatsSO _enemyStats;
    [SerializeField] private GameObject _expPrefab;

    public EnemyStatsSO EnemyStats => _enemyStats;

    public HealthSystem HealthSystem { get; private set; }
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private Transform _player;
    private StatusEffectsManager _statusEffectsManager;
    private bool _isDead = false;

    private Dictionary<StatType, float> _runtimeStats = new();


    private readonly float _threshold = 0.1f;
    private readonly float _stopDistance = 0.15f; // Distance at which the enemy stops moving towards the player
    private Vector3 _localScale = Vector3.one;
    private IObjectResolver _objectResolver;

    private readonly float _healthMultiplierPerWave = 0.10f;
    protected Rigidbody2D Rigidbody => _rigidbody;
    protected Transform Player => _player;
    protected WavesManager _wavesManager;
    public bool IsDead => _isDead;

    [Inject]
    public void Construct(WavesManager wavesManager, IObjectResolver objectResolver)
    {
        _wavesManager = wavesManager;
        _objectResolver = objectResolver;
    }
    protected virtual void Awake()
    {
        _runtimeStats[StatType.MoveSpeed] = _enemyStats.BaseMoveSpeed;
        _runtimeStats[StatType.AttackDamage] = _enemyStats.BaseDamage;

        HealthSystem = new HealthSystem(_enemyStats.BaseHealth);

        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _statusEffectsManager = GetComponent<StatusEffectsManager>();

        _localScale = transform.localScale;
    }
    private void OnEnable()
    {
        HealthSystem.OnDead += OnDead;
        HealthSystem.OnHealthChanged += OnHealthChanged;
    }
    private void OnDisable()
    {
        HealthSystem.OnDead -= OnDead;
        HealthSystem.OnHealthChanged -= OnHealthChanged;
    }
    protected virtual void OnDead(object sender, System.EventArgs e)
    {
        if(_isDead) return;

        ResetEnemy();

        if (_wavesManager == null || _wavesManager.GetCurrentWave().IsBossWave != true)
        {
            Vector2 deathPos = transform.position;
            GameObject expInstance = NightPool.Spawn(_expPrefab, UnityEngine.Random.insideUnitCircle.normalized * 0.5f + deathPos, Quaternion.identity);
            _objectResolver.InjectGameObject(expInstance);

        }
 
        OnDied?.Invoke(this);

        NightPool.Despawn(gameObject, 1f);
    }
    private void OnHealthChanged(object sender, System.EventArgs e)
    {
    }

    protected virtual void FixedUpdate()
    {
        if (_isDead) return;
        MoveToPlayer();
    }

    protected void MoveToPlayer()
    {
        Vector2 direction = (_player.position - transform.position);
        if (direction.magnitude > _stopDistance)
        {
            direction = direction.normalized;
            Turn(direction);
            _rigidbody.linearVelocity = direction * _runtimeStats[StatType.MoveSpeed];
        }
        else
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }

    }

    protected void Turn(Vector2 direction)
    {
        if (direction.x > _threshold)
        {
            _spriteRenderer.flipX = false;
        }
        else if (direction.x < -_threshold)
        {
            _spriteRenderer.flipX = true;
        }
    }
    public int GetDamage() => (int)_runtimeStats[StatType.AttackDamage];

    public virtual void OnSpawn()
    {
        _isDead = false;
        EnemyRegistry.Register(this);

        // Reset runtime stats to base values for pooling
        _runtimeStats[StatType.MoveSpeed] = _enemyStats.BaseMoveSpeed;
        _runtimeStats[StatType.AttackDamage] = _enemyStats.BaseDamage;

        _collider.enabled = true;
        _rigidbody.simulated = true;
        _rigidbody.linearVelocity = Vector2.zero;
        int currentWave = _wavesManager != null ? _wavesManager.CurrentWave : 0;
        HealthSystem.SetMaxHealth(Mathf.RoundToInt(_enemyStats.BaseHealth + (_enemyStats.BaseHealth * (currentWave * _healthMultiplierPerWave))));
        HealthSystem.Heal(HealthSystem.HealthMax);
        transform.rotation = Quaternion.identity;
        transform.localScale = _localScale;
        _statusEffectsManager.ClearAllEffects();
    }
    public virtual void OnDespawn()
    {
        if(_isDead) return;
        ResetEnemy();
    }
    public void ApplyModifier(StatType type, ModifierType modifierType, float value)
    {
        if(_isDead) return;

        if (!_runtimeStats.ContainsKey(type))
        {
            Debug.LogWarning($"Enemy {name} не має StatType {type}, modifier пропущено");
            return;
        }
        
        switch (modifierType)
        {
            case ModifierType.Flat:
                _runtimeStats[type] += value;
                break;
            case ModifierType.AddPercent:
                _runtimeStats[type] += _runtimeStats[type] * value / 100f;
                break;
            case ModifierType.Multiplier:
                _runtimeStats[type] *= value;
                break;
        }
    }
    public void ApplyEffect(StatusEffectSO statusEffectSO)
    {
        if(_isDead) return;

        _statusEffectsManager.ApplyEffect(statusEffectSO);
    }
    public void Init(Transform playerTransform)
    {
        _player = playerTransform;
    }
    private void ResetEnemy()
    {
        _isDead = true;
        EnemyRegistry.Unregister(this);
        _collider.enabled = false;
        _rigidbody.simulated = false;
        _statusEffectsManager.ClearAllEffects();
    }
}
