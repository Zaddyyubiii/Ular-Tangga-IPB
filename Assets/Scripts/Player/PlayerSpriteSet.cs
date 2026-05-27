using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerSpriteSet", menuName = "UlarTangga/PlayerSpriteSet")]
    public class PlayerSpriteSet : ScriptableObject
    {
        public Color identityColor;
        public Sprite stage1; // Petak 0-25: Punk, messy hair
        public Sprite stage2; // Petak 26-50: Neater hair, messy clothes
        public Sprite stage3; // Petak 51-75: Neat clothes but wearing T-shirt
        public Sprite stage4; // Petak 76-99: Full formal shirt
        public Sprite winnerSprite; // Petak 100: Duta IPB
    }
}
