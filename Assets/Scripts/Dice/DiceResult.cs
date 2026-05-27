using System;

namespace Dice
{
    [Serializable]
    public struct DiceResult
    {
        public int value;
        public float chargePercent;
        public string levelLabel;
        public string rangeLabel;

        public DiceResult(int value, float chargePercent, string levelLabel, string rangeLabel)
        {
            this.value = value;
            this.chargePercent = chargePercent;
            this.levelLabel = levelLabel;
            this.rangeLabel = rangeLabel;
        }
    }
}
