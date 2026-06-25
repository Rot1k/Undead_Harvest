using NTC.Pool;
using System;
using System.Collections;
using UnityEngine;

public enum AttackType
{
    AttackAround,
    BurstAttack
}

public class BossEnemy : Enemy
{
    public static event Action<BossEnemy> OnBossSpawned;

    [SerializeField] private Bullet _bulletPrefab;
    private BossEnemyStatsSO _bossEnemyStats;

    private bool _isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        _bossEnemyStats = _enemyStats as BossEnemyStatsSO;
        if (_bossEnemyStats == null)
            Debug.LogError("_enemyStats is not a RangerEnemyStatsSO!");
    }

    private void Start()
    {
        StartCoroutine(AttackLoop());
    }
    public override void OnSpawn()
    {
        base.OnSpawn();
        OnBossSpawned?.Invoke(this);
    }
    protected override void OnDead(object sender, EventArgs e)
    {
        base.OnDead(sender, e);
        StopAllCoroutines();
    }

    private IEnumerator AttackLoop()
    {
        while (!IsDead)
        {
            _isAttacking = true;
            yield return new WaitForSeconds(1f);

            switch (GetRandomAttack())
            {
                case AttackType.AttackAround:
                    yield return StartCoroutine(AttackAroundCoroutine());
                    break;
                case AttackType.BurstAttack:
                    yield return StartCoroutine(BurstAttackCoroutine());
                    break;
            }

            _isAttacking = false;
            yield return new WaitForSeconds(_bossEnemyStats.FireRate);
        }
    }

    protected override void FixedUpdate()
    {
        if (IsDead) return;


        if (!_isAttacking)
        {
            MoveToPlayer();
        }
        else
        {
            Rigidbody.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator AttackAroundCoroutine()
    {
        int bulletCount = 12;
        float angleStep = 360f / bulletCount;
        float angleOffset = 15f;

        for (int attacks = 0; attacks < 3; attacks++)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                float angle = (i * angleStep) + (angleOffset * attacks);
                Quaternion rotation = Quaternion.Euler(0, 0, angle);
                Bullet bullet = NightPool.Spawn(_bulletPrefab, transform.position, rotation);
                bullet.Setup(rotation * Vector2.up, _bossEnemyStats.BaseDamage, _bossEnemyStats.BulletSpeed);
            }
            yield return new WaitForSeconds(0.6f);
        }
    }

    private IEnumerator BurstAttackCoroutine()
    {
        float halfCone = 15f;
        for (int shots = 0; shots < 3; shots++)
        {
            Vector2 origin = transform.position;
            Vector2 targetPos = Player.position;
            Vector2 baseDir = (targetPos - origin).normalized;

            for (int burst = 0; burst < 3; burst++)
            {
                for (int i = -1; i <= 1; i++)
                {
                    Quaternion rotation = Quaternion.Euler(0f, 0f, i * halfCone);
                    Vector2 dir = rotation * baseDir;

                    Bullet bullet = NightPool.Spawn(_bulletPrefab, transform.position, rotation);
                    bullet.Setup(dir, _bossEnemyStats.BaseDamage, _bossEnemyStats.BulletSpeed);
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield return new WaitForSeconds(0.6f);
        }
    }

    private AttackType GetRandomAttack()
    {
        return UnityEngine.Random.value < 0.5f ? AttackType.AttackAround : AttackType.BurstAttack;
    }
}
