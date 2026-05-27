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

        public void Refresh(PlayerData data, bool isActivePlayer)
        {
            if (data == null) return;

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

            // Highlight border if active turn
            if (imageHighlightBorder != null)
            {
                imageHighlightBorder.gameObject.SetActive(isActivePlayer && !data.isFinished && !data.isDroppedOut);
            }

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
            else if (isActivePlayer)
            {
                statusStr = "GILIRAN AKTIF";
                statusColor = new Color(0.12f, 0.73f, 0.35f);
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

            // Background color of avatar representing player color
            if (imageAvatarBackground != null)
            {
                imageAvatarBackground.color = data.playerColor;
            }
        }
    }
}
