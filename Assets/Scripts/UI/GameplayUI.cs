using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;
using Turn;

namespace UI
{
    public class GameplayUI : MonoBehaviour
    {
        public static GameplayUI Instance;

        [Header("General HUD References")]
        public TMPro.TextMeshProUGUI labelActiveTurn;
        public TMPro.TextMeshProUGUI labelTimer;
        public TMPro.TextMeshProUGUI labelInstruction;

        [Header("Player Status Grid References")]
        public Transform statusCardGridContainer;
        public GameObject playerStatusCardPrefab; // Will fall back to procedural cards if null
        
        private List<PlayerStatusView> statusCardViews = new List<PlayerStatusView>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            // Subscribe to TurnManager events
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnTurnStarted += HandleTurnStarted;
                TurnManager.Instance.OnTimerUpdated += HandleTimerUpdated;

                // Sync current state immediately in case it was missed due to start order
                if (Core.GameManager.Instance != null)
                {
                    PlayerData active = Core.GameManager.Instance.GetCurrentPlayer();
                    if (active != null)
                    {
                        HandleTurnStarted(active);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnTurnStarted -= HandleTurnStarted;
                TurnManager.Instance.OnTimerUpdated -= HandleTimerUpdated;
            }
        }

        public void InitializeStatusGrid(List<PlayerData> players)
        {
            // Clear existing views
            foreach (Transform child in statusCardGridContainer)
            {
                Destroy(child.gameObject);
            }
            statusCardViews.Clear();

            // Spawn dynamic status cards
            for (int i = 0; i < players.Count; i++)
            {
                GameObject cardGo;
                if (playerStatusCardPrefab != null)
                {
                    cardGo = Instantiate(playerStatusCardPrefab, statusCardGridContainer);
                }
                else
                {
                    // Generate procedural status card if no prefab is provided
                    cardGo = CreateProceduralStatusCard(players[i]);
                }

                PlayerStatusView statusView = cardGo.GetComponent<PlayerStatusView>();
                if (statusView != null)
                {
                    statusView.Refresh(players[i], false);
                    statusCardViews.Add(statusView);
                }
            }
        }

        public void RefreshStatusGrid(List<PlayerData> players, PlayerData activePlayer)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (i < statusCardViews.Count && statusCardViews[i] != null)
                {
                    bool isActive = (activePlayer != null && players[i].id == activePlayer.id);
                    statusCardViews[i].Refresh(players[i], isActive);
                }
            }
        }

        private void HandleTurnStarted(PlayerData activePlayer)
        {
            if (labelActiveTurn != null)
            {
                labelActiveTurn.text = $"GILIRAN: {activePlayer.playerName.ToUpper()}";
                labelActiveTurn.color = activePlayer.playerColor;
            }

            if (labelInstruction != null)
            {
                if (activePlayer.isBot)
                {
                    labelInstruction.text = "Bot sedang berpikir...";
                }
                else
                {
                    labelInstruction.text = "Tahan tombol ROLL (atau tekan SPACE), lepaskan untuk melempar dadu!";
                }
            }

            // Sync grid cards highlights
            if (Core.GameManager.Instance != null)
            {
                RefreshStatusGrid(Core.GameManager.Instance.players, activePlayer);
            }
        }

        private void HandleTimerUpdated(float timeRemaining)
        {
            if (labelTimer != null)
            {
                labelTimer.text = $"Sisa Waktu: {Mathf.CeilToInt(timeRemaining)}s";
                
                // Color alert when timer is low
                if (timeRemaining < 3.5f)
                {
                    labelTimer.color = Color.red;
                }
                else
                {
                    labelTimer.color = Color.white;
                }
            }
        }

        private GameObject CreateProceduralStatusCard(PlayerData data)
        {
            GameObject cardGo = new GameObject($"StatusCard_{data.playerName}");
            cardGo.transform.SetParent(statusCardGridContainer, false);
            
            // Set size and anchors
            RectTransform rTrans = cardGo.AddComponent<RectTransform>();
            rTrans.sizeDelta = new Vector2(170f, 100f);

            Image bgImg = cardGo.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            // Highlight border child
            GameObject borderGo = new GameObject("HighlightBorder");
            borderGo.transform.SetParent(cardGo.transform, false);
            Image borderImg = borderGo.AddComponent<Image>();
            borderImg.color = new Color(0.12f, 0.73f, 0.35f);
            
            RectTransform borderRect = borderGo.GetComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = new Vector2(-2, -2);
            borderRect.offsetMax = new Vector2(2, 2);
            
            // Text values vertically stacked inside
            GameObject contentGo = new GameObject("ContentLayout");
            contentGo.transform.SetParent(cardGo.transform, false);
            
            RectTransform contentRect = contentGo.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.offsetMin = new Vector2(5, 5);
            contentRect.offsetMax = new Vector2(-5, -5);

            VerticalLayoutGroup vLayout = contentGo.AddComponent<VerticalLayoutGroup>();
            vLayout.childAlignment = TextAnchor.UpperLeft;
            vLayout.childControlHeight = true;
            vLayout.childControlWidth = true;
            vLayout.childForceExpandHeight = false;
            vLayout.childForceExpandWidth = true;

            // Labels inside layout
            PlayerStatusView view = cardGo.AddComponent<PlayerStatusView>();
            view.imageHighlightBorder = borderImg;
            
            view.labelName = CreateLabel(contentGo.transform, "LabelName", 12f, Color.white, FontStyles.Bold);
            view.labelTile = CreateLabel(contentGo.transform, "LabelTile", 10f, Color.yellow, FontStyles.Normal);
            view.labelSkulls = CreateLabel(contentGo.transform, "LabelSkulls", 9f, Color.red, FontStyles.Normal);
            view.labelEvolution = CreateLabel(contentGo.transform, "LabelEvo", 8f, Color.cyan, FontStyles.Italic);
            view.labelStatus = CreateLabel(contentGo.transform, "LabelStatus", 9f, Color.white, FontStyles.Bold);

            // Set background color source
            view.imageAvatarBackground = bgImg;

            return cardGo;
        }

        private TMPro.TextMeshProUGUI CreateLabel(Transform parent, string name, float fontSize, Color color, FontStyles style)
        {
            GameObject txtGo = new GameObject(name);
            txtGo.transform.SetParent(parent, false);
            TMPro.TextMeshProUGUI text = txtGo.AddComponent<TMPro.TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.color = color;
            
            if (style == FontStyles.Bold) text.fontStyle = TMPro.FontStyles.Bold;
            else if (style == FontStyles.Italic) text.fontStyle = TMPro.FontStyles.Italic;
            
            return text;
        }

        private enum FontStyles { Normal, Bold, Italic }
    }
}
