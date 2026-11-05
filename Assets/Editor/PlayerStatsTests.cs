using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using R3;
using UnityEngine;

public class PlayerStatsTests
{
    private PlayerStats _playerStats;
    private PlayerStatsSO _statsSO;

    [SetUp]
    public void SetUp()
    {
        _statsSO = ScriptableObject.CreateInstance<PlayerStatsSO>();
        typeof(PlayerStatsSO)
            .GetField("_stats", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(_statsSO, new List<PlayerStatsSO.StatEntry>
            {
                new() { Type = StatType.MaxHealth, BaseValue = 100 },
                new() { Type = StatType.AttackDamage, BaseValue = 10 }
            });

        _playerStats = new PlayerStats(_statsSO, null);
    }

    [TearDown]
    public void TearDown()
    {
        _playerStats.Dispose();
        Object.DestroyImmediate(_statsSO);
    }

    [Test]
    public void SetBaseStat_WithNegativeMaxHealth_ClampsToZero()
    {
        _playerStats.SetBaseStat(StatType.MaxHealth, -10);

        Assert.That(_playerStats.Get(StatType.MaxHealth), Is.EqualTo(0));
    }

    [Test]
    public void SetBaseStat_WithNegativeAttackDamage_KeepsNegativeValue()
    {
        _playerStats.SetBaseStat(StatType.AttackDamage, -10);

        Assert.That(_playerStats.Get(StatType.AttackDamage), Is.EqualTo(-10));
    }

    [Test]
    public void Observe_AfterStatChanged_ImmediatelyEmitsCurrentValue()
    {
        _playerStats.SetBaseStat(StatType.MaxHealth, 150);
        float observedValue = 0;

        using var subscription = _playerStats.Observe(StatType.MaxHealth)
            .Subscribe(value => observedValue = value);

        Assert.That(observedValue, Is.EqualTo(150));
    }
}
