using UnityEngine;
using VContainer;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponVisual _weaponVisual;

    protected WeaponSO _weaponBaseStats;
    protected PlayerMovement _playerMovement;
    protected PlayerStats _playerStats;
    protected SpriteRenderer _spriteRenderer;
    protected SoundManager _soundManager;
    private EnemyRegistry _enemyRegistry;

    protected Vector3 OriginalLocalPosition;

    protected Transform _lockedEnemy;
    protected Enemy _lockedEnemyComponent;
    private float _targetLoseTime;
    [SerializeField] private float _targetGraceTime = 0.2f;
    [SerializeField] private float _targetSwitchDelay = 0.15f;
    private Transform _candidateEnemy;
    private float _candidateSince;

    protected float _nextFireTime = 0f;

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [Inject]
    public void Construct(SoundManager soundManager, EnemyRegistry enemyRegistry)
    {
        _soundManager = soundManager;
        _enemyRegistry = enemyRegistry;
    }

    public void Initialize(Player player, WeaponSO weaponSO)
    {
        if (player == null || weaponSO == null)
        {
            Debug.LogError($"[{name}] Weapon initialization requires a player and weapon configuration.", this);
            return;
        }

        _playerStats = player.Stats;
        _playerMovement = player.Movement;
        SetStats(weaponSO);
    }


    private bool IsLockedEnemyValid()
    {
        if (_lockedEnemy == null || _lockedEnemyComponent == null)
            return false;

        return _lockedEnemy.gameObject.activeInHierarchy && !_lockedEnemyComponent.IsDead;
    }

    protected void UpdateTarget()
    {
        if (!IsLockedEnemyValid())
        {
            _lockedEnemy = null;
            _lockedEnemyComponent = null;
        }

        Transform found = FindNearestEnemy();

        if (found != null)
        {
            _targetLoseTime = Time.time + _targetGraceTime;

            if (found == _lockedEnemy)
            {
                _candidateEnemy = null;
                return;
            }

            if (_candidateEnemy != found)
            {
                _candidateEnemy = found;
                _candidateSince = Time.time;
            }

            if (_lockedEnemy == null || Time.time - _candidateSince >= _targetSwitchDelay)
            {
                _lockedEnemy = found;
                _lockedEnemyComponent = found.GetComponent<Enemy>();
                _candidateEnemy = null;
            }
        }
        else
        {
            _candidateEnemy = null;
            if (Time.time > _targetLoseTime)
            {
                _lockedEnemy = null;
                _lockedEnemyComponent = null;
            }
        }
    }

    protected bool HasTarget => IsLockedEnemyValid();

    protected Vector2 GetAimDirection(Vector3 fromPosition)
    {
        if (!IsLockedEnemyValid())
            return Vector2.zero;

        return (_lockedEnemy.position - fromPosition).normalized;
    }

    protected Transform FindNearestEnemy()
    {
        float range = _playerStats.Get(StatType.AttackRange) * _weaponBaseStats.RangeMultiplier;
        float rangeSqr = range * range;

        Vector2 origin = transform.position;

        Transform found = null;
        float closest = float.MaxValue;

        if (_enemyRegistry == null)
            return null;

        foreach (var enemy in _enemyRegistry.GetAll())
        {
            if (enemy == null || enemy.IsDead || !enemy.gameObject.activeInHierarchy) continue;

            Vector2 dir = (Vector2)enemy.transform.position - origin;
            float dist = dir.sqrMagnitude;

            if (dist <= rangeSqr && dist < closest)
            {
                closest = dist;
                found = enemy.transform;
            }
        }
        return found;
    }
    protected void LookAtTargetOrInput()
    {
        Vector2 dir;

        if (!IsLockedEnemyValid())
        {
            Vector2 input = _playerMovement.GetInput();
            if (input.sqrMagnitude <= 0.01f)
                return;

            dir = input;
        }
        else
        {
            dir = _lockedEnemy.position - transform.position;
        }

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        _spriteRenderer.flipY = dir.x < 0;
    }

    protected bool CanShoot(float time)
    {
        return HasTarget && time >= _nextFireTime;
    }

    public float CalculateDamage(float projectileBaseDamage)
    {
        return CombatSystem.CalculateDamage(_playerStats, _weaponBaseStats, projectileBaseDamage);
    }

    protected void ApplyCooldown(float currentTime)
    {
        float baseAPS = _weaponBaseStats.BaseAttackSpeed;
        float playerMultiplier = _playerStats.Get(StatType.AttackSpeedMultiplier);

        float finalAPS = baseAPS * playerMultiplier;
        float cooldown = 1f / Mathf.Max(finalAPS, 0.01f);

        _nextFireTime = currentTime + cooldown;
    }

    public void HandleOnHit(Enemy enemy)
    {
        if (enemy == null || _weaponBaseStats == null || _playerStats == null)
            return;

        ApplyWeaponEffects(enemy);
        ApplyPlayerEffects(enemy);
    }
    private void ApplyWeaponEffects(Enemy enemy)
    {
        foreach (var effectData in _weaponBaseStats.OnHitEffects)
        {
            float finalChance =
                effectData.Chance *
                _playerStats.Get(StatType.GlobalEffectChanceMultiplier);

            if (Random.value <= finalChance)
            {
                enemy.ApplyEffect(effectData.Effect);
            }
        }
    }
    private void ApplyPlayerEffects(Enemy enemy)
    {
        _playerStats.TryApplyEffects(enemy);
    }
    public virtual void UpdateStartPosition(Vector3 localPosition)
    {
        OriginalLocalPosition = localPosition;
    }
    public virtual void SetStats(WeaponSO weaponStats)
    {
        if (weaponStats == null)
            return;

        _weaponBaseStats = weaponStats;
        _weaponVisual?.Apply(_weaponBaseStats);
    }
    public void ResetState()
    {
        _nextFireTime = Time.time;

        _lockedEnemy = null;
        _lockedEnemyComponent = null;
        _targetLoseTime = 0f;
        _candidateEnemy = null;
        _candidateSince = 0f;

        transform.localPosition = OriginalLocalPosition;
        transform.rotation = Quaternion.identity;
        _spriteRenderer.flipY = false;
    }
}
