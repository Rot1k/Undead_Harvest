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

    protected Vector3 OriginalLocalPosition;

    protected Transform _lockedEnemy;
    protected Enemy _lockedEnemyComponent;
    private float _targetLoseTime;
    [SerializeField] private float _targetGraceTime = 0.2f;

    protected float _nextFireTime = 0f;

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [Inject]
    public void Construct(SoundManager soundManager)
    {
        _soundManager = soundManager;
    }

    public void Initialize(PlayerStats playerStats, PlayerMovement playerMovement, WeaponSO weaponSO)
    {
        _playerStats = playerStats;
        _playerMovement = playerMovement;
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
            _lockedEnemy = found;
            _lockedEnemyComponent = found.GetComponent<Enemy>();
            _targetLoseTime = Time.time + _targetGraceTime;
        }
        else if (Time.time > _targetLoseTime)
        {
            _lockedEnemy = null;
            _lockedEnemyComponent = null;
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

        foreach (var enemy in EnemyRegistry.GetAll())
        {
            if (enemy == null || enemy.IsDead) continue;

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
        _weaponBaseStats = weaponStats;
        _weaponVisual.Apply(_weaponBaseStats);
    }
    public void ResetState()
    {
        _nextFireTime = Time.time;

        _lockedEnemy = null;
        _lockedEnemyComponent = null;
        _targetLoseTime = 0f;

        transform.localPosition = OriginalLocalPosition;
        transform.rotation = Quaternion.identity;
        _spriteRenderer.flipY = false;
    }
}
