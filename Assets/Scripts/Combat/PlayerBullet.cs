using UnityEngine;

public class PlayerBullet : Bullet
{
    private Weapon _weapon;

    public void Setup(Vector2 shootDir, float damage, float speed, Weapon weapon)
    {
        base.Setup(shootDir, damage, speed);
        _weapon = weapon;
    }

    protected override bool TryGetTarget(Collider2D collision, out IDamageable target)
    {
        if (collision.TryGetComponent<Enemy>(out var enemy))
        {
            target = enemy;
            return true;
        }

        target = null;
        return false;
    }

    protected override void OnHit(IDamageable target)
    {
        if (target is Enemy enemy)
        {
            _weapon.HandleOnHit(enemy);
        }
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        _weapon = null;
    }
}