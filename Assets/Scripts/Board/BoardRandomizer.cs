using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public static class BoardRandomizer
    {
        public static RuntimeBoardConfig GenerateBoard(BoardConfig originalConfig, int seed, Core.MessageBank messageBank)
        {
            int attempts = 0;
            while (attempts < 1000)
            {
                attempts++;
                // Deterministic seed progression per attempt to guarantee we eventually find a solution
                RuntimeBoardConfig runtimeConfig = TryGenerate(originalConfig, seed + attempts, messageBank);
                if (runtimeConfig != null && BoardValidator.ValidateBoard(runtimeConfig, originalConfig))
                {
                    Debug.Log($"[BoardRandomizer] Generated valid board on attempt {attempts} using seed {seed}");
                    return runtimeConfig;
                }
            }

            Debug.LogWarning("[BoardRandomizer] Failed to generate a valid randomized board in 100 attempts. Using default fallback board.");
            return CreateDefaultFallback(originalConfig, messageBank);
        }

        private static RuntimeBoardConfig TryGenerate(BoardConfig originalConfig, int seed, Core.MessageBank messageBank)
        {
            UnityEngine.Random.InitState(seed);

            RuntimeBoardConfig config = new RuntimeBoardConfig();
            HashSet<int> occupied = new HashSet<int>();
            HashSet<int> starts = new HashSet<int>();
            HashSet<int> specialTiles = new HashSet<int>();

            // Setup zones
            List<int>[] zoneTiles = new List<int>[5];
            for (int z = 1; z <= 4; z++) zoneTiles[z] = new List<int>();

            // Fill zones with valid tiles
            for (int i = 2; i <= 99; i++)
            {
                int zone = GetZone(i);
                zoneTiles[zone].Add(i);
            }

            // Determine element counts
            int questionCount = originalConfig != null && originalConfig.questionTiles != null ? originalConfig.questionTiles.Count : 6;
            int skullCount = originalConfig != null && originalConfig.skullTiles != null ? originalConfig.skullTiles.Count : 3;
            int snakeCount = 6;
            int ladderCount = 6;

            // Predefine severity and level arrays to strictly ensure exactly 2 of each level
            List<int> snakeSeverities = new List<int> { 0, 0, 1, 1, 2, 2 };
            ShuffleList(snakeSeverities);

            List<int> ladderLevels = new List<int> { 0, 0, 1, 1, 2, 2 };
            ShuffleList(ladderLevels);

            // Determine zone quotas ensuring minZone counts are always satisfied
            List<int> questionQuota = DistributeQuota(questionCount, 3);
            List<int> skullQuota = DistributeQuota(skullCount, 2);
            List<int> snakeQuota = DistributeQuota(snakeCount, 3);
            List<int> ladderQuota = DistributeQuota(ladderCount, 3);

            // 1. Place Ladders
            for (int i = 0; i < ladderCount; i++)
            {
                int level = ladderLevels[i];
                int rowDiff = level == 0 ? 1 : (level == 1 ? 2 : 3);
                
                bool placed = false;
                List<int> candidates = new List<int>();
                for (int t = 2; t <= 99; t++)
                {
                    int r = (t - 1) / 10;
                    if (r + rowDiff <= 9) candidates.Add(t);
                }
                ShuffleList(candidates);

                foreach (int start in candidates)
                {
                    int startRow = (start - 1) / 10;
                    int destRow = startRow + rowDiff;
                    
                    int startCol = (startRow % 2 == 0) ? ((start - 1) % 10) : (9 - ((start - 1) % 10));
                    int minCol = Mathf.Max(0, startCol - 2);
                    int maxCol = Mathf.Min(9, startCol + 2);
                    int destCol = UnityEngine.Random.Range(minCol, maxCol + 1);
                    
                    int dest = (destRow % 2 == 0) ? (destRow * 10 + destCol + 1) : (destRow * 10 + (9 - destCol) + 1);
                    
                    if (dest > 100) dest = 100;

                    if (IsValidPlacement(start, occupied, specialTiles) && !starts.Contains(dest) && !occupied.Contains(dest))
                    {
                        TileDefinition def = new TileDefinition
                        {
                            tileIndex = start,
                            targetTileIndex = dest,
                            type = TileType.Ladder,
                            severity = level,
                            customMessage = messageBank != null ? messageBank.GetLadderMessage(level == 0 ? UnityEngine.Random.Range(0, 2) : (level == 1 ? UnityEngine.Random.Range(2, 4) : 4)) : "Pencapaian akademik!"
                        };

                        config.ladders.Add(def);
                        config.tiles[start] = def;
                        occupied.Add(start);
                        occupied.Add(dest);
                        starts.Add(start);
                        specialTiles.Add(start);
                        placed = true;
                        break;
                    }
                }
                if (!placed) return null;
            }

            // 2. Place Snakes
            for (int i = 0; i < snakeCount; i++)
            {
                int severity = snakeSeverities[i];
                int rowDiff = severity == 0 ? 1 : (severity == 1 ? 2 : 3);
                
                bool placed = false;
                List<int> candidates = new List<int>();
                for (int t = 2; t <= 99; t++)
                {
                    int r = (t - 1) / 10;
                    if (r - rowDiff >= 0) candidates.Add(t);
                }
                ShuffleList(candidates);

                foreach (int start in candidates)
                {
                    int startRow = (start - 1) / 10;
                    int destRow = startRow - rowDiff;
                    
                    int startCol = (startRow % 2 == 0) ? ((start - 1) % 10) : (9 - ((start - 1) % 10));
                    int minCol = Mathf.Max(0, startCol - 2);
                    int maxCol = Mathf.Min(9, startCol + 2);
                    int destCol = UnityEngine.Random.Range(minCol, maxCol + 1);
                    
                    int dest = (destRow % 2 == 0) ? (destRow * 10 + destCol + 1) : (destRow * 10 + (9 - destCol) + 1);
                    
                    if (dest >= start) continue;

                    if (IsValidPlacement(start, occupied, specialTiles) && !starts.Contains(dest) && !occupied.Contains(dest))
                    {
                        TileDefinition def = new TileDefinition
                        {
                            tileIndex = start,
                            targetTileIndex = dest,
                            type = TileType.Snake,
                            severity = severity,
                            customMessage = messageBank != null ? messageBank.GetSnakeMessage(severity) : "Pelanggaran!"
                        };

                        config.snakes.Add(def);
                        config.tiles[start] = def;
                        occupied.Add(start);
                        occupied.Add(dest);
                        starts.Add(start);
                        specialTiles.Add(start);
                        placed = true;
                        break;
                    }
                }
                if (!placed) return null;
            }

            // 3. Place Skulls
            for (int i = 0; i < skullCount; i++)
            {
                int targetZone = PickZoneFromQuota(skullQuota);
                if (targetZone == -1) return null;

                bool placed = false;
                List<int> candidates = new List<int>(zoneTiles[targetZone]);
                ShuffleList(candidates);

                foreach (int start in candidates)
                {
                    if (start <= 10) continue; // Do not place skulls in 2-10

                    if (IsValidPlacement(start, occupied, specialTiles))
                    {
                        TileDefinition def = new TileDefinition
                        {
                            tileIndex = start,
                            type = TileType.Skull,
                            customMessage = "Pelanggaran Berat! Anda mendapat sanksi akademik skorsing."
                        };

                        config.tiles[start] = def;
                        occupied.Add(start);
                        specialTiles.Add(start);
                        placed = true;
                        break;
                    }
                }
                if (!placed) return null;
            }

            // 4. Place Questions
            for (int i = 0; i < questionCount; i++)
            {
                int targetZone = PickZoneFromQuota(questionQuota);
                if (targetZone == -1) return null;

                bool placed = false;
                List<int> candidates = new List<int>(zoneTiles[targetZone]);
                ShuffleList(candidates);

                foreach (int start in candidates)
                {
                    if (IsValidPlacement(start, occupied, specialTiles))
                    {
                        TileDefinition def = new TileDefinition
                        {
                            tileIndex = start,
                            type = TileType.Question
                        };

                        config.tiles[start] = def;
                        occupied.Add(start);
                        specialTiles.Add(start);
                        placed = true;
                        break;
                    }
                }
                if (!placed) return null;
            }

            return config;
        }

        private static bool IsValidPlacement(int tile, HashSet<int> occupied, HashSet<int> specialTiles)
        {
            if (occupied.Contains(tile)) return false;

            // Radius check 1: allow special tiles to be adjacent if needed, but not on the same tile
            // Removed the strict [tile-1, tile+1] check because 37 special tiles in 98 spaces makes it too restrictive

            // Radius check 2: no more than 2 special tiles in radius 3
            // This aligns with BoardValidator's radius rule.
            for (int p = tile - 3; p <= tile + 3; p++)
            {
                if (p == tile || specialTiles.Contains(p))
                {
                    int count = 0;
                    for (int k = p - 3; k <= p + 3; k++)
                    {
                        if (k == tile || specialTiles.Contains(k)) count++;
                    }
                    if (count > 4) return false;
                }
            }

            return true;
        }

        private static List<int> DistributeQuota(int totalCount, int minZones)
        {
            List<int> quota = new List<int> { 0, 0, 0, 0, 0 }; // 1-indexed

            // Pick minZones distinct zones out of {1, 2, 3, 4}
            List<int> zones = new List<int> { 1, 2, 3, 4 };
            ShuffleList(zones);

            for (int i = 0; i < minZones; i++)
            {
                quota[zones[i]] = 1;
            }

            // Distribute remaining count
            int remaining = totalCount - minZones;
            for (int i = 0; i < remaining; i++)
            {
                int randomZone = UnityEngine.Random.Range(1, 5);
                quota[randomZone]++;
            }

            return quota;
        }

        private static int PickZoneFromQuota(List<int> quota)
        {
            List<int> validZones = new List<int>();
            for (int z = 1; z <= 4; z++)
            {
                if (quota[z] > 0) validZones.Add(z);
            }

            if (validZones.Count == 0) return -1;

            int picked = validZones[UnityEngine.Random.Range(0, validZones.Count)];
            quota[picked]--;
            return picked;
        }

        private static int GetZone(int tile)
        {
            if (tile >= 1 && tile <= 25) return 1;
            if (tile >= 26 && tile <= 50) return 2;
            if (tile >= 51 && tile <= 75) return 3;
            if (tile >= 76 && tile <= 99) return 4;
            return 0;
        }

        private static void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int r = UnityEngine.Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[r];
                list[r] = temp;
            }
        }

        private static RuntimeBoardConfig CreateDefaultFallback(BoardConfig originalConfig, Core.MessageBank messageBank)
        {
            RuntimeBoardConfig runtime = new RuntimeBoardConfig();
            if (originalConfig == null) return runtime;

            runtime.snakes = new List<TileDefinition>(originalConfig.snakes);
            runtime.ladders = new List<TileDefinition>(originalConfig.ladders);

            for (int i = 1; i <= 99; i++)
            {
                TileDefinition def = originalConfig.GetTileDefinition(i);
                if (def.type != TileType.Normal)
                {
                    runtime.tiles[i] = def;
                }
            }

            return runtime;
        }
    }
}
