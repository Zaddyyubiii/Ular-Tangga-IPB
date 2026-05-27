using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameOverUI : MonoBehaviour
    {
        public static GameOverUI Instance;

        [Header("UI Component Bindings")]
        public GameObject gameOverPanel;
        public TMPro.TextMeshProUGUI labelWinnerName;
        public Image imageWinnerAvatar;
        public TMPro.TextMeshProUGUI labelMessage;
        public Button btnPlayAgain;
        public Button btnReturnToMenu;

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

            // Play victory tune (safe check)
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.winClip);
            }

            gameOverPanel.SetActive(true);
            Debug.Log($"[GameOver] Declared winner: {winner.playerName}");
        }

        private void RestartGame()
        {
            PlayClickSound();
            Core.SceneLoader.Instance.LoadScene("GameScene");
        }

        private void ReturnToMainMenu()
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
