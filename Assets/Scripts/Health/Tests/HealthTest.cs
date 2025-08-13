using System;
using System.Collections;
using System.Collections.Generic;
using FoodWars.Health;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HealthTest
{
    private GameObject _testObject;

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
        // Remove all components to ensure clean state for each test.
        foreach (var comp in _testObject.GetComponents<Component>())
        {
            if (!(comp is Transform))
            {
                UnityEngine.Object.DestroyImmediate(comp);
            }
        }
    }

    private Health AddHealthManager(float _maxHitPoints = 100f, float _hitPoints = 100f)
    {
        var hm = _testObject.AddComponent<Health>();
        // typeof(Health).GetField("_maxHitPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        //     .SetValue(hm, _maxHitPoints);
        // typeof(Health).GetField("_hitPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
        //     .SetValue(hm, _hitPoints);

        // Simulate Unity's Awake call
        var awake = typeof(Health).GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        awake.Invoke(hm, null);

        return hm;
    }

    [Test]
    public void Awake_DoesNotChangeIfAlreadyAtMax()
    {
        var hm = AddHealthManager();
        Assert.AreEqual(100, hm.HitPoints);
    }

    [Test]
    public void DealDamage_ThrowsForNegativeInput()
    {
        var hm = AddHealthManager();
        Assert.Throws<ArgumentOutOfRangeException>(() => hm.DealDamage(-1f));
    }

    [Test]
    public void DealDamage_InvokesOnDamageForNonFatal()
    {
        var hm = AddHealthManager(_maxHitPoints: 100f, _hitPoints: 100f);
        bool damageCalled = false;
        hm.OnDamage += () => damageCalled = true;

        hm.DealDamage(10f);

        Assert.True(damageCalled);
        Assert.AreEqual(90f, hm.HitPoints);
    }

    [Test]
    public void DealDamage_InvokesOnDeathWhenHitPointsZeroOrLess()
    {
        var hm = AddHealthManager();
        bool deathCalled = false;
        hm.OnDeath += () => deathCalled = true;

        hm.DealDamage(100f);

        Assert.True(deathCalled);
        Assert.AreEqual(0f, hm.HitPoints);
    }

    [Test]
    public void DealDamage_InvokesOnDeathWhenHitPointsGoesNegative()
    {
        var hm = AddHealthManager();
        bool deathCalled = false;
        hm.OnDeath += () => deathCalled = true;

        hm.DealDamage(101f);

        Assert.True(deathCalled);
        Assert.AreEqual(-1f, hm.HitPoints);
    }

    [Test]
    public void Heal_ThrowsForNegativeInput()
    {
        var hm = AddHealthManager();
        Assert.Throws<ArgumentOutOfRangeException>(() => hm.Heal(-1f));
    }

    [Test]
    public void Heal_InvokesOnHeal()
    {
        var hm = AddHealthManager();
        bool healCalled = false;
        hm.OnHeal += () => healCalled = true;
        hm.DealDamage(10f);
        hm.Heal(10f);

        Assert.True(healCalled);
        Assert.AreEqual(100f, hm.HitPoints);
    }

    [Test]
    public void Heal_DoesNotExceedMaxHitPoints()
    {
        var hm = AddHealthManager();

        hm.Heal(10f);

        Assert.AreEqual(100f, hm.HitPoints);
    }

    [Test]
    public void Heal_AtMaxHitPoints_StaysAtMax()
    {
        var hm = AddHealthManager();

        hm.Heal(1f);

        Assert.AreEqual(100f, hm.HitPoints);
    }

    [Test]
    public void DealDamage_MultipleTimes_TriggersDeathOnce()
    {
        var hm = AddHealthManager();
        int deathCount = 0;
        hm.OnDeath += () => deathCount++;

        hm.DealDamage(90f); // Should not die
        hm.DealDamage(10f); // Should die
        hm.DealDamage(10f); // Should die again if called

        Assert.AreEqual(1, deathCount); // Because code allows OnDeath to be called on every call if <= 0
        Assert.LessOrEqual(hm.HitPoints, 0f);
    }
}
