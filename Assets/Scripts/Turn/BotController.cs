using System.Collections;
using UnityEngine;
using Player;
using Dice;
using Quiz;
using UI;

namespace Turn
{
    public class BotController : MonoBehaviour
    {
        public static BotController Instance;

        public int ChooseQuizAnswerIndex(QuizQuestion question)
        {
            // Untuk MVP, bot punya peluang 60% menjawab benar.
            float correctChance = 0.6f;

            if (UnityEngine.Random.value <= correctChance)
            {
                return question.correctAnswerIndex;
            }

            // Pilih jawaban salah secara random.
            System.Collections.Generic.List<int> wrongIndexes = new System.Collections.Generic.List<int>();

            for (int i = 0; i < question.choices.Length; i++)
            {
                if (i != question.correctAnswerIndex)
                {
                    wrongIndexes.Add(i);
                }
            }

            if (wrongIndexes.Count == 0)
            {
                return question.correctAnswerIndex;
            }

            return wrongIndexes[UnityEngine.Random.Range(0, wrongIndexes.Count)];
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void TriggerBotTurn(PlayerData botData)
        {
            if (!botData.isBot) return;
            StartCoroutine(ExecuteBotTurnCo(botData));
        }

        private IEnumerator ExecuteBotTurnCo(PlayerData botData)
        {
            Debug.Log($"[Bot] {botData.playerName} sedang berpikir...");

            // Wait for game state to settle
            float elapsed = 0f;
            float maxWait = 3f;
            while (Core.GameManager.Instance.currentState != Core.GameState.WaitingForInput && elapsed < maxWait)
            {
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }

            // Extra delay for dramatic effect
            float delay = Random.Range(1.0f, 2.0f);
            yield return new WaitForSeconds(delay);

            // Verify still this bot's turn
            if (Core.GameManager.Instance.currentState != Core.GameState.WaitingForInput)
            {
                Debug.Log($"[Bot] {botData.playerName} skipping - state changed.");
                yield break;
            }

            var currentPlayer = Core.GameManager.Instance.GetCurrentPlayer();
            if (currentPlayer == null || currentPlayer.id != botData.id)
            {
                Debug.Log($"[Bot] {botData.playerName} skipping - no longer active player.");
                yield break;
            }

            // Show rolling indicator first
            if (DiceRollPopupUI.Instance != null)
            {
                yield return DiceRollPopupUI.Instance.ShowBotRollingIndicator(botData);
            }
            else
            {
                if (GameplayUI.Instance != null)
                {
                    GameplayUI.Instance.ShowDiceRollingIndicator(botData);
                }
                yield return new WaitForSeconds(1.0f); // Wait 1s for rolling indicator visibility
            }

            float targetCharge = 50f;
            if (botData.currentTile < 70)
            {
                targetCharge = Random.Range(55f, 100f);
            }
            else if (botData.currentTile <= 88)
            {
                targetCharge = Random.Range(35f, 85f);
            }
            else
            {
                int needed = 100 - botData.currentTile;
                if (needed > 0 && needed <= 12)
                {
                    float baseCharge = DiceRollResolver.GetTargetChargeForValue(needed);
                    targetCharge = Mathf.Clamp(baseCharge + Random.Range(-4f, 4f), 5f, 95f);
                }
                else
                {
                    targetCharge = Random.Range(5f, 25f);
                }
            }

            Debug.Log($"[Bot] {botData.playerName} rolling with strategic {targetCharge:F1}% charge.");

            if (DiceGaugeController.Instance != null)
                DiceGaugeController.Instance.ForceAutoRoll(targetCharge);
        }
    }
}
