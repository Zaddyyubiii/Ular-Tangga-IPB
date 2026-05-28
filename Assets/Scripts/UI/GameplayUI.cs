using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;
using Turn;
using Dice;

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
                    statusView.Bind(players[i], false);
                    statusCardViews.Add(statusView);
                }
            }

            // Reposition cards to corners and center HUD dynamically
            RepositionHUDToGetRichStyle();
        }

        public void RepositionHUDToGetRichStyle()
        {
            Debug.Log("[GameplayUI] Applying LINE Let's Get Rich UI Style Overhaul...");

            // Find the root full-screen Canvas
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindAnyObjectByType<Canvas>();
            }
            Transform mainCanvas = canvas != null ? canvas.transform : (statusCardGridContainer != null ? statusCardGridContainer.parent : null);

            // 1. Center and Scale the Board Panel slightly to fit the HUD perfectly
            if (Core.GameManager.Instance != null && Core.GameManager.Instance.boardContainer != null)
            {
                RectTransform boardRt = Core.GameManager.Instance.boardContainer.GetComponent<RectTransform>();
                if (boardRt != null)
                {
                    boardRt.anchorMin = new Vector2(0.5f, 0.5f);
                    boardRt.anchorMax = new Vector2(0.5f, 0.5f);
                    boardRt.pivot = new Vector2(0.5f, 0.5f);
                    boardRt.anchoredPosition = new Vector2(0f, 32f); // Centered and slightly raised
                    boardRt.localScale = new Vector3(0.83f, 0.83f, 1f); // Scaled down slightly to fit HUD elegantly
                }
            }

            // 2. Set Background color to the original beautiful dark blue-grey
            GameObject mainBg = GameObject.Find("Background");
            if (mainBg != null)
            {
                Image bgImg = mainBg.GetComponent<Image>();
                if (bgImg != null)
                {
                    bgImg.color = new Color(0.13f, 0.13f, 0.18f, 1f); // Original dark grey-blue background
                }
            }

            // 3. Disable layout groups on player cards container so we can position them manually
            if (statusCardGridContainer != null && mainCanvas != null)
            {
                statusCardGridContainer.SetParent(mainCanvas, true);

                var layout = statusCardGridContainer.GetComponent<LayoutGroup>();
                if (layout != null) layout.enabled = false;

                var contentSizeFitter = statusCardGridContainer.GetComponent<ContentSizeFitter>();
                if (contentSizeFitter != null) contentSizeFitter.enabled = false;

                // Make the container itself fill the full screen so child anchored positions correspond to screen coordinates
                RectTransform containerRt = statusCardGridContainer.GetComponent<RectTransform>();
                if (containerRt != null)
                {
                    containerRt.anchorMin = Vector2.zero;
                    containerRt.anchorMax = Vector2.one;
                    containerRt.offsetMin = Vector2.zero;
                    containerRt.offsetMax = Vector2.zero;
                    containerRt.pivot = new Vector2(0.5f, 0.5f);
                    containerRt.anchoredPosition = Vector2.zero;
                }
            }

            // 4. Position Player Cards at 4 corners
            float cardWidth = 180f;
            float cardHeight = 110f;
            for (int i = 0; i < statusCardViews.Count; i++)
            {
                if (statusCardViews[i] == null) continue;
                RectTransform rt = statusCardViews[i].GetComponent<RectTransform>();
                if (rt == null) continue;

                rt.sizeDelta = new Vector2(cardWidth, cardHeight);

                switch (i)
                {
                    case 0: // Player 1: Top-Left
                        rt.anchorMin = new Vector2(0f, 1f);
                        rt.anchorMax = new Vector2(0f, 1f);
                        rt.pivot = new Vector2(0f, 1f);
                        rt.anchoredPosition = new Vector2(30f, -30f);
                        break;
                    case 1: // Player 2: Top-Right
                        rt.anchorMin = new Vector2(1f, 1f);
                        rt.anchorMax = new Vector2(1f, 1f);
                        rt.pivot = new Vector2(1f, 1f);
                        rt.anchoredPosition = new Vector2(-30f, -30f);
                        break;
                    case 2: // Player 3: Bottom-Left
                        rt.anchorMin = new Vector2(0f, 0f);
                        rt.anchorMax = new Vector2(0f, 0f);
                        rt.pivot = new Vector2(0f, 0f);
                        rt.anchoredPosition = new Vector2(30f, 30f);
                        break;
                    case 3: // Player 4: Bottom-Right
                        rt.anchorMin = new Vector2(1f, 0f);
                        rt.anchorMax = new Vector2(1f, 0f);
                        rt.pivot = new Vector2(1f, 0f);
                        rt.anchoredPosition = new Vector2(-30f, 30f);
                        break;
                }
            }

            // 5. Move Dice Gauge Panel to Bottom-Center and rearrange flat
            GameObject diceGaugePanel = GameObject.Find("DiceGaugePanel");
            if (diceGaugePanel != null && mainCanvas != null)
            {
                diceGaugePanel.transform.SetParent(mainCanvas, true);

                RectTransform diceRt = diceGaugePanel.GetComponent<RectTransform>();
                if (diceRt != null)
                {
                    diceRt.anchorMin = new Vector2(0.5f, 0f);
                    diceRt.anchorMax = new Vector2(0.5f, 0f);
                    diceRt.pivot = new Vector2(0.5f, 0f);
                    diceRt.anchoredPosition = new Vector2(0f, 10f); // Very bottom center, very flat
                    diceRt.sizeDelta = new Vector2(580f, 52f); // Elegant flat bar
                    
                    // Translucent dark glassmorphism background
                    Image bgImage = diceGaugePanel.GetComponent<Image>();
                    if (bgImage != null)
                    {
                        bgImage.material = null; // Revert blur material override
                        bgImage.color = new Color(0.1f, 0.1f, 0.15f, 0.85f); // Beautiful clean translucent dark color
                        
                        Outline outline = diceGaugePanel.GetComponent<Outline>();
                        if (outline == null) outline = diceGaugePanel.gameObject.AddComponent<Outline>();
                        outline.effectColor = new Color(1f, 1f, 1f, 0.2f);
                        outline.effectDistance = new Vector2(1.5f, 1.5f);
                    }

                    // Dynamically rearrange children to be horizontal and flat
                    // Hide the vertical title for flat horizontal elegance
                    Transform titleT = diceGaugePanel.transform.Find("GaugeTitle");
                    if (titleT != null) titleT.gameObject.SetActive(false);

                    // ResultLabel (Leftmost big number)
                    Transform resultT = diceGaugePanel.transform.Find("ResultLabel");
                    if (resultT != null)
                    {
                        RectTransform rRt = resultT.GetComponent<RectTransform>();
                        rRt.anchorMin = new Vector2(0f, 0.5f);
                        rRt.anchorMax = new Vector2(0f, 0.5f);
                        rRt.pivot = new Vector2(0f, 0.5f);
                        rRt.anchoredPosition = new Vector2(15f, 0f);
                        rRt.sizeDelta = new Vector2(50f, 40f);
                        var txt = resultT.GetComponent<TMPro.TextMeshProUGUI>();
                        if (txt != null)
                        {
                            txt.fontSize = 24f;
                            txt.alignment = TMPro.TextAlignmentOptions.Center;
                        }
                    }

                    // Status & Range Labels (Stacked next to result)
                    Transform rangeT = diceGaugePanel.transform.Find("RangeLabel");
                    if (rangeT != null)
                    {
                        RectTransform rngRt = rangeT.GetComponent<RectTransform>();
                        rngRt.anchorMin = new Vector2(0f, 0.5f);
                        rngRt.anchorMax = new Vector2(0f, 0.5f);
                        rngRt.pivot = new Vector2(0f, 0.5f);
                        rngRt.anchoredPosition = new Vector2(70f, 10f); // Top half
                        rngRt.sizeDelta = new Vector2(110f, 20f);
                        var txt = rangeT.GetComponent<TMPro.TextMeshProUGUI>();
                        if (txt != null) txt.alignment = TMPro.TextAlignmentOptions.Left;
                    }

                    Transform statusT = diceGaugePanel.transform.Find("StatusLabel");
                    if (statusT != null)
                    {
                        RectTransform statRt = statusT.GetComponent<RectTransform>();
                        statRt.anchorMin = new Vector2(0f, 0.5f);
                        statRt.anchorMax = new Vector2(0f, 0.5f);
                        statRt.pivot = new Vector2(0f, 0.5f);
                        statRt.anchoredPosition = new Vector2(70f, -10f); // Bottom half
                        statRt.sizeDelta = new Vector2(110f, 20f);
                        var txt = statusT.GetComponent<TMPro.TextMeshProUGUI>();
                        if (txt != null) txt.alignment = TMPro.TextAlignmentOptions.Left;
                    }

                    // GaugeSlider (Middle wide bar stretched dynamically)
                    Transform sliderT = diceGaugePanel.transform.Find("GaugeSlider");
                    if (sliderT != null)
                    {
                        RectTransform sRt = sliderT.GetComponent<RectTransform>();
                        sRt.anchorMin = new Vector2(0f, 0.5f);
                        sRt.anchorMax = new Vector2(1f, 0.5f);
                        sRt.pivot = new Vector2(0.5f, 0.5f);
                        sRt.offsetMin = new Vector2(190f, -10f); // Space for leftmost labels
                        sRt.offsetMax = new Vector2(-130f, 10f); // Space for rightmost button

                        // Color the slider fill with the original beautiful vibrant orange
                        Transform fillT = sliderT.Find("Fill Area/Fill");
                        if (fillT != null)
                        {
                            Image fillImg = fillT.GetComponent<Image>();
                            if (fillImg != null)
                            {
                                fillImg.color = new Color(0.95f, 0.35f, 0.15f, 1f); // Vibrant original orange!
                            }
                        }
                    }

                    // BtnRoll (Rightmost button)
                    Transform rollT = diceGaugePanel.transform.Find("BtnRoll");
                    if (rollT != null)
                    {
                        RectTransform rollRt = rollT.GetComponent<RectTransform>();
                        rollRt.anchorMin = new Vector2(1f, 0.5f);
                        rollRt.anchorMax = new Vector2(1f, 0.5f);
                        rollRt.pivot = new Vector2(1f, 0.5f);
                        rollRt.anchoredPosition = new Vector2(-15f, 0f);
                        rollRt.sizeDelta = new Vector2(100f, 38f);
                        
                        Image rImg = rollT.GetComponent<Image>();
                        if (rImg != null)
                        {
                            rImg.color = new Color(0.9f, 0.3f, 0.15f, 1f); // Vibrant original orange-red!
                        }
                    }
                }
            }

            // 6. Relocate Active Turn Text and Timer to a beautiful, styled Top HUD Capsule Banner
            GameObject topHUD = GameObject.Find("TopHUDCapsule");
            if (topHUD == null && mainCanvas != null)
            {
                topHUD = new GameObject("TopHUDCapsule", typeof(RectTransform));
                topHUD.transform.SetParent(mainCanvas, false);
                
                Image capsuleImg = topHUD.AddComponent<Image>();
                capsuleImg.color = new Color(0.1f, 0.1f, 0.15f, 0.85f); // Translucent clean dark background
                
                Outline outline = topHUD.AddComponent<Outline>();
                outline.effectColor = new Color(1f, 1f, 1f, 0.2f);
                outline.effectDistance = new Vector2(1.5f, 1.5f);
            }

            if (topHUD != null)
            {
                RectTransform topHUDTrans = topHUD.GetComponent<RectTransform>();
                if (topHUDTrans != null)
                {
                    topHUDTrans.anchorMin = new Vector2(0.5f, 1f);
                    topHUDTrans.anchorMax = new Vector2(0.5f, 1f);
                    topHUDTrans.pivot = new Vector2(0.5f, 1f);
                    topHUDTrans.anchoredPosition = new Vector2(0f, -15f); // Float near top center
                    topHUDTrans.sizeDelta = new Vector2(420f, 70f); // Sleek horizontal banner
                }

                if (labelActiveTurn != null)
                {
                    RectTransform turnRt = labelActiveTurn.GetComponent<RectTransform>();
                    if (turnRt != null)
                    {
                        turnRt.transform.SetParent(topHUD.transform, true);
                        turnRt.anchorMin = new Vector2(0.5f, 1f);
                        turnRt.anchorMax = new Vector2(0.5f, 1f);
                        turnRt.pivot = new Vector2(0.5f, 1f);
                        turnRt.anchoredPosition = new Vector2(0f, -10f);
                        turnRt.sizeDelta = new Vector2(400f, 25f);
                        labelActiveTurn.fontSize = 15f; // Sleek and compact
                        labelActiveTurn.fontStyle = TMPro.FontStyles.Bold;
                        labelActiveTurn.alignment = TMPro.TextAlignmentOptions.Center;
                    }
                }

                if (labelTimer != null)
                {
                    RectTransform timerRt = labelTimer.GetComponent<RectTransform>();
                    if (timerRt != null)
                    {
                        timerRt.transform.SetParent(topHUD.transform, true);
                        timerRt.anchorMin = new Vector2(0.5f, 0f);
                        timerRt.anchorMax = new Vector2(0.5f, 0f);
                        timerRt.pivot = new Vector2(0.5f, 0f);
                        timerRt.anchoredPosition = new Vector2(0f, 8f);
                        timerRt.sizeDelta = new Vector2(400f, 22f);
                        labelTimer.fontSize = 13f; // Sleek and compact
                        labelTimer.alignment = TMPro.TextAlignmentOptions.Center;
                    }
                }
            }

            if (labelInstruction != null && mainCanvas != null)
            {
                RectTransform instRt = labelInstruction.GetComponent<RectTransform>();
                if (instRt != null)
                {
                    instRt.transform.SetParent(mainCanvas, true);
                    instRt.anchorMin = new Vector2(0.5f, 0f);
                    instRt.anchorMax = new Vector2(0.5f, 0f);
                    instRt.pivot = new Vector2(0.5f, 0f);
                    instRt.anchoredPosition = new Vector2(0f, 75f); // Placed beautifully above the flat dice gauge panel
                    instRt.sizeDelta = new Vector2(800f, 40f); // Wide enough to prevent vertical wrapping
                    labelInstruction.fontSize = 12f;
                    labelInstruction.alignment = TMPro.TextAlignmentOptions.Center;
                    labelInstruction.color = new Color(0.9f, 0.9f, 0.9f, 0.85f);
                }
            }

            // 7. Automatically deactivate the old rigid SidebarPanel and RightPanel
            GameObject sidebarPanel = GameObject.Find("SidebarPanel");
            if (sidebarPanel != null)
            {
                sidebarPanel.SetActive(false);
            }

            GameObject rightPanel = GameObject.Find("RightPanel");
            if (rightPanel != null)
            {
                rightPanel.SetActive(false);
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
            // Log turn transition (Section 11)
            Debug.Log($"Current turn: {activePlayer.playerName}. Applying bright version of player color.");

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

            // 1. Highlight border child (first sibling, drawn behind background)
            GameObject borderGo = new GameObject("HighlightBorder");
            borderGo.transform.SetParent(cardGo.transform, false);
            Image borderImg = borderGo.AddComponent<Image>();
            borderImg.color = new Color(0.12f, 0.73f, 0.35f);
            
            RectTransform borderRect = borderGo.GetComponent<RectTransform>();
            borderRect.anchorMin = Vector2.zero;
            borderRect.anchorMax = Vector2.one;
            borderRect.offsetMin = new Vector2(-2, -2);
            borderRect.offsetMax = new Vector2(2, 2);

            // 2. Background child (second sibling, drawn on top of border to cover center)
            GameObject bgGo = new GameObject("Background");
            bgGo.transform.SetParent(cardGo.transform, false);
            Image bgImg = bgGo.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            RectTransform bgRect = bgGo.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
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
