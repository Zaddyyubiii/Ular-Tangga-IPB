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
            return Color.Lerp(baseColor, Color.black, 0.45f);
        }

        private Color GetActiveCardColor(Color baseColor)
        {
            Color brightColor = Color.Lerp(baseColor, Color.white, 0.18f);
            Color.RGBToHSV(brightColor, out float h, out float s, out float v);
            s = Mathf.Clamp01(s + 0.15f);
            v = Mathf.Clamp01(v + 0.20f);
            return Color.HSVToRGB(h, s, v);
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

            // Log active state change (Section 11)
            if (imageAvatarBackground != null)
            {
                Debug.Log($"Player {data.playerName} status card active={isCurrentlyActive}, color={imageAvatarBackground.color}");
            }
        }
    }
}
