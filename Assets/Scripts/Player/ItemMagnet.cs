using UnityEngine;
using VContainer;

public class ItemMagnet : MonoBehaviour
{
    private Player _player;
    private PlayerStats _playerStats => _player.Stats;

    [SerializeField] private CircleCollider2D _coinPickupCollider;
    [SerializeField] private CircleCollider2D _itemPickupCollider;

    [Inject]
    public void Construct(Player player)
    {
        _player = player;
    }

    private float _currentPickupRange => _playerStats.Get(StatType.ItemPickupRange);
    private float _currentMoveSpeed => _playerStats.Get(StatType.MoveSpeed);

    private void Start()
    {
        _playerStats.OnStatChanged += OnStatChanged;
        OnStatChanged(StatType.ItemPickupRange, _currentPickupRange);
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _playerStats.OnStatChanged -= OnStatChanged;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ICollectible>(out var collectible))
        {
            collectible.StartAttraction(transform, _currentMoveSpeed * 1.3f);
        }
    }
    private void OnStatChanged(StatType type, float value)
    {
        if (type == StatType.ItemPickupRange)
        _coinPickupCollider.radius = _currentPickupRange;
        _itemPickupCollider.radius = _currentPickupRange * 1.1f;
    }
}
