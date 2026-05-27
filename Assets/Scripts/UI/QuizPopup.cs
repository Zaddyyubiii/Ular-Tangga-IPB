using System;
using UnityEngine;
using UnityEngine.UI;
using Quiz;

namespace UI
{
    public class QuizPopup : MonoBehaviour
    {
        public static QuizPopup Instance;

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

            quizPanel.SetActive(true);
        }

        private void OnOptionSelected(int selectedIndex)
        {
            // Lock buttons
            btnOptionA.interactable = false;
            btnOptionB.interactable = false;

            bool isCorrect = (selectedIndex == currentQuestion.correctAnswerIndex);

            // Display feedback
            if (feedbackContainer != null)
            {
                feedbackContainer.SetActive(true);
            }

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
        }

        private void OnCloseQuizClicked()
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.clickClip);
            }

            quizPanel.SetActive(false);
            onQuizFinishedCallback?.Invoke();
        }
    }
}
