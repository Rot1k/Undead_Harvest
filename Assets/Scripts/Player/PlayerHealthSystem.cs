using UnityEngine;

public class PlayerHealthSystem : IDamageable
{
    public HealthSystem HealthSystem { get; private set; }

    private PlayerStats _playerStats;
    private int MaxHealth => Mathf.RoundToInt(_playerStats.Get(StatType.MaxHealth));
    private float _healthRegen;
    private float _regenAccumulator = 0f;
    private bool _isInitialized;

    public PlayerHealthSystem(PlayerStats playerStats)
    {
        _playerStats = playerStats;
        _playerStats.OnStatChanged += OnStatChanged;
        HealthSystem = new HealthSystem(MaxHealth);
        _healthRegen = _playerStats.Get(StatType.HealthRegen);
        _isInitialized = true;
    }

    public void Tick(float deltaTime)
    {
        if (!_isInitialized)
            return;

        _regenAccumulator += _healthRegen * deltaTime;

        if (_regenAccumulator >= 1f)
        {
            int healAmount = Mathf.FloorToInt(_regenAccumulator);
            HealthSystem.Heal(healAmount);
            _regenAccumulator -= healAmount;
        }
        else if (_regenAccumulator < 0)
        {
            _regenAccumulator = 0;
        }
    }

    public void Dispose()
    {
        if (_playerStats != null)
            _playerStats.OnStatChanged -= OnStatChanged;
    }
    private void OnStatChanged(StatType type, float newValue)
    {
        if (type == StatType.MaxHealth)
        {
            HealthSystem.SetMaxHealth(MaxHealth, true);
        }
        if (type == StatType.HealthRegen)
        {
            _healthRegen = newValue;
        }
    }
}
