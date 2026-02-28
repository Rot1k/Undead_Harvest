using System;
using UnityEngine;

public class DashEnemy : Enemy
{
    private const string ENEMY_LAYER_NAME = "Enemy";
    [SerializeField] private float _dashCooldown = 2f;
    [SerializeField] private float _dashSpeed = 10f;
    [SerializeField] private float _dashDuration = 0.2f;
    [SerializeField] private float _dashPrepareTime = 0.5f;
    [SerializeField] private float _dashRange = 5f;
    private float _timeSinceLastDash = 0f;
    private bool _isDashing = false;
    private bool _isPreparing = false;
    private int _enemyLayer;

    public event Action OnDashPrepare;

    protected override void Awake()
    {
        base.Awake();
        _enemyLayer = LayerMask.NameToLayer(ENEMY_LAYER_NAME);
    }

    private void Dash()
    {
        _isDashing = true;
        Vector2 dashDirection = (Player.position - transform.position).normalized;
        Rigidbody.AddForce(dashDirection * _dashSpeed, ForceMode2D.Impulse);
        IgnoreEnemyCollisions(true);
    }

    private void PrepareDash()
    {
        _isPreparing = true;
        Rigidbody.linearVelocity = Vector2.zero;
        _timeSinceLastDash = Time.time;
        OnDashPrepare?.Invoke();
    }

    private void IgnoreEnemyCollisions(bool ignore)
    {
        Physics2D.IgnoreLayerCollision(_enemyLayer, _enemyLayer, ignore);
    }

    protected override void FixedUpdate()
    {
        if (IsDead) return;

        if (_isPreparing)
        {
            Rigidbody.linearVelocity = Vector2.zero;
            if (Time.time >= _timeSinceLastDash + _dashPrepareTime)
            {
                _isPreparing = false;
                Dash();
            }
        }
        else if (_isDashing)
        {
            if (Time.time >= _timeSinceLastDash + _dashPrepareTime + _dashDuration)
            {
                _isDashing = false;
                Rigidbody.linearVelocity = Vector2.zero;
                IgnoreEnemyCollisions(false);
            }
        }
        else
        {
            float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
            if (distanceToPlayer <= _dashRange && Time.time >= _timeSinceLastDash + _dashCooldown + _dashPrepareTime + _dashDuration)
            {
                PrepareDash();
            }
            else
            {
                MoveToPlayer();
            }
        }
    }
}
