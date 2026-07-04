using R3;
using System;
using UnityEngine;
public class HealthSystem
{
    public event EventHandler OnDead;

    private readonly ReactiveProperty<int> _healthMax;
    private readonly ReactiveProperty<int> _health;

    public ReadOnlyReactiveProperty<int> HealthMax => _healthMax;
    public ReadOnlyReactiveProperty<int> Health => _health;

    public HealthSystem(int healthMax)
    {
        _healthMax = new ReactiveProperty<int>(healthMax);
        _health = new ReactiveProperty<int>(healthMax);
    }
    public void SetMaxHealth(int newMax, bool keepCurrentPercent = true)
    {
        newMax = Math.Max(1, newMax);
        if (newMax == _healthMax.Value) return;

        if (keepCurrentPercent)
        {
            float percent = (float)_health.Value / _healthMax.Value;
            _healthMax.Value = newMax;
            _health.Value = Math.Clamp(Mathf.RoundToInt(percent * _healthMax.Value), 0, _healthMax.Value);
        }
        else
        {
            _healthMax.Value = newMax;
            _health.Value = Math.Clamp(_health.Value, 0, _healthMax.Value);
        }

    }


    public void Damage(int amount)
    {
        _health.Value -= amount;
        if (_health.Value < 0)
        {
            _health.Value = 0;
        }


        if (_health.Value <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }
    public void Heal(int amount)
    {
        _health.Value += amount;
        if (_health.Value > _healthMax.Value)
        {
            _health.Value = _healthMax.Value;
        }
    }
    public void Reset(int maxHealth)
    {
        _healthMax.Value = maxHealth;
        _health.Value = maxHealth;
    }
}