using UnityEngine;

namespace Player
{
    /// <summary>
    /// Sprite data for one evolution stage: idle + 3 walk frames.
    /// </summary>
    [System.Serializable]
    public class StageSpriteData
    {
        public Sprite idle;
        [Tooltip("Walk frame 1 – right foot forward")]
        public Sprite walkRight;
        [Tooltip("Walk frame 2 – side profile / mid-stride")]
        public Sprite walkMid;
        [Tooltip("Walk frame 3 – left foot forward")]
        public Sprite walkLeft;
    }

    [CreateAssetMenu(fileName = "PlayerSpriteSet", menuName = "UlarTangga/PlayerSpriteSet")]
    public class PlayerSpriteSet : ScriptableObject
    {
        public Color identityColor;

        [Header("Evolution Stages (rows 1-5 of sprite sheet)")]
        public StageSpriteData stage1; // Petak 0-25: Punk
        public StageSpriteData stage2; // Petak 26-50: Mahasiswa Belajar
        public StageSpriteData stage3; // Petak 51-75: Mahasiswa Tertib
        public StageSpriteData stage4; // Petak 76-99: Mahasiswa Teladan
        public StageSpriteData stage5; // Petak 100: Duta IPB

        [Header("Expression Sprites (row 6 of sprite sheet)")]
        public Sprite happyFace;   // r6c1 – ladder reaction
        public Sprite shockedFace; // r6c2 – snake reaction

        /// <summary>
        /// Returns the StageSpriteData for a given evolution stage (1-5).
        /// </summary>
        public StageSpriteData GetStageData(int stage)
        {
            switch (stage)
            {
                case 1: return stage1;
                case 2: return stage2;
                case 3: return stage3;
                case 4: return stage4;
                case 5: return stage5;
                default: return stage1;
            }
        }

        /// <summary>
        /// Walk frame cycle: walkRight → walkMid → walkLeft → walkMid → ...
        /// </summary>
        public Sprite GetWalkFrame(int stage, int frameIndex)
        {
            var data = GetStageData(stage);
            if (data == null) return null;
            switch (frameIndex % 4)
            {
                case 0: return data.walkRight;
                case 1: return data.walkMid;
                case 2: return data.walkLeft;
                case 3: return data.walkMid;
                default: return data.idle;
            }
        }
    }
}
