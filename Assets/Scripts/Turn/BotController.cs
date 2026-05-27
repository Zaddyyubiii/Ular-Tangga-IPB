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
            Debug.Log($"[Bot] {botData.playerName} is thinking...");
            
            // Wait random delay between 0.5s and 1.5s
            float delay = Random.Range(0.5f, 1.5f);
            yield return new WaitForSeconds(delay);

            if (Core.GameManager.Instance.currentState != Core.GameState.WaitingForInput)
                yield break;

            // Roll dadu dengan charge persen acak
            float randomCharge = Random.Range(0f, 100f);
            
            Debug.Log($"[Bot] {botData.playerName} rolls dadu with {randomCharge:F1}% charge");
            
            if (DiceGaugeController.Instance != null)
            {
                DiceGaugeController.Instance.TriggerAutoRoll(randomCharge);
            }
        }
    }
}
