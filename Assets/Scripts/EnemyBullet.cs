using UnityEngine;

public class EnemyBullet : Bullet
{
    protected override bool TryGetTarget(Collider2D collision, out IDamageable target)
    {
        if (collision.TryGetComponent<PlayerHealthSystem>(out var player))
        {
            target = player;
            return true;
        }

        target = null;
        return false;
    }
}