using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerHealthSystem : MonoBehaviour, IDamageable
{
    public HealthSystem HealthSystem { get; private set; }

    private PlayerStats _playerStats;
    private int MaxHealth => Mathf.RoundToInt(_playerStats.Get(StatType.MaxHealth));
    private float _healthRegen;
    private float _regenAccumulator = 0f;

    private void OnEnable()
    {
        if (_playerStats != null)
        {
            _playerStats.OnStatChanged += OnStatChanged;
        }
    }
    private void Awake()
    {
        _playerStats = GetComponent<PlayerStats>();
        HealthSystem = new HealthSystem(MaxHealth);
        _healthRegen = _playerStats.Get(StatType.HealthRegen);
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

    private void Start()
    {
        StartCoroutine(RegenerateHealth());
    }

    private IEnumerator RegenerateHealth()
    {
        while (true)
        {
            float regenInterval = 0.5f;
            yield return new WaitForSeconds(regenInterval);

            _regenAccumulator += _healthRegen * regenInterval;

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
    }
}
