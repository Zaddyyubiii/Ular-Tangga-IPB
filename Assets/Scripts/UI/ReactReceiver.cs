using UnityEngine;
using Dice;
using Quiz;
using Core;

namespace UI
{
    [System.Serializable]
    public class ReactStartGameData
    {
        public int playerCount;
        public string[] playerNames;
    }

    public class ReactReceiver : MonoBehaviour
    {
        public static ReactReceiver Instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            Debug.Log("[ReactReceiver] Auto-initializing programmatic ReactReceiver GameObject...");
            GameObject receiverGo = GameObject.Find("ReactReceiver");
            if (receiverGo == null)
            {
                receiverGo = new GameObject("ReactReceiver");
                receiverGo.AddComponent<ReactReceiver>();
                DontDestroyOnLoad(receiverGo);
                Debug.Log("[ReactReceiver] Successfully spawned permanent ReactReceiver GameObject programmatically!");
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                name = "ReactReceiver"; // Wajib persis agar SendMessage bisa menemukan objek ini
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        // Dipanggil dari React: window.unityInstance.SendMessage("ReactReceiver", "OnRollDice", power);
        public void OnRollDice(float power)
        {
            Debug.Log($"[ReactReceiver] Received OnRollDice from React with power: {power}");
            if (DiceGaugeController.Instance != null)
            {
                DiceGaugeController.Instance.TriggerRollFromReact(power);
            }
        }

        // Dipanggil dari React saat kuis dijawab
        public void OnAnswerQuiz(string answer)
        {
            Debug.Log($"[ReactReceiver] Received OnAnswerQuiz from React: {answer}");
            if (QuizPopup.Instance != null)
            {
                QuizPopup.Instance.SubmitAnswerFromReact(answer);
            }
        }

        // Dipanggil dari React saat menutup feedback kuis
        public void OnCloseQuizFeedback(string dummy)
        {
            Debug.Log("[ReactReceiver] Received OnCloseQuizFeedback from React");
            if (QuizPopup.Instance != null)
            {
                QuizPopup.Instance.CloseQuiz();
            }
        }

        // Dipanggil dari React saat memulai perjalanan (prologue selesai)
        public void OnStartJourney(string dummy)
        {
            Debug.Log("[ReactReceiver] Received OnStartJourney from React");
            if (PrologueUI.Instance != null)
            {
                PrologueUI.Instance.ClosePrologueFromReact();
            }
        }

        // Dipanggil dari React saat pemain mengklik tombol Lanjut pada Popup biasa (Kegiatan Positif, Ular Tangga)
        public void OnPopupClosedFromReact(string dummy)
        {
            Debug.Log("[ReactReceiver] Received OnPopupClosedFromReact from React");
            if (PopupController.Instance != null)
            {
                PopupController.Instance.ClosePopup();
            }
        }

        // Dipanggil dari React saat mengklik Play Again
        public void OnPlayAgain(string dummy)
        {
            Debug.Log("[ReactReceiver] Received OnPlayAgain from React");
            if (GameOverUI.Instance != null)
            {
                GameOverUI.Instance.RestartGame();
            }
        }

        // Dipanggil dari React saat kembali ke menu utama
        public void OnReturnToMenu(string dummy)
        {
            Debug.Log("[ReactReceiver] Received OnReturnToMenu from React");
            if (GameOverUI.Instance != null)
            {
                GameOverUI.Instance.ReturnToMainMenu();
            }
        }

        // Dipanggil dari React saat klik "Mulai Bermain" di MainMenu React
        public void OnStartGameFromReact(string jsonPayload)
        {
            Debug.Log($"[ReactReceiver] Received OnStartGameFromReact: {jsonPayload}");
            try
            {
                ReactStartGameData data = JsonUtility.FromJson<ReactStartGameData>(jsonPayload);
                
                GameSetup.HumanPlayerCount = data.playerCount;
                for (int i = 0; i < 4; i++)
                {
                    if (i < data.playerCount)
                    {
                        string trimmed = (data.playerNames != null && i < data.playerNames.Length && data.playerNames[i] != null) 
                            ? data.playerNames[i].Trim() : "";
                        GameSetup.PlayerNames[i] = string.IsNullOrEmpty(trimmed) ? $"Mahasiswa {i + 1}" : (trimmed.Length > 14 ? trimmed.Substring(0, 14) : trimmed);
                    }
                    else
                    {
                        GameSetup.PlayerNames[i] = $"Bot {i + 1}";
                    }
                }

                Core.GameManager.numRealPlayers = data.playerCount;
                PlayerPrefs.SetInt("NumRealPlayers", data.playerCount);
                for (int i = 0; i < 4; i++)
                {
                    PlayerPrefs.SetString($"PlayerName_{i}", GameSetup.PlayerNames[i]);
                }
                PlayerPrefs.Save();

                Core.SceneLoader.Instance.LoadScene("GameScene");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ReactReceiver] Failed to parse OnStartGameFromReact payload: {e.Message}");
            }
        }
    }
}
