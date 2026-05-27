using System;

namespace Dice
{
    [Serializable]
    public struct DiceResult
    {
        public int value;
        public float chargePercent;
        public string timingQuality; // "Perfect", "Good", "Normal"
        public string zoneName;      // "Zona 1" to "Zona 5"

        public DiceResult(int value, float chargePercent, string timingQuality, string zoneName)
        {
            this.value = value;
            this.chargePercent = chargePercent;
            this.timingQuality = timingQuality;
            this.zoneName = zoneName;
        }
    }
}
