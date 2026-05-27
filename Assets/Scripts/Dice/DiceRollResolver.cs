using System;
using UnityEngine;

namespace Dice
{
    public static class DiceRollResolver
    {
        public static DiceResult ResolveRoll(float chargePercent, int currentTile)
        {
            // 1. Calculate timing quality based on target points: 25%, 50%, 75%, 95%
            float[] targets = new float[] { 25f, 50f, 75f, 95f };
            float minDiff = float.MaxValue;
            foreach (float target in targets)
            {
                float diff = Mathf.Abs(chargePercent - target);
                if (diff < minDiff) minDiff = diff;
            }

            string timingQuality = "Normal";
            float mainZoneChance = 0.70f;
            if (minDiff <= 3f)
            {
                timingQuality = "Perfect";
                mainZoneChance = 0.90f;
            }
            else if (minDiff <= 7f)
            {
                timingQuality = "Good";
                mainZoneChance = 0.80f;
            }

            // 2. Determine Zone
            string zoneName = "Zona 1";
            int[] mainList;
            int[] neighborList;

            if (chargePercent <= 20f)
            {
                zoneName = "Zona 1";
                mainList = new int[] { 1, 2, 3 };
                neighborList = new int[] { 4, 5 };
            }
            else if (chargePercent <= 40f)
            {
                zoneName = "Zona 2";
                mainList = new int[] { 3, 4, 5 };
                neighborList = new int[] { 1, 2, 6, 7 };
            }
            else if (chargePercent <= 60f)
            {
                zoneName = "Zona 3";
                mainList = new int[] { 5, 6, 7, 8 };
                neighborList = new int[] { 3, 4, 9, 10 };
            }
            else if (chargePercent <= 80f)
            {
                zoneName = "Zona 4";
                mainList = new int[] { 8, 9, 10 };
                neighborList = new int[] { 6, 7, 11, 12 };
            }
            else
            {
                zoneName = "Zona 5";
                mainList = new int[] { 10, 11, 12 };
                neighborList = new int[] { 8, 9 };
            }

            // 3. Roll using weighted random
            float rand = UnityEngine.Random.value;
            int diceValue = 1;

            if (rand < mainZoneChance)
            {
                // Main zone
                diceValue = mainList[UnityEngine.Random.Range(0, mainList.Length)];
            }
            else if (rand < mainZoneChance + 0.20f)
            {
                // Neighbor zone
                diceValue = neighborList[UnityEngine.Random.Range(0, neighborList.Length)];
            }
            else
            {
                // Absolute random 1-12
                diceValue = UnityEngine.Random.Range(1, 13);
            }

            // 4. Near-Finish Strategy (Tile >= 88)
            // If the player is close to finishing, we want to make sure they can get needed numbers.
            if (currentTile >= 88)
            {
                int needed = 100 - currentTile;
                if (needed > 0 && needed <= 12)
                {
                    // 35% chance to force the needed or smaller rolls so they don't overshoot excessively.
                    if (UnityEngine.Random.value < 0.35f)
                    {
                        diceValue = UnityEngine.Random.Range(1, needed + 1);
                        Debug.Log($"[Near Finish Strategy] Overriding roll to {diceValue} (needed <= {needed})");
                    }
                }
            }

            // Ensure values are within 1-12
            diceValue = Mathf.Clamp(diceValue, 1, 12);

            return new DiceResult(diceValue, chargePercent, timingQuality, zoneName);
        }

        // Helper to find the best charge percent for bots targeting a specific dice value
        public static float GetTargetChargeForValue(int targetValue)
        {
            switch (targetValue)
            {
                case 1:
                case 2:
                    return UnityEngine.Random.Range(5f, 15f); // Zona 1
                case 3:
                    return UnityEngine.Random.Range(15f, 25f); // Border Zona 1/2
                case 4:
                    return UnityEngine.Random.Range(25f, 35f); // Zona 2
                case 5:
                    return UnityEngine.Random.Range(35f, 45f); // Border Zona 2/3
                case 6:
                case 7:
                    return UnityEngine.Random.Range(45f, 55f); // Zona 3
                case 8:
                    return UnityEngine.Random.Range(55f, 65f); // Border Zona 3/4
                case 9:
                    return UnityEngine.Random.Range(65f, 75f); // Zona 4
                case 10:
                    return UnityEngine.Random.Range(75f, 85f); // Border Zona 4/5
                case 11:
                case 12:
                    return UnityEngine.Random.Range(85f, 95f); // Zona 5
                default:
                    return UnityEngine.Random.Range(50f, 90f);
            }
        }
    }
}
