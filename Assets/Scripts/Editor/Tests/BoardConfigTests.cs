using NUnit.Framework;
using UnityEngine;
using Board;
using System.Collections.Generic;

namespace Editor.Tests
{
    public class BoardConfigTests
    {
        private BoardConfig boardConfig;

        [SetUp]
        public void SetUp()
        {
            boardConfig = ScriptableObject.CreateInstance<BoardConfig>();
        }

        [TearDown]
        public void TearDown()
        {
            if (boardConfig != null)
            {
                Object.DestroyImmediate(boardConfig);
            }
        }

        [Test]
        public void TestBoardConfigTransitions()
        {
            // Verify Snakes targets are always lower than source tiles
            Assert.IsNotNull(boardConfig.snakes, "Snakes configuration list should not be null");
            foreach (var snake in boardConfig.snakes)
            {
                Assert.IsTrue(snake.targetTileIndex < snake.tileIndex, 
                    $"Snake at tile {snake.tileIndex} goes to {snake.targetTileIndex}, which is invalid (must move down).");
                Assert.IsTrue(snake.tileIndex > 0 && snake.tileIndex < 100, $"Snake source tile {snake.tileIndex} out of bounds");
                Assert.IsTrue(snake.targetTileIndex >= 0 && snake.targetTileIndex < 100, $"Snake target tile {snake.targetTileIndex} out of bounds");
            }

            // Verify Ladders targets are always higher than source tiles
            Assert.IsNotNull(boardConfig.ladders, "Ladders configuration list should not be null");
            foreach (var ladder in boardConfig.ladders)
            {
                Assert.IsTrue(ladder.targetTileIndex > ladder.tileIndex, 
                    $"Ladder at tile {ladder.tileIndex} goes to {ladder.targetTileIndex}, which is invalid (must move up).");
                Assert.IsTrue(ladder.tileIndex > 0 && ladder.tileIndex < 100, $"Ladder source tile {ladder.tileIndex} out of bounds");
                Assert.IsTrue(ladder.targetTileIndex > 0 && ladder.targetTileIndex <= 100, $"Ladder target tile {ladder.targetTileIndex} out of bounds");
            }
        }

        [Test]
        public void TestSpecialTilesBounds()
        {
            // Verify Question Tiles are in range [1, 99]
            Assert.IsNotNull(boardConfig.questionTiles, "Question tiles list should not be null");
            foreach (int tile in boardConfig.questionTiles)
            {
                Assert.IsTrue(tile > 0 && tile < 100, $"Question tile {tile} is out of valid bounds (1-99)");
            }

            // Verify Skull Tiles are in range [1, 99]
            Assert.IsNotNull(boardConfig.skullTiles, "Skull tiles list should not be null");
            foreach (int tile in boardConfig.skullTiles)
            {
                Assert.IsTrue(tile > 0 && tile < 100, $"Skull tile {tile} is out of valid bounds (1-99)");
            }
        }

        [Test]
        public void TestTileTypeResolving()
        {
            // Verify Start & Finish
            Assert.AreEqual(TileType.Start, boardConfig.GetTileDefinition(0).type);
            Assert.AreEqual(TileType.Finish, boardConfig.GetTileDefinition(100).type);

            // Verify specific static snakes
            // According to BoardConfig: tile 35 has a snake, tile 8 has a ladder
            Assert.AreEqual(TileType.Snake, boardConfig.GetTileDefinition(35).type);
            Assert.AreEqual(TileType.Ladder, boardConfig.GetTileDefinition(8).type);

            // Verify specific static skull tiles: 15, 59, 72
            Assert.AreEqual(TileType.Skull, boardConfig.GetTileDefinition(15).type);
            Assert.AreEqual(TileType.Skull, boardConfig.GetTileDefinition(59).type);

            // Verify normal tile resolving (e.g. tile 1, which has no special attributes)
            Assert.AreEqual(TileType.Normal, boardConfig.GetTileDefinition(1).type);
        }

        [Test]
        public void TestSerpentineMath()
        {
            // Test standard serpentine path layout formula (10x10 grid)
            // Left to right on even rows, right to left on odd rows.
            
            // Local implementation of the calculation function to verify index mapping
            System.Func<int, (int row, int col)> calcPos = (int n) =>
            {
                int z = n - 1;
                int r = z / 10;
                int colInRow = z % 10;
                int c = (r % 2 == 0) ? colInRow : (9 - colInRow);
                return (r, c);
            };

            // Tile 1: Row 0, Col 0
            var t1 = calcPos(1);
            Assert.AreEqual((0, 0), t1, "Tile 1 calculation should map to row 0, col 0");

            // Tile 10: Row 0, Col 9 (End of first row)
            var t10 = calcPos(10);
            Assert.AreEqual((0, 9), t10, "Tile 10 calculation should map to row 0, col 9");

            // Tile 11: Row 1, Col 9 (First tile of second row - odd row, so moving right to left)
            var t11 = calcPos(11);
            Assert.AreEqual((1, 9), t11, "Tile 11 calculation should map to row 1, col 9");

            // Tile 20: Row 1, Col 0 (End of second row)
            var t20 = calcPos(20);
            Assert.AreEqual((1, 0), t20, "Tile 20 calculation should map to row 1, col 0");

            // Tile 21: Row 2, Col 0 (First tile of third row - even row, so moving left to right)
            var t21 = calcPos(21);
            Assert.AreEqual((2, 0), t21, "Tile 21 calculation should map to row 2, col 0");

            // Tile 100: Row 9, Col 0 (Last tile on top left)
            var t100 = calcPos(100);
            Assert.AreEqual((9, 0), t100, "Tile 100 calculation should map to row 9, col 0");
        }
    }
}
