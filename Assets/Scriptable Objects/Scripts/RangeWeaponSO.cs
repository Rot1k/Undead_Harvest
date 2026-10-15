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
    public PlayerBullet BulletPrefab;
    [Min(0.01f)]
    public float BulletSpeed;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (BulletPrefab == null)
            Debug.LogError($"{name} requires a BulletPrefab.", this);
    }
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
