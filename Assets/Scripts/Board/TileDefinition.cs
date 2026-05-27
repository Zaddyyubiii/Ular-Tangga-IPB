using System;

namespace Board
{
    [Serializable]
    public class TileDefinition
    {
        public int tileIndex;
        public TileType type;
        public int targetTileIndex; // For Snakes and Ladders
        public string customMessage;
        public int severity; // 0 = Light, 1 = Medium, 2 = Severe/Heavy
    }
}
