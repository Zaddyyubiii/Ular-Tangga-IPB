using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI Controls")]
        public TMPro.TextMeshProUGUI labelPlayerCount;
        public Button btnDecreasePlayers;
        public Button btnIncreasePlayers;
        public Button btnStartGame;
        public Button btnQuitGame;

        private int playerCountSelected = 4; // Default to 4

        private void Awake()
        {
            RepairGeneratedLayout();
        }

        private void Start()
        {
            // Bind buttons
            if (btnDecreasePlayers != null) btnDecreasePlayers.onClick.AddListener(DecreasePlayers);
            if (btnIncreasePlayers != null) btnIncreasePlayers.onClick.AddListener(IncreasePlayers);
            if (btnStartGame != null) btnStartGame.onClick.AddListener(StartGame);
            if (btnQuitGame != null) btnQuitGame.onClick.AddListener(QuitGame);

            // Hide Quit button on WebGL
#if UNITY_WEBGL
            if (btnQuitGame != null) btnQuitGame.gameObject.SetActive(false);
#endif

            UpdatePlayerLabel();

            // Play background music (safe)
            if (Audio.AudioManager.Instance != null && Audio.AudioManager.Instance.mainMenuBGM != null)
            {
                Audio.AudioManager.Instance.PlayBGM(Audio.AudioManager.Instance.mainMenuBGM);
            }
        }

        private void DecreasePlayers()
        {
            PlayClickSound();
            if (playerCountSelected > 2)
            {
                playerCountSelected--;
                UpdatePlayerLabel();
            }
        }

        private void IncreasePlayers()
        {
            PlayClickSound();
            if (playerCountSelected < 4)
            {
                playerCountSelected++;
                UpdatePlayerLabel();
            }
        }

        private void UpdatePlayerLabel()
        {
            if (labelPlayerCount != null)
            {
                labelPlayerCount.text = $"Player: {playerCountSelected}";
            }
        }

        private void StartGame()
        {
            PlayClickSound();
            Debug.Log($"[MainMenu] Starting game with {playerCountSelected} real players");

            // Store selection globally
            Core.GameManager.numRealPlayers = playerCountSelected;

            // Load GameScene
            Core.SceneLoader.Instance.LoadScene("GameScene");
        }

        private void QuitGame()
        {
            PlayClickSound();
            Debug.Log("[MainMenu] Quitting Game...");
            Application.Quit();
        }

        private void PlayClickSound()
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.clickClip);
            }
        }

        private void RepairGeneratedLayout()
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null) return;

            foreach (TMPro.TextMeshProUGUI text in canvas.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
            {
                // Text should not steal pointer hits from nearby buttons.
                text.raycastTarget = false;
            }

            RectTransform title = canvas.transform.Find("Title") as RectTransform;
            if (title != null)
            {
                title.anchoredPosition = new Vector2(0f, 95f);
                title.sizeDelta = new Vector2(620f, 105f);

                TMPro.TextMeshProUGUI titleText = title.GetComponent<TMPro.TextMeshProUGUI>();
                if (titleText != null)
                {
                    titleText.fontSize = 38f;
                    titleText.enableWordWrapping = false;
                }
            }

            RectTransform dadu = canvas.transform.Find("DaduDecoration") as RectTransform;
            if (dadu != null)
            {
                dadu.anchoredPosition = new Vector2(0f, 215f);
                dadu.sizeDelta = new Vector2(260f, 80f);

                TMPro.TextMeshProUGUI daduText = dadu.GetComponent<TMPro.TextMeshProUGUI>();
                if (daduText != null)
                {
                    daduText.text = "DADU";
                    daduText.fontSize = 42f;
                    daduText.enableWordWrapping = false;
                }
            }

            MoveRect(canvas.transform.Find("ButtonStart") as RectTransform, new Vector2(0f, -25f), new Vector2(180f, 48f));
            MoveRect(canvas.transform.Find("PlayerSelectRow") as RectTransform, new Vector2(0f, -92f), new Vector2(300f, 44f));
            MoveRect(canvas.transform.Find("ButtonQuit") as RectTransform, new Vector2(0f, -160f), new Vector2(160f, 40f));
        }

        private void MoveRect(RectTransform rect, Vector2 position, Vector2 size)
        {
            if (rect == null) return;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }
    }
}
