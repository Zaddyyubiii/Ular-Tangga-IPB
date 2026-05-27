using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public static class BoardValidator
    {
        public static bool ValidateBoard(RuntimeBoardConfig config, BoardConfig originalConfig = null)
        {
            if (config == null) return false;

            int questionCount = 0;
            int skullCount = 0;
            int snakeCount = config.snakes.Count;
            int ladderCount = config.ladders.Count;

            HashSet<int> specialTiles = new HashSet<int>();
            HashSet<int> starts = new HashSet<int>();
            HashSet<int> endpoints = new HashSet<int>();

            foreach (var kvp in config.tiles)
            {
                int index = kvp.Key;
                TileDefinition def = kvp.Value;

                if (index < 0 || index > 100) return false;

                if (def.type == TileType.Question)
                {
                    questionCount++;
                    specialTiles.Add(index);
                }
                else if (def.type == TileType.Skull)
                {
                    skullCount++;
                    specialTiles.Add(index);
                }
                else if (def.type == TileType.Start)
                {
                    if (index != 0) return false;
                }
                else if (def.type == TileType.Finish)
                {
                    if (index != 100) return false;
                }
            }

            // Verify Question/Skull counts
            if (questionCount != 6) return false;
            if (skullCount != 3) return false;

            int expectedSnakes = 5;
            int expectedLadders = 5;
            if (originalConfig != null)
            {
                if (originalConfig.snakes != null && originalConfig.snakes.Count > 0) expectedSnakes = originalConfig.snakes.Count;
                if (originalConfig.ladders != null && originalConfig.ladders.Count > 0) expectedLadders = originalConfig.ladders.Count;
            }

            if (snakeCount != expectedSnakes) return false;
            if (ladderCount != expectedLadders) return false;

            // Check snakes
            foreach (var snake in config.snakes)
            {
                if (snake.tileIndex <= 1 || snake.tileIndex >= 100) return false;
                if (snake.targetTileIndex < 1 || snake.targetTileIndex >= snake.tileIndex) return false;
                
                if (specialTiles.Contains(snake.tileIndex)) return false;
                specialTiles.Add(snake.tileIndex);

                starts.Add(snake.tileIndex);
                endpoints.Add(snake.targetTileIndex);
            }

            // Check ladders
            foreach (var ladder in config.ladders)
            {
                if (ladder.tileIndex <= 1 || ladder.tileIndex >= 100) return false;
                if (ladder.targetTileIndex <= ladder.tileIndex || ladder.targetTileIndex >= 100) return false;

                if (specialTiles.Contains(ladder.tileIndex)) return false;
                specialTiles.Add(ladder.tileIndex);

                starts.Add(ladder.tileIndex);
                endpoints.Add(ladder.targetTileIndex);
            }

            // Endpoint overlaps checks
            foreach (int ep in endpoints)
            {
                if (starts.Contains(ep)) return false;
            }

            // Check radius rule: no more than 2 special tiles in radius 3
            List<int> sortedSpecials = new List<int>(specialTiles);
            sortedSpecials.Sort();
            for (int i = 0; i < sortedSpecials.Count; i++)
            {
                int countInRange = 0;
                for (int j = 0; j < sortedSpecials.Count; j++)
                {
                    if (Mathf.Abs(sortedSpecials[i] - sortedSpecials[j]) <= 3)
                    {
                        countInRange++;
                    }
                }
                if (countInRange > 2) return false;
            }

            // Check zones distribution
            int qZone1 = 0, qZone2 = 0, qZone3 = 0, qZone4 = 0;
            int sZone1 = 0, sZone2 = 0, sZone3 = 0, sZone4 = 0;
            int snakeZone1 = 0, snakeZone2 = 0, snakeZone3 = 0, snakeZone4 = 0;
            int ladderZone1 = 0, ladderZone2 = 0, ladderZone3 = 0, ladderZone4 = 0;

            foreach (int index in specialTiles)
            {
                TileType type = config.tiles.ContainsKey(index) ? config.tiles[index].type : TileType.Normal;
                int zone = GetZone(index);
                if (type == TileType.Question)
                {
                    if (zone == 1) qZone1++;
                    else if (zone == 2) qZone2++;
                    else if (zone == 3) qZone3++;
                    else if (zone == 4) qZone4++;
                }
                else if (type == TileType.Skull)
                {
                    if (zone == 1) sZone1++;
                    else if (zone == 2) sZone2++;
                    else if (zone == 3) sZone3++;
                    else if (zone == 4) sZone4++;
                }
            }

            foreach (var snake in config.snakes)
            {
                int zone = GetZone(snake.tileIndex);
                if (zone == 1) snakeZone1++;
                else if (zone == 2) snakeZone2++;
                else if (zone == 3) snakeZone3++;
                else if (zone == 4) snakeZone4++;
            }

            foreach (var ladder in config.ladders)
            {
                int zone = GetZone(ladder.tileIndex);
                if (zone == 1) ladderZone1++;
                else if (zone == 2) ladderZone2++;
                else if (zone == 3) ladderZone3++;
                else if (zone == 4) ladderZone4++;
            }

            int questionZones = (qZone1 > 0 ? 1 : 0) + (qZone2 > 0 ? 1 : 0) + (qZone3 > 0 ? 1 : 0) + (qZone4 > 0 ? 1 : 0);
            int skullZones = (sZone1 > 0 ? 1 : 0) + (sZone2 > 0 ? 1 : 0) + (sZone3 > 0 ? 1 : 0) + (sZone4 > 0 ? 1 : 0);
            int snakeZones = (snakeZone1 > 0 ? 1 : 0) + (snakeZone2 > 0 ? 1 : 0) + (snakeZone3 > 0 ? 1 : 0) + (snakeZone4 > 0 ? 1 : 0);
            int ladderZones = (ladderZone1 > 0 ? 1 : 0) + (ladderZone2 > 0 ? 1 : 0) + (ladderZone3 > 0 ? 1 : 0) + (ladderZone4 > 0 ? 1 : 0);

            if (questionZones < 3) return false;
            if (skullZones < 2) return false;
            if (snakeZones < 3) return false;
            if (ladderZones < 3) return false;

            // Infinite loop checks
            if (HasInfiniteLoop(config)) return false;

            return true;
        }

        private static int GetZone(int tile)
        {
            if (tile >= 1 && tile <= 25) return 1;
            if (tile >= 26 && tile <= 50) return 2;
            if (tile >= 51 && tile <= 75) return 3;
            if (tile >= 76 && tile <= 99) return 4;
            return 0;
        }

        private static bool HasInfiniteLoop(RuntimeBoardConfig config)
        {
            Dictionary<int, int> transitions = new Dictionary<int, int>();
            foreach (var snake in config.snakes)
            {
                transitions[snake.tileIndex] = snake.targetTileIndex;
            }
            foreach (var ladder in config.ladders)
            {
                transitions[ladder.tileIndex] = ladder.targetTileIndex;
            }

            for (int start = 1; start <= 100; start++)
            {
                int slow = start;
                int fast = start;

                while (true)
                {
                    if (transitions.ContainsKey(slow)) slow = transitions[slow];
                    else break;

                    if (transitions.ContainsKey(fast)) fast = transitions[fast];
                    else break;
                    if (transitions.ContainsKey(fast)) fast = transitions[fast];
                    else break;

                    if (slow == fast) return true; // Loop detected
                }
            }
            return false;
        }
    }
}
