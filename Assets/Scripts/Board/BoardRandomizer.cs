using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    public static class BoardRandomizer
    {
        public static RuntimeBoardConfig GenerateBoard(BoardConfig originalConfig, int seed, Core.MessageBank messageBank)
        {
            int attempts = 0;
            while (attempts < 100)
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
            int snakeCount = originalConfig != null && originalConfig.snakes != null ? originalConfig.snakes.Count : 5;
            int ladderCount = originalConfig != null && originalConfig.ladders != null ? originalConfig.ladders.Count : 5;

            // Predefine severity and level arrays
            List<int> snakeSeverities = new List<int>();
            for (int i = 0; i < snakeCount; i++)
            {
                // Balance combination: severity 0 (light), 1 (medium), 2 (heavy)
                if (i < 2) snakeSeverities.Add(0); // 2 light
                else if (i < 4) snakeSeverities.Add(1); // 2 medium
                else snakeSeverities.Add(2); // 1 heavy (and scale beyond that)
            }
            ShuffleList(snakeSeverities);

            List<int> ladderLevels = new List<int>();
            for (int i = 0; i < ladderCount; i++)
            {
                // Balance combination: level 0 (short), 1 (medium), 2 (long)
                if (i < 2) ladderLevels.Add(0); // 2 short
                else if (i < 4) ladderLevels.Add(1); // 2 medium
                else ladderLevels.Add(2); // 1 long (and scale beyond that)
            }
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
                int targetZone = PickZoneFromQuota(ladderQuota);
                if (targetZone == -1) return null;

                bool placed = false;
                List<int> candidates = new List<int>(zoneTiles[targetZone]);
                ShuffleList(candidates);

                foreach (int start in candidates)
                {
                    int dest = start + (level == 0 ? 10 : (level == 1 ? 20 : 30));
                    if (dest > 99) continue;

                    if (IsValidPlacement(start, occupied) && !starts.Contains(dest) && !occupied.Contains(dest))
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
                int targetZone = PickZoneFromQuota(snakeQuota);
                if (targetZone == -1) return null;

                bool placed = false;
                List<int> candidates = new List<int>(zoneTiles[targetZone]);
                ShuffleList(candidates);

                foreach (int start in candidates)
                {
                    int dest = start - (severity == 0 ? 10 : (severity == 1 ? 20 : 40));
                    dest = Mathf.Max(1, dest);

                    if (dest >= start) continue; // Safety check

                    if (IsValidPlacement(start, occupied) && !starts.Contains(dest) && !occupied.Contains(dest))
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

                    if (IsValidPlacement(start, occupied))
                    {
                        TileDefinition def = new TileDefinition
                        {
                            tileIndex = start,
                            type = TileType.Skull,
                            customMessage = "Pelanggaran Berat! Anda mendapat sanksi akademik skorsing."
                        };

                        config.tiles[start] = def;
                        occupied.Add(start);
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
                    if (IsValidPlacement(start, occupied))
                    {
                        TileDefinition def = new TileDefinition
                        {
                            tileIndex = start,
                            type = TileType.Question
                        };

                        config.tiles[start] = def;
                        occupied.Add(start);
                        placed = true;
                        break;
                    }
                }
                if (!placed) return null;
            }

            return config;
        }

        private static bool IsValidPlacement(int tile, HashSet<int> occupied)
        {
            if (occupied.Contains(tile)) return false;

            // Radius check: no more than 2 special tiles in [tile-3, tile+3]
            int specialCount = 0;
            for (int i = -3; i <= 3; i++)
            {
                int checkTile = tile + i;
                if (occupied.Contains(checkTile))
                {
                    specialCount++;
                }
            }

            // Adding this tile would make it 2, which is allowed, but > 2 is forbidden
            if (specialCount >= 2) return false;

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
