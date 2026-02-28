using NTC.Pool;
using UnityEngine;

public class SingleShotBehavior : IWeaponBehavior
{
    public void Shoot(Weapon weapon, Transform firePoint, RangeWeaponContext context, Vector2 shootDirection)
    {
        var bulletGo = NightPool.Spawn(context.BulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bullet = bulletGo.GetComponent<Bullet>();
        bullet.Setup(shootDirection, context.Damage, context.BulletSpeed);
    }
}
