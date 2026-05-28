using UnityEngine;
using UnityEngine.UI;
using Player;

namespace UI
{
    public class PlayerStatusView : MonoBehaviour
    {
        [Header("UI Component Bindings")]
        public TMPro.TextMeshProUGUI labelName;
        public TMPro.TextMeshProUGUI labelTile;
        public TMPro.TextMeshProUGUI labelEvolution;
        public TMPro.TextMeshProUGUI labelSkulls;
        public TMPro.TextMeshProUGUI labelStatus;
        public Image imageAvatarBackground;
        public Image imageHighlightBorder;

        private PlayerData playerData;
        private Outline cardOutline;

        private void EnsureOutlineComponent()
        {
            if (cardOutline == null && imageAvatarBackground != null)
            {
                cardOutline = imageAvatarBackground.GetComponent<Outline>();
                if (cardOutline == null)
                {
                    cardOutline = imageAvatarBackground.gameObject.AddComponent<Outline>();
                }
                cardOutline.effectDistance = new Vector2(3f, 3f);
                cardOutline.useGraphicAlpha = true;
            }
        }

        public void Bind(PlayerData player)
        {
            Bind(player, false);
        }

        public void Bind(PlayerData player, bool isActivePlayer)
        {
            this.playerData = player;
            Debug.Log($"Status card bound to Player {player.id} with color: {player.playerColor}");
            ApplyPlayerColor(player.playerColor, isActivePlayer);
            Refresh(player, isActivePlayer);
        }

        private void ApplyPlayerColor(Color color, bool isActive)
        {
            if (imageAvatarBackground != null)
            {
                imageAvatarBackground.material = null; // Remove any material override
                if (isActive)
                {
                    imageAvatarBackground.color = GetActiveCardColor(color);
                }
                else
                {
                    imageAvatarBackground.color = GetInactiveCardColor(color);
                }
            }

            // Ensure text color is highly readable against the card background color
            Color txtColor = GetReadableTextColor(imageAvatarBackground != null ? imageAvatarBackground.color : color);
            if (labelName != null) labelName.color = txtColor;
            if (labelTile != null) labelTile.color = txtColor;
            if (labelEvolution != null) labelEvolution.color = txtColor;
            if (labelSkulls != null) labelSkulls.color = txtColor;
            if (labelStatus != null) labelStatus.color = txtColor;
        }

        private Color GetInactiveCardColor(Color baseColor)
        {
            // Transparent glassmorphic inactive card color
            return new Color(baseColor.r, baseColor.g, baseColor.b, 0.32f);
        }

        private Color GetActiveCardColor(Color baseColor)
        {
            // Translucent glowing active card color
            return new Color(baseColor.r, baseColor.g, baseColor.b, 0.78f);
        }

        private Color GetReadableTextColor(Color backgroundColor)
        {
            // Pure white text looks best on translucent glassmorphism
            return Color.white;
        }

        public void Refresh(PlayerData data, bool isActivePlayer)
        {
            if (data == null) return;
            this.playerData = data;

            // Name & Bot Tag
            string botTag = data.isBot ? " (Bot)" : " (Human)";
            if (labelName != null)
            {
                labelName.text = data.playerName + botTag;
            }

            // Tile position
            if (labelTile != null)
            {
                if (data.isFinished)
                {
                    string rankStr = data.finishRank <= 3 ? $"Juara {data.finishRank}" : $"Posisi {data.finishRank}";
                    labelTile.text = $"Tile: 100/100 ({rankStr})";
                }
                else
                {
                    labelTile.text = $"Tile: {data.currentTile}";
                }
            }

            // Evolution Label
            if (labelEvolution != null)
            {
                string evoLabel = "Stage 1";
                if (PlayerEvolutionController.Instance != null)
                {
                    evoLabel = PlayerEvolutionController.Instance.GetStageLabel(data.currentEvolutionStage);
                }
                else
                {
                    evoLabel = $"Stage {data.currentEvolutionStage}";
                }
                labelEvolution.text = evoLabel;
            }

            // Skull hits with visual rich indicators
            if (labelSkulls != null)
            {
                string skullsVisual = "";
                for (int s = 0; s < data.skullHitCount; s++) skullsVisual += "☠️";
                if (data.skullHitCount == 0) skullsVisual = "Aman";
                labelSkulls.text = $"Sanksi: {data.skullHitCount}/3 ({skullsVisual})";
            }

            bool isCurrentlyActive = isActivePlayer && !data.isFinished && !data.isDroppedOut;

            // Highlight border if active turn (using Outline component if child covers card to prevent solid white blocking background)
            if (imageHighlightBorder != null)
            {
                // If highlight border is configured as a child of the root card or background (old layout),
                // use the dynamic Outline component to avoid solid color overlay completely masking the background.
                if (imageHighlightBorder.transform.parent == transform || imageHighlightBorder.transform.parent == imageAvatarBackground.transform)
                {
                    EnsureOutlineComponent();
                    if (cardOutline != null)
                    {
                        cardOutline.enabled = isCurrentlyActive;
                        cardOutline.effectColor = Color.white;
                    }
                    imageHighlightBorder.gameObject.SetActive(false);
                }
                else
                {
                    // Sibling order layout: safe to use border directly as it is behind background
                    imageHighlightBorder.gameObject.SetActive(isCurrentlyActive);
                    if (isCurrentlyActive)
                    {
                        imageHighlightBorder.color = Color.white;
                    }
                }
            }
            else
            {
                // Fallback to Outline if border image reference is null
                EnsureOutlineComponent();
                if (cardOutline != null)
                {
                    cardOutline.enabled = isCurrentlyActive;
                    cardOutline.effectColor = Color.white;
                }
            }

            // Apply card scale (1.03x) when active
            transform.localScale = isCurrentlyActive 
                ? Vector3.one * 1.03f 
                : Vector3.one;

            // Status details
            string statusStr = "Menunggu";
            Color statusColor = Color.white;

            if (data.isDroppedOut)
            {
                statusStr = "DROP OUT";
                statusColor = Color.red;
            }
            else if (data.isFinished)
            {
                statusStr = "FINISH";
                statusColor = new Color(0.85f, 0.65f, 0.12f);
            }
            else if (data.isWinner)
            {
                statusStr = "MENANG";
                statusColor = new Color(0.85f, 0.65f, 0.12f);
            }
            else if (isCurrentlyActive)
            {
                statusStr = "SEDANG BERJALAN";
                statusColor = Color.white;
            }
            else if (data.skipTurns > 0)
            {
                statusStr = $"DISKORS ({data.skipTurns})";
                statusColor = Color.orange;
            }

            if (labelStatus != null)
            {
                labelStatus.text = statusStr;
                labelStatus.color = statusColor;
            }

            // Apply card background and text colors
            ApplyPlayerColor(data.playerColor, isCurrentlyActive);

            // Log active state change (Section 11)
            if (imageAvatarBackground != null)
            {
                Debug.Log($"Player {data.playerName} status card active={isCurrentlyActive}, color={imageAvatarBackground.color}");
            }
        }
    }
}
