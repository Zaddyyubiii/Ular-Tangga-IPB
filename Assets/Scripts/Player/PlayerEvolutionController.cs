using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerEvolutionController : MonoBehaviour
    {
        public static PlayerEvolutionController Instance;

        [Header("Character Evolution Sets")]
        public List<PlayerSpriteSet> characterSets = new List<PlayerSpriteSet>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void EvaluateEvolution(PlayerData data)
        {
            if (data == null) return;

            int prevStage = data.currentEvolutionStage;

            // Determine stage based on tile threshold
            if (data.currentTile == 100)
            {
                data.currentEvolutionStage = 5;
            }
            else if (data.currentTile >= 76)
            {
                data.currentEvolutionStage = 4;
            }
            else if (data.currentTile >= 51)
            {
                data.currentEvolutionStage = 3;
            }
            else if (data.currentTile >= 26)
            {
                data.currentEvolutionStage = 2;
            }
            else
            {
                data.currentEvolutionStage = 1;
            }

            // Assign sprite based on selected character set
            if (characterSets != null && characterSets.Count > 0)
            {
                // Each player id gets a dedicated set (wrap around if needed)
                int setIndex = (data.id - 1) % characterSets.Count;
                PlayerSpriteSet spriteSet = characterSets[setIndex];

                if (spriteSet != null)
                {
                    switch (data.currentEvolutionStage)
                    {
                        case 1: data.currentSprite = spriteSet.stage1; break;
                        case 2: data.currentSprite = spriteSet.stage2; break;
                        case 3: data.currentSprite = spriteSet.stage3; break;
                        case 4: data.currentSprite = spriteSet.stage4; break;
                        case 5: data.currentSprite = spriteSet.winnerSprite; break;
                    }
                }
            }

            if (prevStage != data.currentEvolutionStage)
            {
                Debug.Log($"[Evolution] Player {data.playerName} evolved from Stage {prevStage} to Stage {data.currentEvolutionStage}!");
            }
        }

        public string GetStageLabel(int stage)
        {
            switch (stage)
            {
                case 1: return "Stage 1: Punk Bermasalah";
                case 2: return "Stage 2: Mahasiswa Belajar";
                case 3: return "Stage 3: Mahasiswa Tertib";
                case 4: return "Stage 4: Mahasiswa Teladan";
                case 5: return "Stage 5: Duta IPB University";
                default: return "Mahasiswa";
            }
        }
    }
}
