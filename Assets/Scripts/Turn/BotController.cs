using System.Collections;
using UnityEngine;
using Player;
using Dice;

namespace Turn
{
    public class BotController : MonoBehaviour
    {
        public static BotController Instance;

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

            float randomCharge = Random.Range(5f, 95f);
            Debug.Log($"[Bot] {botData.playerName} rolling with {randomCharge:F1}% charge.");

            if (DiceGaugeController.Instance != null)
                DiceGaugeController.Instance.ForceAutoRoll(randomCharge);
        }
    }
}
