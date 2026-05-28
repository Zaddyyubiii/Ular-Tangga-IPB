using NUnit.Framework;
using UnityEngine;
using Dice;

namespace Editor.Tests
{
    public class DiceRollResolverTests
    {
        [Test]
        public void TestChargeZoneMapping()
        {
            // Test Zone 1: Charge <= 20%
            for (int i = 0; i < 50; i++)
            {
                float charge = Random.Range(0f, 20f);
                DiceResult result = DiceRollResolver.ResolveRoll(charge, 0);
                Assert.AreEqual("Zona 1", result.zoneName);
                Assert.IsTrue(result.value >= 1 && result.value <= 12, "Dice value out of bounds in Zone 1: " + result.value);
            }

            // Test Zone 2: 21% <= Charge <= 40%
            for (int i = 0; i < 50; i++)
            {
                float charge = Random.Range(21f, 40f);
                DiceResult result = DiceRollResolver.ResolveRoll(charge, 0);
                Assert.AreEqual("Zona 2", result.zoneName);
                Assert.IsTrue(result.value >= 1 && result.value <= 12, "Dice value out of bounds in Zone 2: " + result.value);
            }

            // Test Zone 3: 41% <= Charge <= 60%
            for (int i = 0; i < 50; i++)
            {
                float charge = Random.Range(41f, 60f);
                DiceResult result = DiceRollResolver.ResolveRoll(charge, 0);
                Assert.AreEqual("Zona 3", result.zoneName);
                Assert.IsTrue(result.value >= 1 && result.value <= 12, "Dice value out of bounds in Zone 3: " + result.value);
            }

            // Test Zone 4: 61% <= Charge <= 80%
            for (int i = 0; i < 50; i++)
            {
                float charge = Random.Range(61f, 80f);
                DiceResult result = DiceRollResolver.ResolveRoll(charge, 0);
                Assert.AreEqual("Zona 4", result.zoneName);
                Assert.IsTrue(result.value >= 1 && result.value <= 12, "Dice value out of bounds in Zone 4: " + result.value);
            }

            // Test Zone 5: 81% <= Charge <= 100%
            for (int i = 0; i < 50; i++)
            {
                float charge = Random.Range(81f, 100f);
                DiceResult result = DiceRollResolver.ResolveRoll(charge, 0);
                Assert.AreEqual("Zona 5", result.zoneName);
                Assert.IsTrue(result.value >= 1 && result.value <= 12, "Dice value out of bounds in Zone 5: " + result.value);
            }
        }

        [Test]
        public void TestTimingQuality()
        {
            // Targets: 25%, 50%, 75%, 95%
            
            // Perfect: diff <= 3%
            DiceResult p1 = DiceRollResolver.ResolveRoll(25f, 0);
            Assert.AreEqual("Perfect", p1.timingQuality);
            
            DiceResult p2 = DiceRollResolver.ResolveRoll(52.5f, 0); // diff = 2.5%
            Assert.AreEqual("Perfect", p2.timingQuality);

            // Good: 3% < diff <= 7%
            DiceResult g1 = DiceRollResolver.ResolveRoll(30f, 0); // diff from 25 is 5%
            Assert.AreEqual("Good", g1.timingQuality);
            
            DiceResult g2 = DiceRollResolver.ResolveRoll(70f, 0); // diff from 75 is 5%
            Assert.AreEqual("Good", g2.timingQuality);

            // Normal: diff > 7%
            DiceResult n1 = DiceRollResolver.ResolveRoll(10f, 0); // diff from 25 is 15%
            Assert.AreEqual("Normal", n1.timingQuality);
        }

        [Test]
        public void TestNearFinishStrategy()
        {
            // Player is close to finishing: tile >= 88. Let's say tile 96 (needed = 4)
            int currentTile = 96;
            int needed = 100 - currentTile; // 4
            
            int smallRollCount = 0;
            for (int i = 0; i < 200; i++)
            {
                // High charge (Zone 5 normally rolls 8-12)
                DiceResult result = DiceRollResolver.ResolveRoll(90f, currentTile);
                if (result.value <= needed)
                {
                    smallRollCount++;
                }
            }
            
            // Under normal circumstances at 90% charge (Zone 5), rolls <= 4 are extremely rare (only 10% chance of random fallthrough, divided by 12 = ~0.83% chance).
            // But Near-Finish Strategy forces roll to be in [1, 4] with a 35% probability.
            // Out of 200 runs, we should definitely see a significant number of rolls <= 4 (expected around 70).
            Assert.IsTrue(smallRollCount > 0, "Near-Finish Strategy should trigger and provide rolls <= needed. Found: " + smallRollCount);
        }
    }
}
