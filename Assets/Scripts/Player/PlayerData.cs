using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class PlayerData
    {
        public int id;
        public string playerName;
        public bool isBot;
        public int currentTile; // 0 = start, 1-100 = grid
        public int skullHitCount;
        public int skipTurns;
        public bool isDroppedOut;
        public bool isWinner;
        public int currentEvolutionStage = 1; // 1 to 5
        public Sprite currentSprite;
        public Color playerColor;
    }
}
