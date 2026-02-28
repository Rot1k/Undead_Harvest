using UnityEngine;
using NTC.Pool;

public class MeeleWeaponVisual : MonoBehaviour
{
    [SerializeField] private MeeleWeapon _meeleWeapon;
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private Transform _topPoint;

    private void OnEnable()
    {
        _meeleWeapon.OnHit += PlayHitEffect;
    }
    private void OnDisable()
    {
        _meeleWeapon.OnHit -= PlayHitEffect;
    }
    private void PlayHitEffect()
    {
        if (_hitEffect != null)
        {
            NightPool.Spawn(_hitEffect, _topPoint.position, Quaternion.identity);
        }
    }
}
