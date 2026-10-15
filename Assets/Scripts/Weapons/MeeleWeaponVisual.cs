using UnityEngine;
using NTC.Pool;
using UnityEngine.Serialization;

public class MeeleWeaponVisual : MonoBehaviour
{
    [FormerlySerializedAs("_meeleWeapon")]
    [SerializeField] private MeeleWeapon _meleeWeapon;
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private Transform _topPoint;

    private void OnEnable()
    {
        if (_meleeWeapon != null)
            _meleeWeapon.OnHit += PlayHitEffect;
    }
    private void OnDisable()
    {
        if (_meleeWeapon != null)
            _meleeWeapon.OnHit -= PlayHitEffect;
    }
    private void PlayHitEffect()
    {
        if (_hitEffect != null)
        {
            NightPool.Spawn(_hitEffect, _topPoint.position, Quaternion.identity);
        }
    }
}
