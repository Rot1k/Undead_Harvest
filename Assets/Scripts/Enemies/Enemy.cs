using NTC.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
[RequireComponent(typeof(StatusEffectsManager))]
public class Enemy : MonoBehaviour, ISpawnable, IDamageable
{
    public event Action<Enemy> OnDied;


    [SerializeField] protected EnemyStatsSO _enemyStats;
    [SerializeField] private Transform _expPrefab;

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
    protected Rigidbody2D Rigidbody => _rigidbody;
    protected Transform Player => _player;
    protected bool IsDead => _isDead;

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

        _isDead = true;
        _collider.enabled = false;
        _rigidbody.simulated = false;

        if (!(WavesManager.Instance.GetCurrentWave().IsBossWave))
        {
            Vector2 deathPos = transform.position;
            NightPool.Spawn(_expPrefab, UnityEngine.Random.insideUnitCircle.normalized * 0.5f + deathPos, Quaternion.identity);
        }
 
        OnDied?.Invoke(this);

        NightPool.Despawn(gameObject, 1f);
    }
    private void OnHealthChanged(object sender, System.EventArgs e)
    {
        Debug.Log($"Enemy Health: {HealthSystem.Health} / {HealthSystem.HealthMax}");
    }

    protected virtual void Start()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
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
        _collider.enabled = true;
        _rigidbody.simulated = true;
        _rigidbody.linearVelocity = Vector2.zero;
        HealthSystem.Heal(_enemyStats.BaseHealth);
        transform.rotation = Quaternion.identity;
        transform.localScale = _localScale;
        _statusEffectsManager.ClearAllEffects();
        Debug.Log($"Enemy spawned with {HealthSystem.Health} health.");
    }
    public void ApplyModifier(StatType type, ModifierType modifierType, float value)
    {
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
}
