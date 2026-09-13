using UnityEngine;
using VContainer;

public class ItemCollector : MonoBehaviour
{
    private Player _player;
    public LevelSystem LevelSystem => _player.Level.LevelSystem;
    public HealthSystem HealthSystem => _player.HealthSystem;

    [Inject]
    public void Construct(Player player)
    {
        _player = player;
    }

    public void AddExp(float amount)
    {
        _player.Level.AddExp(amount);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ICollectible>(out var collectible))
        {
            collectible.Collect(this);
        }
    }
}
