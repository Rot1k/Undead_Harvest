using System;
using UnityEngine;
public class HealthSystem
{

    public event EventHandler OnHealthChanged;
    public event EventHandler OnDamaged;
    public event EventHandler OnHealed;
    public event EventHandler OnDead;

    public int HealthMax { get; private set; }
    public int Health { get; private set; }

    public HealthSystem(int healthMax)
    {
        HealthMax = healthMax;
        Health = healthMax;
    }
    public void SetMaxHealth(int newMax, bool keepCurrentPercent = true)
    {
        newMax = Math.Max(1, newMax);
        if (newMax == HealthMax) return;

        if (keepCurrentPercent)
        {
            float percent = (float)Health / HealthMax;
            HealthMax = newMax;
            Health = Math.Clamp(Mathf.RoundToInt(percent * HealthMax), 0, HealthMax);
        }
        else
        {
            HealthMax = newMax;
            Health = Math.Clamp(Health, 0, HealthMax);
        }

        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }


    public void Damage(int amount)
    {
        Health -= amount;
        if (Health < 0)
        {
            Health = 0;
        }
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        OnDamaged?.Invoke(this, EventArgs.Empty);

        if (Health <= 0)
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
        Health += amount;
        if (Health > HealthMax)
        {
            Health = HealthMax;
        }
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        OnHealed?.Invoke(this, EventArgs.Empty);
    }
    public float GetHealthPercent()
    {
        return (float)Health / HealthMax;
    }
}