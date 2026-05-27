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
                if (isActive)
                {
                    imageAvatarBackground.color = GetHighContrastColor(color);
                }
                else
                {
                    imageAvatarBackground.color = GetNormalCardColor(color);
                }
            }

            // Ensure text color is highly readable against the card background color
            Color txtColor = GetReadableTextColor(imageAvatarBackground != null ? imageAvatarBackground.color : color);
            if (labelName != null) labelName.color = txtColor;
            if (labelTile != null) labelTile.color = txtColor;
            if (labelEvolution != null) labelEvolution.color = txtColor;
            if (labelSkulls != null) labelSkulls.color = txtColor;
        }

        private Color GetNormalCardColor(Color baseColor)
        {
            // Soft background (darkened / soft version of player color)
            return new Color(baseColor.r * 0.25f + 0.1f, baseColor.g * 0.25f + 0.1f, baseColor.b * 0.25f + 0.1f, 0.9f);
        }

        private Color GetHighContrastColor(Color baseColor)
        {
            float luminance = 
                0.299f * baseColor.r +
                0.587f * baseColor.g +
                0.114f * baseColor.b;

            // If color is relatively bright, make it much darker; if dark, make it brighter
            if (luminance > 0.6f)
            {
                return Color.Lerp(baseColor, Color.black, 0.35f);
            }
            else
            {
                return Color.Lerp(baseColor, Color.white, 0.25f);
            }
        }

        private Color GetReadableTextColor(Color backgroundColor)
        {
            float luminance = 
                0.299f * backgroundColor.r +
                0.587f * backgroundColor.g +
                0.114f * backgroundColor.b;

            return luminance > 0.6f ? Color.black : Color.white;
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

            // Skull hits
            if (labelSkulls != null)
            {
                labelSkulls.text = $"Sanksi: {data.skullHitCount}/3";
            }

            bool isCurrentlyActive = isActivePlayer && !data.isFinished && !data.isDroppedOut;

            // Highlight border if active turn (using White for contrast outline instead of universal green)
            if (imageHighlightBorder != null)
            {
                imageHighlightBorder.gameObject.SetActive(isCurrentlyActive);
                if (isCurrentlyActive)
                {
                    imageHighlightBorder.color = Color.white;
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
                statusStr = "GILIRAN AKTIF";
                statusColor = Color.white;
            }
            else if (data.skipTurns > 0)
            {
                statusStr = $"DITANGGUHKAN ({data.skipTurns})";
                statusColor = Color.orange;
            }

            if (labelStatus != null)
            {
                labelStatus.text = statusStr;
                labelStatus.color = statusColor;
            }

            // Apply card background and text colors
            ApplyPlayerColor(data.playerColor, isCurrentlyActive);
        }
    }
}
