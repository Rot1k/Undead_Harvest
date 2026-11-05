using UnityEngine;

public interface IWeaponBehavior
{
    void Shoot(Weapon weapon,Transform firePoint, RangeWeaponContext context, Vector2 shootDirection);
}
