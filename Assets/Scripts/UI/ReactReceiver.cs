using UnityEngine;
using Dice;
using Quiz;
using Core;

namespace UI
{
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
    }
}
