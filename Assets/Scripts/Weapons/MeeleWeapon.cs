using System;
using UnityEngine;

public class MeeleWeapon : Weapon
{
    public enum State { Idle, Attacking, Returning }

    public event Action OnHit;

    private State _state;
    private MeeleWeaponSO _meeleWeaponSO;
    [SerializeField] private StatusEffectSO _statusEffectSO;
    private void Start()
    {

        _meeleWeaponSO = _weaponBaseStats as MeeleWeaponSO;
        if (_meeleWeaponSO == null)
            Debug.LogError($"[{name}] Invalid MeeleWeaponSO");

        _state = State.Idle;
        OriginalLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        // Використовуємо базовий Targeting та Aim
        base.UpdateTarget();              // тепер доступно
        base.LookAtTargetOrInput();       // візуальний поворот

        switch (_state)
        {
            case State.Idle:
                if (HasTarget && Time.time >= _nextFireTime)
                {
                    _state = State.Attacking;
                }
                break;

            case State.Attacking:
                Attack();
                break;

            case State.Returning:
                Return();
                break;
        }
    }

    private void Attack()
    {
        if (!HasTarget)
        {
            _state = State.Returning;
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            _lockedEnemy.position,
            _meeleWeaponSO.WeaponSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, _lockedEnemy.position) < 0.1f)
        {
            if (_lockedEnemy.TryGetComponent<Enemy>(out Enemy enemy))
            {
                Damage(enemy);
            }

            _state = State.Returning;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_state != State.Attacking) return;

        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            Damage(enemy);
            SoundManager.PlaySound(SoundType.HIT);
            OnHit?.Invoke();
            _state = State.Returning;
        }
    }

    private void Return()
    {
        transform.localPosition = Vector2.MoveTowards(
            transform.localPosition,
            OriginalLocalPosition,
            _meeleWeaponSO.WeaponSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.localPosition, OriginalLocalPosition) < 0.1f)
        {
            _state = State.Idle;
            _lockedEnemy = null;

            base.ApplyCooldown(Time.time); // використання базового DoCooldown
        }
    }

    private void Damage(Enemy enemy)
    {
        int damage = Mathf.RoundToInt(
            CombatSystem.CalculateDamage(_playerStats, _weaponBaseStats)
        );
        Debug.Log($"Hit {enemy.name} for {damage} damage.");
        enemy.GetComponent<StatusEffectsManager>()?.ApplyEffect(_statusEffectSO);
        enemy.HealthSystem.Damage(damage);
    }
}
