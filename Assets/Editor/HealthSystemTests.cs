using NUnit.Framework;

public class HealthSystemTests
{
    [Test]
    public void Damage_ReducesHealthByDamageAmount()
    {
        var healthSystem = new HealthSystem(100);

        healthSystem.Damage(30);

        Assert.That(healthSystem.Health.Value, Is.EqualTo(70));
    }

    [Test]
    public void Heal_DoesNotExceedMaximumHealth()
    {
        var healthSystem = new HealthSystem(100);
        healthSystem.Damage(20);

        healthSystem.Heal(50);

        Assert.That(healthSystem.Health.Value, Is.EqualTo(100));
    }

    [Test]
    public void Damage_AtZeroHealth_RaisesDeathEventOnlyOnce()
    {
        var healthSystem = new HealthSystem(100);
        int deathEvents = 0;
        healthSystem.OnDead += (_, _) => deathEvents++;

        healthSystem.Damage(100);
        healthSystem.Damage(1);

        Assert.That(deathEvents, Is.EqualTo(1));
    }
}
