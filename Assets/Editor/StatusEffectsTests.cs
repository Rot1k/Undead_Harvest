using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class StatusEffectsTests
{
    private GameObject _enemyObject;
    private StatusEffectsManager _statusEffectsManager;
    private EnemyStatsSO _enemyStats;

    [SetUp]
    public void SetUp()
    {
        _enemyStats = ScriptableObject.CreateInstance<EnemyStatsSO>();

        _enemyObject = new GameObject("Test Enemy");
        _enemyObject.SetActive(false);
        _enemyObject.AddComponent<CircleCollider2D>();

        var enemy = _enemyObject.AddComponent<Enemy>();
        var enemyStatsField = typeof(Enemy).GetField(
            "_enemyStats",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        Assert.That(enemyStatsField, Is.Not.Null);
        enemyStatsField.SetValue(enemy, _enemyStats);

        _enemyObject.SetActive(true);
        _statusEffectsManager = _enemyObject.GetComponent<StatusEffectsManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_enemyObject);
        Object.DestroyImmediate(_enemyStats);
    }

    [Test]
    public void ApplyEffect_WithTwoDifferentEffects_HasCorrectEffectCount()
    {
        var firstEffect = CreateEffect(false);
        var secondEffect = CreateEffect(false);

        _statusEffectsManager.ApplyEffect(firstEffect);
        _statusEffectsManager.ApplyEffect(secondEffect);

        Assert.That(_statusEffectsManager.ActiveEffectCount, Is.EqualTo(2));

        Object.DestroyImmediate(firstEffect);
        Object.DestroyImmediate(secondEffect);
    }

    [Test]
    public void ApplyEffect_TwiceForNonStackableEffect_DoesNotAddAnotherEffect()
    {
        var effect = CreateEffect(false);

        _statusEffectsManager.ApplyEffect(effect);
        _statusEffectsManager.ApplyEffect(effect);

        Assert.That(_statusEffectsManager.ActiveEffectCount, Is.EqualTo(1));

        Object.DestroyImmediate(effect);
    }

    [Test]
    public void ClearAllEffects_RemovesAllEffects()
    {
        var firstEffect = CreateEffect(false);
        var secondEffect = CreateEffect(false);

        _statusEffectsManager.ApplyEffect(firstEffect);
        _statusEffectsManager.ApplyEffect(secondEffect);
        _statusEffectsManager.ClearAllEffects();

        Assert.That(_statusEffectsManager.ActiveEffectCount, Is.EqualTo(0));

        Object.DestroyImmediate(firstEffect);
        Object.DestroyImmediate(secondEffect);
    }

    private StatusEffectSO CreateEffect(bool isStackable)
    {
        var effect = ScriptableObject.CreateInstance<StatusEffectSO>();
        effect.Duration = 1;
        effect.TickInterval = 1;
        effect.IsStackable = isStackable;
        effect.MaxStacks = 1;
        effect.Modifiers = new();
        return effect;
    }
}
