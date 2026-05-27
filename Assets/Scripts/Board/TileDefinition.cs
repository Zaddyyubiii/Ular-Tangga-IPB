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

        // Minimal field aliases as requested
        public int tileNumber { get { return tileIndex; } set { tileIndex = value; } }
        public TileType tileType { get { return type; } set { type = value; } }
        public int destinationTile { get { return targetTileIndex; } set { targetTileIndex = value; } }
        public string message { get { return customMessage; } set { customMessage = value; } }
        public int severityOrLevel { get { return severity; } set { severity = value; } }
    }
}
