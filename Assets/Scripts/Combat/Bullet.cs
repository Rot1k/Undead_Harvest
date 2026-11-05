using NTC.Pool;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Bullet : MonoBehaviour, ISpawnable
{
    protected float _damage;
    private Rigidbody2D _rigidbody;
    private bool _isDespawning;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public virtual void OnSpawn()
    {
        _isDespawning = false;
        _rigidbody.linearVelocity = Vector2.zero;
        transform.rotation = Quaternion.identity;
    }

    public virtual void Setup(Vector2 shootDir, float damage, float speed)
    {
        _damage = damage;
        _rigidbody.AddForce(shootDir * speed, ForceMode2D.Impulse);

        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        transform.Rotate(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsObstacle(collision))
        {
            Despawn();
            return;
        }

        if (TryGetTarget(collision, out var target))
        {
            target.HealthSystem.Damage(Mathf.RoundToInt(_damage));

            OnHit(target);

            Despawn();
        }
    }

    protected virtual bool IsObstacle(Collider2D collision)
    {
        return collision.gameObject.layer == LayerMask.NameToLayer("Obstacle");
    }

    protected abstract bool TryGetTarget(Collider2D collision, out IDamageable target);

    protected virtual void OnHit(IDamageable target) { }

    protected void Despawn()
    {
        if (_isDespawning)
            return;

        _isDespawning = true;
        NightPool.Despawn(gameObject);
    }
}
