using System;

namespace Quiz
{
    [Serializable]
    public class QuizQuestion
    {
        [UnityEngine.TextArea(3, 5)]
        public string questionText;
        public bool isTrueFalse = true;
        public string[] choices = new string[] { "Benar", "Salah" }; // Default choices for True/False
        public int correctAnswerIndex; // 0 for True/A, 1 for False/B
        [UnityEngine.TextArea(2, 4)]
        public string correctFeedback;
        [UnityEngine.TextArea(2, 4)]
        public string incorrectFeedback;

        // Optional placeholders for future features
        public int rewardTiles = 0;
        public int penaltyTiles = 0;
    }
}
