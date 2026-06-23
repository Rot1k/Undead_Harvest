using NTC.Pool;
using UnityEngine;
using VContainer;

public class ExpCoin : MonoBehaviour, ICollectible, ISpawnable
{
    [SerializeField] private float _expAmount = 10;
    [SerializeField] private float _moneyAmount = 10f;

    private Transform _target;
    private float _speed = 10f;
    private float _speedMultiplier = 1.5f;
    private bool _isAttracted = false;

    private WalletManager _walletManager;

    [Inject]
    public void Construct(WalletManager walletManager)
    {
        _walletManager = walletManager;
    }

    public void StartAttraction(Transform target, float speed)
    {
        _speed = speed;
        _target = target;
        _isAttracted = true;
        enabled = true;
    }

    private void Awake()
    {
        enabled = false;
    }

    private void Update()
    {
        if (_isAttracted && _target != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _target.position,
                _speed * Time.deltaTime * _speedMultiplier
            );
        }
    }
    public void Collect(ItemCollector itemCollector)
    {
        itemCollector.AddExp(_expAmount);
        _walletManager.AddMoney(_moneyAmount);
        NightPool.Despawn(this);
    }

    public void OnSpawn()
    {
        _target = null;
        _isAttracted = false;
        enabled = false;
    }
}
