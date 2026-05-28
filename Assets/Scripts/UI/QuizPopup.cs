using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Quiz;

namespace UI
{
    [System.Serializable]
    public class ReactQuizState
    {
        public string questionText;
        public string[] choices;
        public int correctAnswerIndex;
        public string correctFeedback;
        public string incorrectFeedback;
    }

    public class QuizPopup : MonoBehaviour
    {
        public static QuizPopup Instance;

        #if UNITY_WEBGL && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void ShowQuizToReact(string quizJson);
        #else
        private static void ShowQuizToReact(string quizJson) { }
        #endif

        [Header("UI Component Bindings")]
        public GameObject quizPanel;
        public TMPro.TextMeshProUGUI labelQuestion;
        public Button btnOptionA; // Representing True / Option A
        public Button btnOptionB; // Representing False / Option B
        
        [Header("Feedback Panel")]
        public GameObject feedbackContainer;
        public TMPro.TextMeshProUGUI labelFeedbackResult; // "Benar!" or "Kurang tepat."
        public TMPro.TextMeshProUGUI labelFeedbackExplanations;
        public Button btnCloseQuiz;

        private QuizQuestion currentQuestion;
        private Action onQuizFinishedCallback;
        private Coroutine autoCloseCoroutine;
        private bool isQuizOpen;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (quizPanel != null) quizPanel.SetActive(false);

            if (btnOptionA != null) btnOptionA.onClick.AddListener(() => OnOptionSelected(0));
            if (btnOptionB != null) btnOptionB.onClick.AddListener(() => OnOptionSelected(1));
            if (btnCloseQuiz != null) btnCloseQuiz.onClick.AddListener(OnCloseQuizClicked);
        }

        public void ShowQuiz(QuizQuestion question, Action callback)
        {
            // Pause turn timer
            if (Turn.TurnManager.Instance != null)
            {
                Turn.TurnManager.Instance.StopTimer();
            }

            currentQuestion = question;
            onQuizFinishedCallback = callback;
            isQuizOpen = true;

            // Notify React of the Quiz
            ReactQuizState rQuiz = new ReactQuizState();
            rQuiz.questionText = question.questionText;
            rQuiz.choices = question.choices;
            rQuiz.correctAnswerIndex = question.correctAnswerIndex;
            rQuiz.correctFeedback = question.correctFeedback;
            rQuiz.incorrectFeedback = question.incorrectFeedback;
            ShowQuizToReact(JsonUtility.ToJson(rQuiz));

            if (autoCloseCoroutine != null)
            {
                StopCoroutine(autoCloseCoroutine);
                autoCloseCoroutine = null;
            }

            labelQuestion.text = question.questionText;

            // Setup button labels
            if (btnOptionA != null && btnOptionA.GetComponentInChildren<TMPro.TextMeshProUGUI>() != null)
            {
                btnOptionA.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = question.choices[0];
            }
            if (btnOptionB != null && btnOptionB.GetComponentInChildren<TMPro.TextMeshProUGUI>() != null)
            {
                btnOptionB.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = question.choices[1];
            }

            // Reset buttons
            btnOptionA.gameObject.SetActive(true);
            btnOptionB.gameObject.SetActive(true);
            btnOptionA.interactable = true;
            btnOptionB.interactable = true;

            // Hide feedback originally
            if (feedbackContainer != null) feedbackContainer.SetActive(false);

            // Animate main quiz panel entry
            #if UNITY_WEBGL && !UNITY_EDITOR
            // React handles the display
            #else
            quizPanel.SetActive(true);
            quizPanel.transform.localScale = Vector3.zero;
            StartCoroutine(PopScaleCo(quizPanel.transform, Vector3.one, 0.25f));
            #endif

            // Trigger bot auto-solve if current player is a bot
            var curPlayer = Core.GameManager.Instance != null ? Core.GameManager.Instance.GetCurrentPlayer() : null;
            if (curPlayer != null && curPlayer.isBot)
            {
                StartCoroutine(ExecuteBotQuizTurnCo());
            }
        }

        private void OnOptionSelected(int selectedIndex)
        {
            // Lock buttons
            btnOptionA.interactable = false;
            btnOptionB.interactable = false;

            bool isCorrect = (selectedIndex == currentQuestion.correctAnswerIndex);

            // Display feedback with smooth animation
            #if UNITY_WEBGL && !UNITY_EDITOR
            // React handles feedback panel
            #else
            if (feedbackContainer != null)
            {
                feedbackContainer.SetActive(true);
                feedbackContainer.transform.localScale = Vector3.zero;
                StartCoroutine(PopScaleCo(feedbackContainer.transform, Vector3.one, 0.2f));
            }
            #endif

            if (isCorrect)
            {
                labelFeedbackResult.text = "BENAR! *";
                labelFeedbackResult.color = new Color(0.12f, 0.73f, 0.35f); // Beautiful green
                labelFeedbackExplanations.text = currentQuestion.correctFeedback;

                if (Audio.AudioManager.Instance != null)
                {
                    Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.quizCorrectClip);
                }
            }
            else
            {
                labelFeedbackResult.text = "KURANG TEPAT. *";
                labelFeedbackResult.color = Color.red;
                labelFeedbackExplanations.text = currentQuestion.incorrectFeedback;

                if (Audio.AudioManager.Instance != null)
                {
                    Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.quizWrongClip);
                }
            }

            Debug.Log($"[Quiz] Answered correct: {isCorrect}");

            // Start auto close timer for human players
            var curPlayer = Core.GameManager.Instance != null ? Core.GameManager.Instance.GetCurrentPlayer() : null;
            bool isBot = curPlayer != null && curPlayer.isBot;
            if (!isBot)
            {
                if (autoCloseCoroutine != null) StopCoroutine(autoCloseCoroutine);
                autoCloseCoroutine = StartCoroutine(AutoCloseQuizCo(PopupController.POPUP_AUTO_CLOSE_DELAY));
            }
        }

        private IEnumerator AutoCloseQuizCo(float delay)
        {
            Debug.Log($"Quiz feedback opened. Auto close in {delay} seconds.");
            yield return new WaitForSeconds(delay);
            Debug.Log("Quiz feedback auto closed after 5 seconds.");
            CloseQuiz();
        }

        private void OnCloseQuizClicked()
        {
            Debug.Log("Popup closed manually before auto close.");
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.clickClip);
            }
            CloseQuiz();
        }

        public void CloseQuiz()
        {
            if (!isQuizOpen) return;
            isQuizOpen = false;

            if (autoCloseCoroutine != null)
            {
                StopCoroutine(autoCloseCoroutine);
                autoCloseCoroutine = null;
            }

            if (quizPanel != null) quizPanel.SetActive(false);

            Action callback = onQuizFinishedCallback;
            onQuizFinishedCallback = null;

            Debug.Log("Popup onClose callback executed.");
            callback?.Invoke();
        }

        public void SubmitAnswerFromReact(string answer)
        {
            int selectedIndex = 0;
            if (answer.ToUpper() == "B" || answer == "1") selectedIndex = 1;
            OnOptionSelected(selectedIndex);
        }

        private IEnumerator PopScaleCo(Transform trans, Vector3 targetScale, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                trans.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsed / duration);
                yield return null;
            }
            trans.localScale = targetScale;
        }

        private IEnumerator ExecuteBotQuizTurnCo()
        {
            // Disable buttons for bot turn so the human player cannot click them
            if (btnOptionA != null) btnOptionA.interactable = false;
            if (btnOptionB != null) btnOptionB.interactable = false;
            if (btnCloseQuiz != null) btnCloseQuiz.interactable = false;

            var curPlayer = Core.GameManager.Instance != null ? Core.GameManager.Instance.GetCurrentPlayer() : null;
            string botName = curPlayer != null ? curPlayer.playerName : "Bot";

            // Wait for dramatic effect (bot "thinking" / player reading the question)
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 1.0f));

            // Choose an answer: bot has a 75% chance of answering correctly
            bool answerCorrectly = UnityEngine.Random.value < 0.75f;
            int chosenIndex = currentQuestion.correctAnswerIndex;
            if (!answerCorrectly)
            {
                chosenIndex = 1 - currentQuestion.correctAnswerIndex;
            }

            // Trigger visual feedback and SFX via option selection
            string choiceChar = chosenIndex == 0 ? "A" : "B";
            Debug.Log($"Bot Player {botName} quiz answered: {choiceChar}.");
            OnOptionSelected(chosenIndex);

            // Wait for feedback to be displayed for exactly 5 seconds
            yield return new WaitForSeconds(PopupController.POPUP_AUTO_CLOSE_DELAY);

            // Enable close button and close the quiz automatically
            if (btnCloseQuiz != null) btnCloseQuiz.interactable = true;
            Debug.Log($"Bot Player {botName} quiz feedback auto closed.");
            CloseQuiz();
        }
    }
}
