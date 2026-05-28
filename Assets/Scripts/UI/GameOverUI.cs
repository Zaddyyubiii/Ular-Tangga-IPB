using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [System.Serializable]
    public class ReactGameOverState
    {
        public string winnerName;
        public string winnerColorHex;
        public string messageText;
    }

    public class GameOverUI : MonoBehaviour
    {
        public static GameOverUI Instance;

        #if UNITY_WEBGL && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void ShowGameOverToReact(string gameOverJson);
        #else
        private static void ShowGameOverToReact(string gameOverJson) { }
        #endif

        [Header("UI Component Bindings")]
        public GameObject gameOverPanel;
        public TMPro.TextMeshProUGUI labelWinnerName;
        public Image imageWinnerAvatar;
        public TMPro.TextMeshProUGUI labelMessage;
        public Button btnPlayAgain;
        public Button btnReturnToMenu;

        [Header("Ranking Results (Optional)")]
        public TMPro.TextMeshProUGUI labelJuara1;
        public TMPro.TextMeshProUGUI labelJuara2;
        public TMPro.TextMeshProUGUI labelJuara3;
        public TMPro.TextMeshProUGUI labelPosisi4;
        public TMPro.TextMeshProUGUI labelDropOuts;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(false);

            if (btnPlayAgain != null) btnPlayAgain.onClick.AddListener(RestartGame);
            if (btnReturnToMenu != null) btnReturnToMenu.onClick.AddListener(ReturnToMainMenu);
        }

        public void ShowWinner(Player.PlayerData winner)
        {
            if (winner == null) return;

            // Pause turn timer
            if (Turn.TurnManager.Instance != null)
            {
                Turn.TurnManager.Instance.StopTimer();
            }

            if (labelWinnerName != null)
            {
                labelWinnerName.text = $"{winner.playerName.ToUpper()} MENANG!";
                labelWinnerName.color = winner.playerColor;
            }

            if (imageWinnerAvatar != null)
            {
                if (winner.currentSprite != null)
                {
                    imageWinnerAvatar.sprite = winner.currentSprite;
                    imageWinnerAvatar.color = Color.white;
                }
                else
                {
                    // Fallback visual
                    imageWinnerAvatar.sprite = null;
                    imageWinnerAvatar.color = winner.playerColor;
                }
            }

            if (labelMessage != null)
            {
                labelMessage.text = "Selamat! Kamu berhasil melewati semua tantangan, mematuhi tata tertib, dan dinobatkan menjadi Duta Tata Tertib IPB University!";
            }

            ReactGameOverState rGameOver = new ReactGameOverState();
            rGameOver.winnerName = winner.playerName;
            rGameOver.winnerColorHex = "#" + ColorUtility.ToHtmlStringRGB(winner.playerColor);
            rGameOver.messageText = labelMessage != null ? labelMessage.text : "Selamat! Kamu berhasil melewati semua tantangan, mematuhi tata tertib, dan dinobatkan menjadi Duta Tata Tertib IPB University!";
            ShowGameOverToReact(JsonUtility.ToJson(rGameOver));

            // Play victory tune (safe check)
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.winClip);
            }

            #if UNITY_WEBGL && !UNITY_EDITOR
            // React handles winner gameover screen overlay
            #else
            gameOverPanel.SetActive(true);
            #endif
            Debug.Log($"[GameOver] Declared winner: {winner.playerName}");
        }

        public void ShowFinalRanking(System.Collections.Generic.List<Player.PlayerData> players, System.Collections.Generic.List<Player.PlayerData> finishOrder)
        {
            // Pause turn timer
            if (Turn.TurnManager.Instance != null)
            {
                Turn.TurnManager.Instance.StopTimer();
            }

            // Play victory tune
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.winClip);
            }

            if (labelWinnerName != null)
            {
                labelWinnerName.text = "Hasil Akhir Permainan";
            }

            // Extract players from finishOrder
            Player.PlayerData juara1 = finishOrder.Count > 0 ? finishOrder[0] : null;
            Player.PlayerData juara2 = finishOrder.Count > 1 ? finishOrder[1] : null;
            Player.PlayerData juara3 = finishOrder.Count > 2 ? finishOrder[2] : null;
            
            Player.PlayerData posisi4 = null;
            if (finishOrder.Count > 3)
            {
                posisi4 = finishOrder[3];
            }
            else
            {
                foreach (var p in players)
                {
                    if (!p.isFinished && !p.isDroppedOut)
                    {
                        posisi4 = p;
                        break;
                    }
                }
            }

            // Gather dropped out players
            System.Collections.Generic.List<string> dropOuts = new System.Collections.Generic.List<string>();
            foreach (var p in players)
            {
                if (p.isDroppedOut)
                {
                    dropOuts.Add(p.playerName);
                }
            }

            // Build large consolidated string inside labelMessage as a robust fallback
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (juara1 != null) sb.AppendLine($"🥇 Juara 1: {juara1.playerName}");
            else sb.AppendLine($"🥇 Juara 1: -");
            
            if (juara2 != null) sb.AppendLine($"🥈 Juara 2: {juara2.playerName}");
            else sb.AppendLine($"🥈 Juara 2: -");

            if (juara3 != null) sb.AppendLine($"🥉 Juara 3: {juara3.playerName}");
            else sb.AppendLine($"🥉 Juara 3: -");

            if (posisi4 != null)
            {
                sb.AppendLine();
                if (posisi4.isFinished)
                {
                    sb.AppendLine($"Posisi 4: {posisi4.playerName}");
                }
                else
                {
                    sb.AppendLine($"Belum Finish: {posisi4.playerName}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("Drop Out:");
            if (dropOuts.Count > 0)
            {
                foreach (var dName in dropOuts)
                {
                    sb.AppendLine($"- {dName}");
                }
            }
            else
            {
                sb.AppendLine("- Tidak ada");
            }

            if (labelMessage != null)
            {
                labelMessage.text = sb.ToString();
                labelMessage.alignment = TMPro.TextAlignmentOptions.Center;
            }

            // Set optional granular ranking labels if bound in the inspector
            if (labelJuara1 != null)
            {
                labelJuara1.gameObject.SetActive(true);
                labelJuara1.text = juara1 != null ? $"🥇 Juara 1: {juara1.playerName}" : "🥇 Juara 1: -";
            }
            if (labelJuara2 != null)
            {
                labelJuara2.gameObject.SetActive(true);
                labelJuara2.text = juara2 != null ? $"🥈 Juara 2: {juara2.playerName}" : "🥈 Juara 2: -";
            }
            if (labelJuara3 != null)
            {
                labelJuara3.gameObject.SetActive(true);
                labelJuara3.text = juara3 != null ? $"🥉 Juara 3: {juara3.playerName}" : "🥉 Juara 3: -";
            }
            if (labelPosisi4 != null)
            {
                labelPosisi4.gameObject.SetActive(posisi4 != null);
                if (posisi4 != null)
                {
                    labelPosisi4.text = posisi4.isFinished 
                        ? $"Posisi 4: {posisi4.playerName}" 
                        : $"Belum Finish: {posisi4.playerName}";
                }
            }
            if (labelDropOuts != null)
            {
                labelDropOuts.gameObject.SetActive(true);
                labelDropOuts.text = dropOuts.Count > 0 
                    ? $"Drop Out:\n- {string.Join("\n- ", dropOuts)}" 
                    : "Drop Out: Tidak ada";
            }

            // Avatar displays Juara 1
            if (imageWinnerAvatar != null && juara1 != null)
            {
                if (juara1.currentSprite != null)
                {
                    imageWinnerAvatar.sprite = juara1.currentSprite;
                    imageWinnerAvatar.color = Color.white;
                }
                else
                {
                    imageWinnerAvatar.sprite = null;
                    imageWinnerAvatar.color = juara1.playerColor;
                }
            }

            gameOverPanel.SetActive(true);
            Debug.Log("Final ranking displayed.");
        }

        public void RestartGame()
        {
            PlayClickSound();
            Core.SceneLoader.Instance.LoadScene("GameScene");
        }

        public void ReturnToMainMenu()
        {
            PlayClickSound();
            Core.SceneLoader.Instance.LoadScene("MainMenuScene");
        }

        private void PlayClickSound()
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.clickClip);
            }
        }
    }
}
