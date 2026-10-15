using UnityEngine;

[CreateAssetMenu(fileName = "MeeleWeaponSO", menuName = "Scriptable Objects/MeeleWeaponSO")]
public class MeeleWeaponSO : WeaponSO
{
    [Header("Melee Weapon Specifics")]
    [Min(0.01f)]
    public float WeaponSpeed;

}
