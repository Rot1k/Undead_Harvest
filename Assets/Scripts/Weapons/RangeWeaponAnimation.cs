using UnityEngine;

public class RangeWeaponAnimation : MonoBehaviour
{
    [SerializeField] private RangeWeapon _rangeWeapon;
    [SerializeField] private float _recoilDistance = 0.2f;
    [SerializeField] private float _recoilSpeed = 10f;


    private Vector3 _targetPos;
    private Vector3 _originalLocalPos;


    private void OnEnable()
    {
        _rangeWeapon.OnShoot += HandleRecoil;
    }

    private void OnDisable()
    {
        _rangeWeapon.OnShoot -= HandleRecoil;
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPos, Time.deltaTime * _recoilSpeed);
    }

    private void HandleRecoil()
    {
        _targetPos = _originalLocalPos - transform.right * _recoilDistance;

        Invoke(nameof(ResetRecoil), 0.05f);
    }

    private void ResetRecoil()
    {
        _targetPos = _originalLocalPos;
    }
    public void InitializePosition(Vector3 startLocalPosition)
    {
        _originalLocalPos = startLocalPosition;
        _targetPos = startLocalPosition;
    }
}