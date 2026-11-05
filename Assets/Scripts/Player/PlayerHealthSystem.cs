using System;
using R3;
using UnityEngine;

public class PlayerHealthSystem : IDamageable
{
    public HealthSystem HealthSystem { get; private set; }

    private PlayerStats _playerStats;
    private int MaxHealth => Mathf.RoundToInt(_playerStats.Get(StatType.MaxHealth));
    private float _healthRegen;
    private float _regenAccumulator = 0f;
    private bool _isInitialized;
    private readonly IDisposable _maxHealthSubscription;
    private readonly IDisposable _healthRegenSubscription;

    public PlayerHealthSystem(PlayerStats playerStats)
    {
        _playerStats = playerStats;
        HealthSystem = new HealthSystem(MaxHealth);
        _healthRegen = _playerStats.Get(StatType.HealthRegen);
        _isInitialized = true;

        _maxHealthSubscription = _playerStats.Observe(StatType.MaxHealth)
            .Subscribe(value => HealthSystem.SetMaxHealth(Mathf.RoundToInt(value), true));
        _healthRegenSubscription = _playerStats.Observe(StatType.HealthRegen)
            .Subscribe(value => _healthRegen = value);
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
        _maxHealthSubscription?.Dispose();
        _healthRegenSubscription?.Dispose();
    }
}
