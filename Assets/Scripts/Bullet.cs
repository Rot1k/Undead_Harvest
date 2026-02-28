using NTC.Pool;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour, ISpawnable
{
    private float _damage;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    public void OnSpawn()
    {
        _rigidbody.linearVelocity  = Vector2.zero;
        transform.rotation = Quaternion.identity;
    }

    public void Setup(Vector2 shootDir, float damage, float speed)
    {
        _damage = damage;
        _rigidbody.AddForce(shootDir * speed, ForceMode2D.Impulse);

        float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        transform.Rotate(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            NightPool.Despawn(gameObject);
        }
        else if (collision.gameObject.TryGetComponent<IDamageable>(out var target))
        {
            target.HealthSystem.Damage(Mathf.RoundToInt(_damage));
            NightPool.Despawn(gameObject);
        }
    }
}
