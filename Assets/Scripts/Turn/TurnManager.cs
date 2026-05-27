using System;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Turn
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance;

        [Header("Turn Timing")]
        public float turnDuration = 10f;
        public float currentTimer = 10f;
        private bool timerActive = false;
        public int currentTurnCount = 0;

        public int currentPlayerIndex = -1;
        private List<PlayerData> players = new List<PlayerData>();

        public Action<PlayerData> OnTurnStarted;
        public Action<float> OnTimerUpdated;
        public Action OnTimerExpired;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void SetPlayers(List<PlayerData> activePlayers)
        {
            players = activePlayers;
            currentPlayerIndex = -1;
            currentTurnCount = 0;
        }

        private void Update()
        {
            if (timerActive)
            {
                currentTimer -= Time.deltaTime;
                OnTimerUpdated?.Invoke(currentTimer);

                if (currentTimer <= 0f)
                {
                    currentTimer = 0f;
                    timerActive = false;
                    OnTimerExpired?.Invoke();
                }
            }
        }

        public void StartNextTurn()
        {
            if (players == null || players.Count == 0) return;

            // Check if game is over (all active players finished)
            if (Core.GameManager.Instance.AreAllActivePlayersFinished())
            {
                timerActive = false;
                Core.GameManager.Instance.ShowFinalRanking();
                return;
            }

            // Move to next player index
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            PlayerData activePlayer = players[currentPlayerIndex];

            Debug.Log($"[Turn] Cycling turn. Player: {activePlayer.playerName}, Index: {currentPlayerIndex}, DroppedOut: {activePlayer.isDroppedOut}, SkipTurns: {activePlayer.skipTurns}, Finished: {activePlayer.isFinished}");

            // If dropped out, skip immediately
            if (activePlayer.isDroppedOut)
            {
                Debug.Log($"Skipping {activePlayer.playerName} because dropped out.");
                StartNextTurn();
                return;
            }

            // If already finished, skip immediately
            if (activePlayer.isFinished)
            {
                Debug.Log($"Skipping {activePlayer.playerName} because already finished.");
                StartNextTurn();
                return;
            }

            // If suspended (skipTurns > 0)
            if (activePlayer.skipTurns > 0)
            {
                activePlayer.skipTurns--;
                Debug.Log($"[Skorsing] {activePlayer.playerName} sedang diskors dan melewati giliran ini. Sisa skors: {activePlayer.skipTurns}");
                
                // Show temporary overlay or message popup, then move to next turn
                if (UI.PopupController.Instance != null)
                {
                    UI.PopupController.Instance.ShowPopup(
                        "Skorsing Akademik",
                        $"{activePlayer.playerName} sedang menjalani masa skorsing akademik akibat pelanggaran tata tertib dan melewati giliran ini.",
                        () => { StartNextTurn(); }
                    );
                }
                else
                {
                    StartNextTurn();
                }
                return;
            }

            // Valid active player, start turn
            currentTurnCount++;
            currentTimer = turnDuration;
            timerActive = true;
            OnTurnStarted?.Invoke(activePlayer);

            // Handle Bot AI trigger
            if (activePlayer.isBot)
            {
                Debug.Log($"Bot Player {activePlayer.playerName} started turn");
                if (BotController.Instance != null)
                {
                    BotController.Instance.TriggerBotTurn(activePlayer);
                }
            }
        }

        public void StopTimer()
        {
            timerActive = false;
        }

        public void ResumeTimer()
        {
            timerActive = true;
        }
    }
}
