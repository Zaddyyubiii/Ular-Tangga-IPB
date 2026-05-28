using NUnit.Framework;
using UnityEngine;
using Player;

namespace Editor.Tests
{
    public class PlayerEvolutionTests
    {
        private GameObject testGo;
        private PlayerEvolutionController evolutionController;

        [SetUp]
        public void SetUp()
        {
            testGo = new GameObject("TestEvolutionController");
            evolutionController = testGo.AddComponent<PlayerEvolutionController>();
        }

        [TearDown]
        public void TearDown()
        {
            if (testGo != null)
            {
                Object.DestroyImmediate(testGo);
            }
        }

        [Test]
        public void TestEvolutionStages()
        {
            PlayerData data = new PlayerData();
            data.id = 1;
            data.playerName = "TestPlayer";

            // Tile 0 to 25 -> Stage 1
            data.currentTile = 0;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(1, data.currentEvolutionStage, "Tile 0 should trigger Evolution Stage 1");

            data.currentTile = 25;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(1, data.currentEvolutionStage, "Tile 25 should trigger Evolution Stage 1");

            // Tile 26 to 50 -> Stage 2
            data.currentTile = 26;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(2, data.currentEvolutionStage, "Tile 26 should trigger Evolution Stage 2");

            data.currentTile = 50;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(2, data.currentEvolutionStage, "Tile 50 should trigger Evolution Stage 2");

            // Tile 51 to 75 -> Stage 3
            data.currentTile = 51;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(3, data.currentEvolutionStage, "Tile 51 should trigger Evolution Stage 3");

            data.currentTile = 75;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(3, data.currentEvolutionStage, "Tile 75 should trigger Evolution Stage 3");

            // Tile 76 to 99 -> Stage 4
            data.currentTile = 76;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(4, data.currentEvolutionStage, "Tile 76 should trigger Evolution Stage 4");

            data.currentTile = 99;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(4, data.currentEvolutionStage, "Tile 99 should trigger Evolution Stage 4");

            // Tile 100 -> Stage 5
            data.currentTile = 100;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(5, data.currentEvolutionStage, "Tile 100 should trigger Evolution Stage 5");
        }

        [Test]
        public void TestSpriteSetSelection()
        {
            // Create mock sprite sets and test if they get correctly assigned
            PlayerSpriteSet set = ScriptableObject.CreateInstance<PlayerSpriteSet>();
            
            // Create dummy sprites
            Sprite s1 = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
            Sprite s2 = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
            Sprite s3 = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
            Sprite s4 = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
            Sprite sWinner = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);

            set.stage1 = s1;
            set.stage2 = s2;
            set.stage3 = s3;
            set.stage4 = s4;
            set.winnerSprite = sWinner;

            evolutionController.characterSets.Add(set);

            PlayerData data = new PlayerData();
            data.id = 1;

            data.currentTile = 10;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(s1, data.currentSprite, "Stage 1 sprite should match stage1 field");

            data.currentTile = 35;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(s2, data.currentSprite, "Stage 2 sprite should match stage2 field");

            data.currentTile = 60;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(s3, data.currentSprite, "Stage 3 sprite should match stage3 field");

            data.currentTile = 85;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(s4, data.currentSprite, "Stage 4 sprite should match stage4 field");

            data.currentTile = 100;
            evolutionController.EvaluateEvolution(data);
            Assert.AreEqual(sWinner, data.currentSprite, "Stage 5 sprite should match winnerSprite field");
            
            // Clean up resources
            Object.DestroyImmediate(set);
        }
    }
}
