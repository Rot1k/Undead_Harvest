using UnityEngine;
public enum WeaponType
{
    Single,
    Shotgun,
    Burst,
    Laser,
}

[CreateAssetMenu(fileName = "RangeWeaponSO", menuName = "Scriptable Objects/RangeWeaponSO")]
public class RangeWeaponSO : WeaponSO
{
    [Header("Range Weapon Specifics")]
    public WeaponType WeaponType;
    public Bullet BulletPrefab;
    public float BulletSpeed;
    public RangeWeaponContext GetContext()
    {
        return new RangeWeaponContext
        {
            Damage = BaseDamage,
            FireRate = BaseAttackSpeed,
            BulletSpeed = BulletSpeed,
            BulletPrefab = BulletPrefab
        };
    }
}
