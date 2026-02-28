using UnityEngine;
using NTC.Pool;
using System.Collections;

public class BurstShotBehavior : IWeaponBehavior
{
    public void Shoot(Weapon weapon,Transform firePoint, RangeWeaponContext context, Vector2 shootDirection)
    {
        weapon.StartCoroutine(BurstRoutine(firePoint, context, shootDirection));
    }
    private IEnumerator BurstRoutine(Transform firePoint, RangeWeaponContext context, Vector2 shootDirection)
    {
        int burstCount = 3;
        for (int i = 0; i < burstCount; i++)
        {
            var bulletGo = NightPool.Spawn(context.BulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bullet = bulletGo.GetComponent<Bullet>();
            bullet.Setup(shootDirection, context.Damage, context.BulletSpeed);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
