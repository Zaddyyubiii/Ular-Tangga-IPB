using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public class RuntimeBoardConfig
    {
        public Dictionary<int, TileDefinition> tiles = new Dictionary<int, TileDefinition>();
        public List<TileDefinition> snakes = new List<TileDefinition>();
        public List<TileDefinition> ladders = new List<TileDefinition>();

        public TileDefinition GetTileDefinition(int tileIndex)
        {
            // Check Start & Finish
            if (tileIndex == 0)
                return new TileDefinition { tileIndex = 0, type = TileType.Start };
            if (tileIndex == 100)
                return new TileDefinition { tileIndex = 100, type = TileType.Finish };

            // Check dictionary
            if (tiles.TryGetValue(tileIndex, out TileDefinition definition))
            {
                return definition;
            }

            // Fallback to normal tile
            return new TileDefinition { tileIndex = tileIndex, type = TileType.Normal };
        }
    }
}
