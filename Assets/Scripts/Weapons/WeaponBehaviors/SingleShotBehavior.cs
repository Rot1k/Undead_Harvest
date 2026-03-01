using NTC.Pool;
using UnityEngine;

public class SingleShotBehavior : IWeaponBehavior
{
    public void Shoot(Weapon weapon, Transform firePoint, RangeWeaponContext context, Vector2 shootDirection)
    {
        PlayerBullet bullet = NightPool.Spawn(context.BulletPrefab, firePoint.position, Quaternion.identity);
        bullet.Setup(shootDirection, context.Damage, context.BulletSpeed, weapon);
    }
}
