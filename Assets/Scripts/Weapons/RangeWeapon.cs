using System;
using UnityEngine;

public class RangeWeapon : Weapon
{
    public event Action OnShoot;

    [SerializeField] private Transform _muzzleTopPoint;

    private IWeaponBehavior _weaponBehavior;
    private RangeWeaponSO _rangeWeaponSO;

    private void Start()
    {

        _rangeWeaponSO = _weaponBaseStats as RangeWeaponSO;
        if (_rangeWeaponSO == null)
            Debug.LogError($"[{name}] Invalid RangeWeaponSO");

        _weaponBehavior = _rangeWeaponSO.WeaponType switch
        {
            WeaponType.Single => new SingleShotBehavior(),
            WeaponType.Shotgun => new ShotgunBehavior(),
            WeaponType.Burst => new BurstShotBehavior(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }


    private void Update()
    {
        UpdateTarget();
        LookAtTargetOrInput();

        if (!CanShoot(Time.time))
            return;

        Shoot();
    }

    private void Shoot()
    {
        Vector2 shootDirection = GetAimDirection(_muzzleTopPoint.position);
        if (shootDirection == Vector2.zero)
            return;

        int damage = Mathf.RoundToInt(
            CombatSystem.CalculateDamage(_playerStats, _weaponBaseStats)
        );

        _weaponBehavior.Shoot(
            this,
            _muzzleTopPoint,
            _rangeWeaponSO.GetContext(),
            shootDirection
        );

        OnShoot?.Invoke();
        ApplyCooldown(Time.time);
    }

    public override void UpdateStartPosition(Vector3 localPosition)
    {
        base.UpdateStartPosition(localPosition);

        if (TryGetComponent<RangeWeaponAnimation>(out var recoil))
        {
            recoil.InitializePosition(localPosition);
        }
    }
}
