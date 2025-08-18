using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FoodWars.Health;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HealthTest
{
    private GameObject _testObject;
    private float _defaultMaxHitpoints = 100;

    [SetUp]
    public void SetUp()
    {
        if (_testObject == null)
        {
            _testObject = new GameObject("TestObject");
        }
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_testObject.GetComponent<Health>());

        foreach (var comp in _testObject.GetComponents<Component>())
        {
            if (!(comp is Transform))
            {
                UnityEngine.Object.DestroyImmediate(comp);
            }
        }

    }

    private Health SetupHealthComponent()
    {
        _testObject.AddComponent<Rigidbody>();
        _testObject.AddComponent<CapsuleCollider>();

        var healthComponent = _testObject.AddComponent<Health>();

        typeof(Health).GetField("_maxHitPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        .SetValue(healthComponent, _defaultMaxHitpoints);

        // Simulate Unity's Awake call
        var awake = typeof(Health).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        awake.Invoke(healthComponent, null);

        return healthComponent;
    }

    [Test]
    public void Awake_DoesNotChangeIfAlreadyAtMax()
    {
        var healthObject = SetupHealthComponent();
        Assert.AreEqual(healthObject.MaxHitPoints, healthObject.HitPoints);
    }

    [Test]
    public void DealDamage_ThrowsForNegativeInput()
    {
        var healthObject = SetupHealthComponent();
        Assert.Throws<ArgumentOutOfRangeException>(() => healthObject.DealDamage(-1f));
    }

    [Test]
    public void DealDamage_DoesNotChangeIfZero()
    {
        var healthObject = SetupHealthComponent();
        healthObject.DealDamage(0);
        Assert.AreEqual(healthObject.MaxHitPoints, healthObject.HitPoints);
    }

    [Test]
    public void DealDamage_InvokesOnDamageForNonFatal()
    {
        var healthObject = SetupHealthComponent();
        bool damageCalled = false;
        healthObject.OnDamage += () => damageCalled = true;

        float damage = 10f;
        healthObject.DealDamage(damage);

        Assert.True(damageCalled);
        Assert.AreEqual(_defaultMaxHitpoints - damage, healthObject.HitPoints);
    }

    [Test]
    public void DealDamage_InvokesOnDeathWhenHitPointsZeroOrLess()
    {
        var healthObject = SetupHealthComponent();
        bool deathCalled = false;
        healthObject.OnDeath += () => deathCalled = true;

        healthObject.DealDamage(_defaultMaxHitpoints);

        Assert.True(deathCalled);
        Assert.AreEqual(0, healthObject.HitPoints);
    }

    [Test]
    public void DealDamage_InvokesOnDeathWhenHitPointsGoesNegative()
    {
        var healthObject = SetupHealthComponent();
        bool deathCalled = false;
        healthObject.OnDeath += () => deathCalled = true;

        healthObject.DealDamage(_defaultMaxHitpoints + 1);

        Assert.True(deathCalled);
        Assert.AreEqual(-1f, healthObject.HitPoints);
    }

    [Test]
    public void Heal_ThrowsForNegativeInput()
    {
        var healthObject = SetupHealthComponent();
        Assert.Throws<ArgumentOutOfRangeException>(() => healthObject.Heal(-1f));
    }


    [Test]
    public void Heal_DoesNotChangeIfZero()
    {
        var healthObject = SetupHealthComponent();
        float damage = 20f;
        healthObject.DealDamage(damage);
        healthObject.Heal(0);
        Assert.AreEqual(_defaultMaxHitpoints - damage, healthObject.HitPoints);
    }

    [Test]
    public void Heal_InvokesOnHeal()
    {
        var healthObject = SetupHealthComponent();
        bool healCalled = false;
        healthObject.OnHeal += () => healCalled = true;
        healthObject.DealDamage(10f);
        healthObject.Heal(10f);

        Assert.True(healCalled);
        Assert.AreEqual(_defaultMaxHitpoints, healthObject.HitPoints);
    }

    [Test]
    public void Heal_DoesNotExceedMaxHitPoints()
    {
        var healthObject = SetupHealthComponent();

        healthObject.Heal(10f);

        Assert.AreEqual(_defaultMaxHitpoints, healthObject.HitPoints);
    }

    [Test]
    public void Heal_AtMaxHitPoints_StaysAtMax()
    {
        var healthObject = SetupHealthComponent();

        healthObject.Heal(1f);

        Assert.AreEqual(_defaultMaxHitpoints, healthObject.HitPoints);
    }

    [Test]
    public void DealDamage_MultipleTimes_TriggersDeathOnce()
    {
        var healthObject = SetupHealthComponent();
        int deathCount = 0;
        healthObject.OnDeath += () => deathCount++;

        healthObject.DealDamage(_defaultMaxHitpoints * 0.9f); // Alive
        healthObject.DealDamage(_defaultMaxHitpoints * 0.1f); // Dead, with event
        healthObject.DealDamage(_defaultMaxHitpoints * 0.1f); // Still dead, no event.

        Assert.AreEqual(1, deathCount);
        Assert.LessOrEqual(healthObject.HitPoints, 0f);
    }

    [Test]
    public async void Regeneration_WhenDamagedWithRegenerationEnabled_StartsAfterInitialDelay()
    {
        var healthObject = SetupHealthComponent();

        healthObject.EnableRegeneration();
        bool regenerationStarted = false;
        healthObject.OnRegenerationStarted += () => regenerationStarted = true;

        healthObject.DealDamage(50f);

        await UniTask.WaitForSeconds(4.5f);

        Assert.IsTrue(regenerationStarted);
        Assert.IsTrue(healthObject.IsRegenerating);
    }

    [Test]
    public async void Regeneration_WhenDamagedWithRegenerationDisabled_DoesNotStart()
    {
        var healthObject = SetupHealthComponent();

        healthObject.DisableRegeneration();
        bool regenerationStarted = false;
        healthObject.OnRegenerationStarted += () => regenerationStarted = true;

        healthObject.DealDamage(50f);

        await UniTask.WaitForSeconds(5f);

        // Assert
        Assert.IsFalse(regenerationStarted);
        Assert.IsFalse(healthObject.IsRegenerating);
    }

    [Test]
    public async void Regeneration_WhenAtFullHealth_DoesNotStart()
    {
        var healthObject = SetupHealthComponent();

        healthObject.EnableRegeneration();
        bool regenerationStarted = false;
        healthObject.OnRegenerationStarted += () => regenerationStarted = true;

        healthObject.DealDamage(0f);

        await UniTask.WaitForSeconds(5f);

        Assert.IsFalse(regenerationStarted);
        Assert.IsFalse(healthObject.IsRegenerating);
    }

    [Test]
    public async void Regeneration_HealsAtCorrectRate()
    {
        var healthObject = SetupHealthComponent();

        healthObject.EnableRegeneration();
        float initialHealth = healthObject.HitPoints;

        healthObject.DealDamage(30f);

        await UniTask.WaitForSeconds(5.5f);


        Assert.AreEqual(80f, healthObject.HitPoints, 0.1f);
        Assert.IsTrue(healthObject.IsRegenerating);
    }

    [Test]
    public async void Regeneration_ContinuesUntilFullHealth()
    {
        var healthObject = SetupHealthComponent();

        healthObject.EnableRegeneration();
        bool regenerationStopped = false;
        healthObject.OnRegenerationStopped += () => regenerationStopped = true;

        healthObject.DealDamage(25f);
        await UniTask.WaitForSeconds(16f);

        Assert.AreEqual(100f, healthObject.HitPoints);
        Assert.IsFalse(healthObject.IsRegenerating);
        Assert.IsTrue(regenerationStopped);
    }

    [Test]
    public async void Regeneration_MultipleHealTicks_InvokesOnHealEvent()
    {
        var healthObject = SetupHealthComponent();

        healthObject.EnableRegeneration();
        int healEventCount = 0;
        healthObject.OnHeal += () => healEventCount++;

        healthObject.DealDamage(25f);

        await UniTask.WaitForSeconds(15f);

        Assert.AreEqual(3, healEventCount);
    }

    [Test]
    public async void Regeneration_WhenDamagedDuringRegeneration_StopsAndRestarts()
    {
        var healthObject = SetupHealthComponent();

        healthObject.EnableRegeneration();
        int regenerationStartedCount = 0;
        int regenerationStoppedCount = 0;
        healthObject.OnRegenerationStarted += () => regenerationStartedCount++;
        healthObject.OnRegenerationStopped += () => regenerationStoppedCount++;

        healthObject.DealDamage(50f);

        await UniTask.WaitForSeconds(5f);

        healthObject.DealDamage(10f); // Second damage during regeneration

        await UniTask.WaitForSeconds(5f);

        Assert.AreEqual(2, regenerationStartedCount); // Started twice
        Assert.AreEqual(1, regenerationStoppedCount); // Stopped once (when interrupted)
        Assert.IsTrue(healthObject.IsRegenerating); // Should be regenerating again
    }

    [Test]
    public async void Regeneration_WhenHealedToFullExternally_Stops()
    {
        var healthObject = SetupHealthComponent();

        healthObject.EnableRegeneration();
        bool regenerationStopped = false;
        healthObject.OnRegenerationStopped += () => regenerationStopped = true;

        healthObject.DealDamage(50f);

        await UniTask.WaitForSeconds(5f);

        healthObject.Heal(50f);

        await UniTask.WaitForSeconds(1f);

        Assert.IsFalse(healthObject.IsRegenerating);
        Assert.IsTrue(regenerationStopped);
    }

    [Test]
    public async void Regeneration_WhenKilled_DoesNotStart()
    {
        var healthObject = SetupHealthComponent();

        healthObject.EnableRegeneration();
        bool regenerationStarted = false;
        healthObject.OnRegenerationStarted += () => regenerationStarted = true;

        healthObject.DealDamage(100f);

        await UniTask.WaitForSeconds(5f);

        Assert.IsTrue(healthObject.IsDead);
        Assert.IsFalse(regenerationStarted);
        Assert.IsFalse(healthObject.IsRegenerating);
    }

    [Test]
    public async void Regeneration_WhenKilledDuringRegeneration_Stops()
    {
        var healthObject = SetupHealthComponent();

        healthObject.EnableRegeneration();
        bool regenerationStopped = false;
        healthObject.OnRegenerationStopped += () => regenerationStopped = true;

        healthObject.DealDamage(50f);

        await UniTask.WaitForSeconds(5f);

        healthObject.DealDamage(60f);

        await UniTask.WaitForSeconds(0.1f);

        Assert.IsTrue(healthObject.IsDead);
        Assert.IsFalse(healthObject.IsRegenerating);
        Assert.IsTrue(regenerationStopped);
    }

    [Test]
    public void Regeneration_WhenGameObjectDisabled_StopsGracefully()
    {
        var healthObject = SetupHealthComponent();

        healthObject.EnableRegeneration();
        healthObject.DealDamage(50f);

        healthObject.gameObject.SetActive(false);

        Assert.IsFalse(healthObject.IsRegenerating);
    }

}
