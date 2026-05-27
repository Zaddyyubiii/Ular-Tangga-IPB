using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;
using Board;
using Turn;
using UI;
using Dice;
using Quiz;
using Audio;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        // Static configuration shared from MainMenu
        public static int numRealPlayers = 4;

        [Header("State Machine")]
        public GameState currentState = GameState.MainMenu;

        [Header("Game Data References")]
        public BoardConfig boardConfig;
        public MessageBank messageBank;
        public QuizBank quizBank;

        [Header("Prefabs & Parents")]
        public Transform boardContainer; // Container inside Canvas for the board serpentine grid
        public GameObject playerTokenPrefab; // Null fallback supported
        public Transform tokenContainer; // Visual container for player tokens

        [Header("Active Game Status")]
        public List<PlayerData> players = new List<PlayerData>();
        public Dictionary<int, PlayerToken> playerTokens = new Dictionary<int, PlayerToken>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            // Subscribe to turn and dice events
            if (DiceGaugeController.Instance != null)
            {
                DiceGaugeController.Instance.OnDiceRolled += HandleDiceRolled;
            }

            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnTimerExpired += HandleTimerExpired;
            }

            // Detect if we are in GameScene and boot automatically
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GameScene" || boardContainer != null)
            {
                InitializeGame();
            }
        }

        private void OnDestroy()
        {
            if (DiceGaugeController.Instance != null)
            {
                DiceGaugeController.Instance.OnDiceRolled -= HandleDiceRolled;
            }

            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnTimerExpired -= HandleTimerExpired;
            }
        }

        public void InitializeGame()
        {
            Debug.Log("[GameManager] Initializing Game Scene...");
            
            // Load player count from PlayerPrefs (persists across scene loads)
            numRealPlayers = PlayerPrefs.GetInt("NumRealPlayers", numRealPlayers);
            
            // Create default data configurations if null
            if (boardConfig == null) boardConfig = ScriptableObject.CreateInstance<BoardConfig>();
            if (messageBank == null) messageBank = ScriptableObject.CreateInstance<MessageBank>();
            if (quizBank == null) quizBank = ScriptableObject.CreateInstance<QuizBank>();

            currentState = GameState.Prologue;

            // 1. Generate Board Grid serpentine mapping
            if (BoardManager.Instance != null)
            {
                if (BoardManager.Instance.boardPanel == null && boardContainer != null)
                {
                    BoardManager.Instance.boardPanel = boardContainer.GetComponent<RectTransform>();
                }
                BoardManager.Instance.boardConfig = boardConfig;
                BoardManager.Instance.GenerateBoard();
            }

            // 2. Setup Players
            players.Clear();
            playerTokens.Clear();

            // Clean active tokens from container
            if (tokenContainer != null)
            {
                foreach (Transform child in tokenContainer)
                {
                    Destroy(child.gameObject);
                }
            }
            else
            {
                tokenContainer = boardContainer;
            }

            Color[] colors = new Color[]
            {
                new Color(0.9f, 0.2f, 0.2f),   // Red
                new Color(0.2f, 0.4f, 0.9f),   // Blue
                new Color(0.12f, 0.73f, 0.35f),// Green
                new Color(0.95f, 0.75f, 0.15f) // Yellow/Gold
            };

            for (int i = 1; i <= 4; i++)
            {
                PlayerData p = new PlayerData();
                p.id = i;
                p.playerName = $"Mahasiswa {i}";
                p.isBot = (i > numRealPlayers);
                p.currentTile = 0; // Starts at petak 0 (start)
                p.playerColor = colors[i - 1];

                // Initial evolution evaluation
                if (PlayerEvolutionController.Instance != null)
                {
                    PlayerEvolutionController.Instance.EvaluateEvolution(p);
                }

                players.Add(p);

                // Spawn visual token
                GameObject tokenGo;
                if (playerTokenPrefab != null)
                {
                    tokenGo = Instantiate(playerTokenPrefab, tokenContainer);
                }
                else
                {
                    // Generate token procedurally
                    tokenGo = new GameObject($"Token_{p.playerName}");
                    tokenGo.transform.SetParent(tokenContainer, false);
                    Image img = tokenGo.AddComponent<Image>();
                    
                    // Spawn inner border
                    GameObject borderGo = new GameObject("Border");
                    borderGo.transform.SetParent(tokenGo.transform, false);
                    Image borderImg = borderGo.AddComponent<Image>();
                    borderImg.color = Color.white;
                    RectTransform borderRect = borderGo.GetComponent<RectTransform>();
                    borderRect.anchorMin = new Vector2(0.1f, 0.1f);
                    borderRect.anchorMax = new Vector2(0.9f, 0.9f);
                    borderRect.offsetMin = Vector2.zero;
                    borderRect.offsetMax = Vector2.zero;
                }

                RectTransform rTrans = tokenGo.GetComponent<RectTransform>();
                // Adjust scale so 4 tokens can fit in a single tile together slightly offset
                rTrans.sizeDelta = new Vector2(25f, 25f);
                
                PlayerToken token = tokenGo.GetComponent<PlayerToken>();
                if (token == null) token = tokenGo.AddComponent<PlayerToken>();
                
                token.Initialize(p);
                playerTokens[p.id] = token;

                // Position initially at start (Tile 0) with a small offset based on player index
                Vector2 startPos = BoardManager.Instance.GetTilePosition(0);
                rTrans.anchoredPosition = startPos + GetTokenOffset(p.id);
            }

            // 3. Initialize Status Display HUD
            if (GameplayUI.Instance != null)
            {
                GameplayUI.Instance.InitializeStatusGrid(players);
            }

            // 4. Play Gameplay Music (safe)
            if (AudioManager.Instance != null && AudioManager.Instance.gameplayBGM != null)
            {
                AudioManager.Instance.PlayBGM(AudioManager.Instance.gameplayBGM);
            }

            // 5. Fire Prologue
            if (PrologueUI.Instance != null)
            {
                PrologueUI.Instance.ShowPrologue(messageBank.prologueText, StartMainGameplayLoop);
            }
            else
            {
                StartMainGameplayLoop();
            }
        }

        private void StartMainGameplayLoop()
        {
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.SetPlayers(players);
                currentState = GameState.WaitingForInput;
                TurnManager.Instance.StartNextTurn();
            }
        }

        public PlayerData GetCurrentPlayer()
        {
            if (TurnManager.Instance == null) return null;
            int idx = TurnManager.Instance.currentPlayerIndex;
            if (idx >= 0 && idx < players.Count)
            {
                return players[idx];
            }
            return null;
        }

        private Vector2 GetTokenOffset(int playerId)
        {
            switch (playerId)
            {
                case 1: return new Vector2(-12f, 12f);
                case 2: return new Vector2(12f, 12f);
                case 3: return new Vector2(-12f, -12f);
                case 4: return new Vector2(12f, -12f);
                default: return Vector2.zero;
            }
        }

        private void HandleDiceRolled(int rollValue)
        {
            if (currentState != GameState.WaitingForInput) return;
            
            currentState = GameState.Rolling;
            TurnManager.Instance.StopTimer();

            StartCoroutine(ProcessRollCo(rollValue));
        }

        private void HandleTimerExpired()
        {
            if (currentState != GameState.WaitingForInput) return;
            
            Debug.Log("[GameManager] Turn timer expired! Triggering auto-roll.");
            
            // Auto roll with random charge
            if (DiceGaugeController.Instance != null)
            {
                DiceGaugeController.Instance.TriggerAutoRoll(Random.Range(0f, 100f));
            }
        }

        private IEnumerator ProcessRollCo(int diceValue)
        {
            PlayerData curPlayer = GetCurrentPlayer();
            if (curPlayer == null) yield break;

            int startTile = curPlayer.currentTile;
            int targetTile = startTile + diceValue;

            yield return new WaitForSeconds(0.6f); // Wait for roll animation text display

            // Edge Case 1: Overshoot finish tile 100
            if (targetTile > 100)
            {
                Debug.Log($"[Overshoot] Player {curPlayer.playerName} rolled {diceValue} from tile {startTile}. Stays at tile.");
                
                if (PopupController.Instance != null)
                {
                    PopupController.Instance.ShowPopup(
                        "Kelebihan Langkah!", 
                        $"Kamu harus mendapatkan angka dadu yang tepat ({100 - startTile}) untuk mencapai petak 100. Lemparanmu ({diceValue}) melewati garis finish, kamu tetap di petak {startTile}.",
                        () => { TransitionToNextTurn(); }
                    );
                }
                else
                {
                    TransitionToNextTurn();
                }
                yield break;
            }

            // Normal Movement
            currentState = GameState.Moving;

            // Generate path list of tile positions
            List<Vector2> pathPositions = new List<Vector2>();
            for (int t = startTile + 1; t <= targetTile; t++)
            {
                Vector2 cellCenter = BoardManager.Instance.GetTilePosition(t);
                pathPositions.Add(cellCenter + GetTokenOffset(curPlayer.id));
            }

            // Animate token movement
            if (playerTokens.TryGetValue(curPlayer.id, out PlayerToken token))
            {
                yield return token.MoveTileByTile(pathPositions, 0.22f);
            }

            curPlayer.currentTile = targetTile;
            
            // Evaluate evolution after movement
            if (PlayerEvolutionController.Instance != null)
            {
                PlayerEvolutionController.Instance.EvaluateEvolution(curPlayer);
            }
            if (token != null) token.UpdateVisuals();

            // Refresh Status grid
            if (GameplayUI.Instance != null)
            {
                GameplayUI.Instance.RefreshStatusGrid(players, curPlayer);
            }

            yield return new WaitForSeconds(0.2f);

            // Tile Resolution
            currentState = GameState.ResolvingTile;
            ResolveTileEffect(curPlayer, targetTile, diceValue);
        }

        private void ResolveTileEffect(PlayerData player, int tile, int roll)
        {
            TileDefinition def = boardConfig.GetTileDefinition(tile);
            
            Debug.Log($"[Tile Resolution] Player {player.playerName} landed on Tile {tile} (Type: {def.type})");

            switch (def.type)
            {
                case TileType.Finish:
                    // Win game
                    DeclareWinner(player);
                    break;

                case TileType.Question:
                    // Show quiz popup
                    QuizQuestion question = quizBank.GetRandomQuestion();
                    if (QuizPopup.Instance != null && question != null)
                    {
                        QuizPopup.Instance.ShowQuiz(question, () => { TransitionToNextTurn(); });
                    }
                    else
                    {
                        TransitionToNextTurn();
                    }
                    break;

                case TileType.Skull:
                    // Skorsing: skull count +1, skip next turn, -40 tiles
                    player.skullHitCount++;
                    player.skipTurns = 1;
                    
                    int skullPenaltyTile = Mathf.Max(0, tile - 40);
                    string skullMsg = messageBank.GetSkullMessage(player.skullHitCount - 1);

                    if (player.skullHitCount >= 3)
                    {
                        player.isDroppedOut = true;
                        Debug.Log($"[Skull] Player {player.playerName} hit skull 3 times. DROP OUT!");
                        
                        if (PopupController.Instance != null)
                        {
                            PopupController.Instance.ShowPopup(
                                "DROP OUT!", 
                                $"{skullMsg}\n\nSanksi akumulatif mencapai batas! Anda dinyatakan Drop Out (DO) dari IPB University.",
                                () => {
                                    // Move token off board or make transparent
                                    if (playerTokens.TryGetValue(player.id, out PlayerToken t)) t.UpdateVisuals();
                                    TransitionToNextTurn();
                                }, 
                                playExplosion: true
                            );
                        }
                        else
                        {
                            TransitionToNextTurn();
                        }
                    }
                    else
                    {
                        if (PopupController.Instance != null)
                        {
                            PopupController.Instance.ShowPopup(
                                "Sanksi Skorsing Akademik!", 
                                $"{skullMsg}\n\nAnda diskors (melewati 1 giliran) dan diturunkan 4 baris (-40 petak) ke petak {skullPenaltyTile}.",
                                () => {
                                    StartCoroutine(AnimateTeleportCo(player, skullPenaltyTile));
                                }, 
                                playExplosion: true
                            );
                        }
                        else
                        {
                            StartCoroutine(AnimateTeleportCo(player, skullPenaltyTile));
                        }
                    }
                    break;

                case TileType.Snake:
                    // Turun baris
                    string snakeMsg = def.customMessage;
                    if (string.IsNullOrEmpty(snakeMsg))
                    {
                        snakeMsg = messageBank.GetSnakeMessage(def.severity);
                    }

                    if (Audio.AudioManager.Instance != null)
                    {
                        Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.snakeClip);
                    }

                    if (PopupController.Instance != null)
                    {
                        PopupController.Instance.ShowPopup(
                            "Pelanggaran!", 
                            snakeMsg,
                            () => {
                                StartCoroutine(AnimateTeleportCo(player, def.targetTileIndex));
                            }
                        );
                    }
                    else
                    {
                        StartCoroutine(AnimateTeleportCo(player, def.targetTileIndex));
                    }
                    break;

                case TileType.Ladder:
                    // Naik baris
                    string ladderMsg = def.customMessage;
                    if (string.IsNullOrEmpty(ladderMsg))
                    {
                        ladderMsg = messageBank.GetLadderMessage(def.severity);
                    }

                    if (Audio.AudioManager.Instance != null)
                    {
                        Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.ladderClip);
                    }

                    if (PopupController.Instance != null)
                    {
                        PopupController.Instance.ShowPopup(
                            "Prestasi Akademik!", 
                            ladderMsg,
                            () => {
                                StartCoroutine(AnimateTeleportCo(player, def.targetTileIndex));
                            }
                        );
                    }
                    else
                    {
                        StartCoroutine(AnimateTeleportCo(player, def.targetTileIndex));
                    }
                    break;

                case TileType.Normal:
                default:
                    // Positive informative messages based on roll
                    int positiveIndex = (roll - 2) % 6; // Range [2, 12] mapped to [0, 5]
                    string posMsg = messageBank.GetPositiveMessage(positiveIndex);
                    
                    string formattedMsg = $"Selamat, Anda telah mendapatkan {roll} poin dadu dikarenakan pencapaian positif berikut:\n\n\"{posMsg}\"";

                    if (PopupController.Instance != null)
                    {
                        PopupController.Instance.ShowPopup(
                            "Kegiatan Positif", 
                            formattedMsg,
                            () => { TransitionToNextTurn(); }
                        );
                    }
                    else
                    {
                        TransitionToNextTurn();
                    }
                    break;
            }
        }

        private IEnumerator AnimateTeleportCo(PlayerData player, int destTile)
        {
            currentState = GameState.Moving;

            // Make a quick hop/animation from current position to target destination
            List<Vector2> path = new List<Vector2>();
            
            // For snakes and ladders, animate directly in 1 big jump or intermediate tiles
            path.Add(BoardManager.Instance.GetTilePosition(destTile) + GetTokenOffset(player.id));

            if (playerTokens.TryGetValue(player.id, out PlayerToken token))
            {
                yield return token.MoveTileByTile(path, 0.4f);
            }

            player.currentTile = destTile;

            // Re-evaluate evolution
            if (PlayerEvolutionController.Instance != null)
            {
                PlayerEvolutionController.Instance.EvaluateEvolution(player);
            }
            if (token != null) token.UpdateVisuals();

            // Refresh HUD
            if (GameplayUI.Instance != null)
            {
                GameplayUI.Instance.RefreshStatusGrid(players, player);
            }

            yield return new WaitForSeconds(0.2f);
            TransitionToNextTurn();
        }

        public void DeclareWinner(PlayerData winner)
        {
            currentState = GameState.GameOver;
            winner.isWinner = true;
            
            if (PlayerEvolutionController.Instance != null)
            {
                PlayerEvolutionController.Instance.EvaluateEvolution(winner);
            }

            if (playerTokens.TryGetValue(winner.id, out PlayerToken token))
            {
                token.UpdateVisuals();
            }

            // Sync grid cards
            if (GameplayUI.Instance != null)
            {
                GameplayUI.Instance.RefreshStatusGrid(players, winner);
            }

            if (GameOverUI.Instance != null)
            {
                GameOverUI.Instance.ShowWinner(winner);
            }
            else
            {
                Debug.LogWarning($"[GameOver] GAME OVER! Winner declared: {winner.playerName}");
            }
        }

        private void TransitionToNextTurn()
        {
            currentState = GameState.ChangingTurn;
            
            // Clean up dice controller visuals
            if (DiceGaugeController.Instance != null)
            {
                DiceGaugeController.Instance.ResetGauge();
            }

            // Trigger TurnManager next step
            if (TurnManager.Instance != null)
            {
                currentState = GameState.WaitingForInput;
                TurnManager.Instance.StartNextTurn();
            }
        }
    }
}
