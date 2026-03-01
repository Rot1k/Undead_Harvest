using UnityEngine;
using NTC.Pool;
using System.Collections;

public class BurstShotBehavior : IWeaponBehavior
{
    public void Shoot(Weapon weapon,Transform firePoint, RangeWeaponContext context, Vector2 shootDirection)
    {
        weapon.StartCoroutine(BurstRoutine(firePoint, context, shootDirection, weapon));
    }
    private IEnumerator BurstRoutine(Transform firePoint, RangeWeaponContext context, Vector2 shootDirection, Weapon weapon)
    {
        int burstCount = 3;
        for (int i = 0; i < burstCount; i++)
        {
            PlayerBullet bullet = NightPool.Spawn(context.BulletPrefab, firePoint.position, Quaternion.identity);
            bullet.Setup(shootDirection, context.Damage, context.BulletSpeed, weapon);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
