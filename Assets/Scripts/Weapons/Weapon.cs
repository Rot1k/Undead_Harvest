using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponVisual _weaponVisual;

    protected WeaponSO _weaponBaseStats;
    protected PlayerMovement _playerMovement;
    protected PlayerStats _playerStats;
    protected SpriteRenderer _spriteRenderer;

    protected Vector3 OriginalLocalPosition;

    protected Transform _lockedEnemy;
    private float _targetLoseTime;
    [SerializeField] private float _targetGraceTime = 0.2f;

    protected float _nextFireTime = 0f;

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Initialize(PlayerStats playerStats, PlayerMovement playerMovement, WeaponSO weaponSO)
    {
        _playerStats = playerStats;
        _playerMovement = playerMovement;
        SetStats(weaponSO);
    }


    protected void UpdateTarget()
    {
        Transform found = FindNearestEnemy();

        if (found != null)
        {
            _lockedEnemy = found;
            _targetLoseTime = Time.time + _targetGraceTime;
        }
        else if (Time.time > _targetLoseTime)
        {
            _lockedEnemy = null;
        }
    }

    protected bool HasTarget => _lockedEnemy != null;

    protected Vector2 GetAimDirection(Vector3 fromPosition)
    {
        if (_lockedEnemy == null)
            return Vector2.zero;

        return (_lockedEnemy.position - fromPosition).normalized;
    }

    protected Transform FindNearestEnemy()
    {
        Transform found = null;
        float closestDistance = float.MaxValue;

        float range = _playerStats.Get(StatType.AttackRange) * _weaponBaseStats.RangeMultiplier;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, LayerMask.GetMask("Enemy")
        );

        foreach (var enemy in enemies)
        {
            if (!enemy.TryGetComponent<Enemy>(out _))
                continue;

            float dist = (enemy.transform.position - transform.position).sqrMagnitude;
            if (dist < closestDistance)
            {
                closestDistance = dist;
                found = enemy.transform;
            }
        }

        return found;
    }
    protected void LookAtTargetOrInput()
    {
        Vector2 dir;

        if (_lockedEnemy == null)
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
        _targetLoseTime = 0f;

        transform.localPosition = OriginalLocalPosition;
        transform.rotation = Quaternion.identity;
        _spriteRenderer.flipY = false;
    }
}
