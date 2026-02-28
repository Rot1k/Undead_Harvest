using NTC.Pool;
using UnityEngine;

public class ShotgunBehavior : IWeaponBehavior
{
    public void Shoot(Weapon weapon, Transform firePoint, RangeWeaponContext context, Vector2 shootDirection)
    {
        int pellets = 3;
        float spreadAngle = 30f;

        for (int i = 0; i < pellets; i++)
        {
            float t = (float)i / (pellets - 1);
            float angleOffset = Mathf.Lerp(-spreadAngle / 2, spreadAngle / 2, t);

            Quaternion rotation =
                Quaternion.AngleAxis(angleOffset, Vector3.forward) *
                Quaternion.FromToRotation(Vector2.right, shootDirection);

            Vector2 pelletDirection = rotation * Vector2.right;

            Bullet bullet = NightPool.Spawn(context.BulletPrefab, firePoint.position,rotation);
            bullet.Setup(pelletDirection, context.Damage, context.BulletSpeed);
        }
    }
}
