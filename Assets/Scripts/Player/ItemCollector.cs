using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] private PlayerLevelSystem _playerLevelSystem;
    [SerializeField] private PlayerHealthSystem _playerHealthSystem;
    public LevelSystem LevelSystem => _playerLevelSystem.LevelSystem;
    public HealthSystem HealthSystem => _playerHealthSystem.HealthSystem;

    public void AddExp(float amount)
    {
        _playerLevelSystem.AddExp(amount);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ICollectible>(out var collectible))
        {
            collectible.Collect(this);
        }
    }
}
