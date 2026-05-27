using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Board
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "UlarTangga/BoardConfig")]
    public class BoardConfig : ScriptableObject
    {
        [Header("Special Tile Indices")]
        public List<int> questionTiles = new List<int>() { 6, 20, 34, 46, 64, 85 };
        public List<int> skullTiles = new List<int>() { 15, 59, 72 };

        [Header("Snakes Configurations")]
        public List<TileDefinition> snakes = new List<TileDefinition>()
        {
            new TileDefinition { tileIndex = 35, targetTileIndex = 25, type = TileType.Snake, severity = 0, customMessage = "Pelanggaran Ringan! Anda parkir sembarangan. Turun 1 baris." },
            new TileDefinition { tileIndex = 45, targetTileIndex = 35, type = TileType.Snake, severity = 0, customMessage = "Pelanggaran Ringan! Anda parkir sembarangan. Turun 1 baris." },
            new TileDefinition { tileIndex = 63, targetTileIndex = 43, type = TileType.Snake, severity = 1, customMessage = "Pelanggaran Sedang! Anda merusak fasilitas IPB. Turun 2 baris." },
            new TileDefinition { tileIndex = 88, targetTileIndex = 68, type = TileType.Snake, severity = 1, customMessage = "Pelanggaran Sedang! Anda merusak fasilitas IPB. Turun 2 baris." },
            new TileDefinition { tileIndex = 93, targetTileIndex = 73, type = TileType.Snake, severity = 1, customMessage = "Pelanggaran Sedang! Anda merusak fasilitas IPB. Turun 2 baris." }
        };

        [Header("Ladders Configurations")]
        public List<TileDefinition> ladders = new List<TileDefinition>()
        {
            new TileDefinition { tileIndex = 8, targetTileIndex = 28, type = TileType.Ladder, severity = 1, customMessage = "Karya tulis ilmiahmu memenangkan kompetisi tingkat nasional. Naik 2 baris." },
            new TileDefinition { tileIndex = 18, targetTileIndex = 38, type = TileType.Ladder, severity = 1, customMessage = "Kamu diterima magang karena konsisten membangun portofolio. Naik 2 baris." },
            new TileDefinition { tileIndex = 21, targetTileIndex = 41, type = TileType.Ladder, severity = 1, customMessage = "Kamu diterima magang karena konsisten membangun portofolio. Naik 2 baris." },
            new TileDefinition { tileIndex = 48, targetTileIndex = 68, type = TileType.Ladder, severity = 1, customMessage = "Kamu diterima magang karena konsisten membangun portofolio. Naik 2 baris." },
            new TileDefinition { tileIndex = 65, targetTileIndex = 85, type = TileType.Ladder, severity = 1, customMessage = "Kamu diterima magang karena konsisten membangun portofolio. Naik 2 baris." }
        };

        public TileDefinition GetTileDefinition(int tileIndex)
        {
            // Check Start & Finish
            if (tileIndex == 0)
                return new TileDefinition { tileIndex = 0, type = TileType.Start };
            if (tileIndex == 100)
                return new TileDefinition { tileIndex = 100, type = TileType.Finish };

            // Check Snakes
            foreach (var snake in snakes)
            {
                if (snake.tileIndex == tileIndex)
                    return snake;
            }

            // Check Ladders
            foreach (var ladder in ladders)
            {
                if (ladder.tileIndex == tileIndex)
                    return ladder;
            }

            // Check Skull
            if (skullTiles.Contains(tileIndex))
            {
                return new TileDefinition 
                { 
                    tileIndex = tileIndex, 
                    type = TileType.Skull,
                    customMessage = "Pelanggaran Berat! Anda melakukan pelanggaran serius terhadap tata tertib kampus."
                };
            }

            // Check Question
            if (questionTiles.Contains(tileIndex))
            {
                return new TileDefinition { tileIndex = tileIndex, type = TileType.Question };
            }

            // Fallback to Normal
            return new TileDefinition { tileIndex = tileIndex, type = TileType.Normal };
        }

        public void ValidateConfig()
        {
            HashSet<int> occupied = new HashSet<int>();
            WarnDuplicateEffects(questionTiles, occupied, "Question");
            WarnDuplicateEffects(skullTiles, occupied, "Skull");

            ValidateTransitions(snakes, occupied, TileType.Snake);
            ValidateTransitions(ladders, occupied, TileType.Ladder);
        }

        private void ValidateTransitions(List<TileDefinition> definitions, HashSet<int> occupied, TileType expectedType)
        {
            if (definitions == null) return;

            foreach (TileDefinition definition in definitions.Where(d => d != null))
            {
                if (definition.type != expectedType)
                {
                    Debug.LogWarning($"[BoardConfig] Tile {definition.tileIndex} is listed as {expectedType} but has type {definition.type}.");
                }

                if (definition.tileIndex <= 0 || definition.tileIndex >= 100 || definition.targetTileIndex < 0 || definition.targetTileIndex > 100)
                {
                    Debug.LogWarning($"[BoardConfig] Invalid {expectedType} transition {definition.tileIndex} -> {definition.targetTileIndex}.");
                }

                if (occupied.Contains(definition.tileIndex))
                {
                    Debug.LogWarning($"[BoardConfig] Tile {definition.tileIndex} has more than one primary effect.");
                }
                else
                {
                    occupied.Add(definition.tileIndex);
                }
            }
        }

        private void WarnDuplicateEffects(List<int> tileIndices, HashSet<int> occupied, string label)
        {
            if (tileIndices == null) return;

            foreach (int tileIndex in tileIndices)
            {
                if (tileIndex <= 0 || tileIndex >= 100)
                {
                    Debug.LogWarning($"[BoardConfig] {label} tile {tileIndex} should be between 1 and 99.");
                }

                if (occupied.Contains(tileIndex))
                {
                    Debug.LogWarning($"[BoardConfig] Tile {tileIndex} has more than one primary effect.");
                }
                else
                {
                    occupied.Add(tileIndex);
                }
            }
        }
    }
}
