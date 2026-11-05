using System;
using R3;
using UnityEngine;
using VContainer;

public class ItemMagnet : MonoBehaviour
{
    private Player _player;
    private PlayerStats _playerStats => _player.Stats;

    [SerializeField] private CircleCollider2D _coinPickupCollider;
    [SerializeField] private CircleCollider2D _itemPickupCollider;
    private IDisposable _pickupRangeSubscription;

    [Inject]
    public void Construct(Player player)
    {
        _player = player;
    }

    private float _currentMoveSpeed => _playerStats.Get(StatType.MoveSpeed);

    private void Start()
    {
        _pickupRangeSubscription = _playerStats.Observe(StatType.ItemPickupRange)
            .Subscribe(value => UpdatePickupRange(value));
    }

    private void OnDestroy()
    {
        _pickupRangeSubscription?.Dispose();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ICollectible>(out var collectible))
        {
            collectible.StartAttraction(transform, _currentMoveSpeed * 1.3f);
        }
    }
    private void UpdatePickupRange(float value)
    {
        _coinPickupCollider.radius = value;
        _itemPickupCollider.radius = value * 1.1f;
    }
}
