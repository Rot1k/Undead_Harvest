using NTC.Pool;
using UnityEngine;

public class RangerEnemy : Enemy
{
    
    [SerializeField] private Bullet _bulletPrefab;

    private RangerEnemyStatsSO _rangerEnemyStats;

    private float _nextFireTime = 0f;

    private Vector2 _moveDirection;
    private bool _wantToMove;

    protected override void Awake()
    {
        base.Awake();
        _rangerEnemyStats = _enemyStats as RangerEnemyStatsSO;
        if (_rangerEnemyStats == null)
            Debug.LogError("_enemyStats is not a RangerEnemyStatsSO!");
    }

    
    protected override void FixedUpdate()
    {
        if (IsDead) return;

        if (_wantToMove)
        {
            Rigidbody.linearVelocity = _moveDirection * _rangerEnemyStats.BaseMoveSpeed;
        }
        else
        {
            Rigidbody.linearVelocity = Vector2.zero;
        }
    }
    private void Update()
    {
        if (IsDead) return;

        float distance = Vector2.Distance(transform.position, Player.position);

        if (distance > _rangerEnemyStats.ShootingRange)
        {
            _wantToMove = true;
            _moveDirection = (Player.position - transform.position).normalized;
        }
        else if (distance < _rangerEnemyStats.RunAwayDistance)
        {
            TryShoot();
            _wantToMove = true;
            _moveDirection = -(Player.position - transform.position).normalized;
        }
        else
        {
            Rigidbody.linearVelocity = Vector2.zero;
            TryShoot();
            _wantToMove = false;
        }
    }



    private void TryShoot()
    {
        if (Time.time < _nextFireTime)
            return;

        Shoot();
        _nextFireTime = Time.time + _rangerEnemyStats.FireRate;
    }
    private void Shoot()
    {

        Bullet bullet = NightPool.Spawn(_bulletPrefab, transform.position, Quaternion.identity);
        Vector2 shootDirecton = (Player.position - transform.position).normalized;
        bullet.Setup(shootDirecton, _rangerEnemyStats.BaseDamage, _rangerEnemyStats.BulletSpeed);
    }
}
