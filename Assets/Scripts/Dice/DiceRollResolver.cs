using System;
using UnityEngine;

namespace Dice
{
    public static class DiceRollResolver
    {
        public static DiceResult ResolveRoll(float chargePercent, int currentTile)
        {
            // 1. Calculate timing quality based on target points: 12.5%, 37.5%, 62.5%, 87.5% (centers of the 4 zones)
            float[] targets = new float[] { 12.5f, 37.5f, 62.5f, 87.5f };
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

            if (chargePercent <= 25f)
            {
                zoneName = "Zona 1";
                mainList = new int[] { 2, 3 };
                neighborList = new int[] { 4, 5 };
            }
            else if (chargePercent <= 50f)
            {
                zoneName = "Zona 2";
                mainList = new int[] { 4, 5, 6 };
                neighborList = new int[] { 2, 3, 7 };
            }
            else if (chargePercent <= 75f)
            {
                zoneName = "Zona 3";
                mainList = new int[] { 7, 8, 9 };
                neighborList = new int[] { 6, 10 };
            }
            else
            {
                zoneName = "Zona 4";
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
                // Absolute random 2-12
                diceValue = UnityEngine.Random.Range(2, 13);
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
                case 2:
                case 3:
                    return UnityEngine.Random.Range(5f, 20f); // Zona 1
                case 4:
                case 5:
                case 6:
                    return UnityEngine.Random.Range(30f, 45f); // Zona 2
                case 7:
                case 8:
                case 9:
                    return UnityEngine.Random.Range(55f, 70f); // Zona 3
                case 10:
                case 11:
                case 12:
                    return UnityEngine.Random.Range(80f, 95f); // Zona 4
                default:
                    return UnityEngine.Random.Range(30f, 70f);
            }
        }
    }
}
