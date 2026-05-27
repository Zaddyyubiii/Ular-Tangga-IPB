using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using Board;
using Core;
using Dice;
using Player;
using Turn;
using UI;
using Audio;
using Quiz;

namespace UlarTangga.EditorSetup
{
    public class SceneSetupHelper : EditorWindow
    {
        [MenuItem("Ular Tangga/Setup Playable Scenes")]
        public static void SetupPlayableScenes()
        {
            Debug.Log("[SceneSetup] Beginning scene auto-setup...");

            // 0. Automatically switch Active Input Handling to 'Both' to prevent StandaloneInputModule crash
            try
            {
                SerializedObject playerSettings = new SerializedObject(Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings"));
                SerializedProperty activeInputHandler = playerSettings.FindProperty("activeInputHandler");
                if (activeInputHandler != null && activeInputHandler.intValue != 2) // 2 represents 'Both'
                {
                    activeInputHandler.intValue = 2; // Set to Both
                    playerSettings.ApplyModifiedProperties();
                    Debug.Log("[SceneSetup] Automatically switched Active Input Handling to 'Both' for Input System compatibility.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[SceneSetup] Failed to automatically set activeInputHandler: " + ex.Message);
            }

            EnsureProjectFolders();

            // 1. Generate core assets
            BoardConfig boardConfig = AssetDatabase.LoadAssetAtPath<BoardConfig>("Assets/ScriptableObjects/BoardConfig.asset");
            if (boardConfig == null)
            {
                boardConfig = ScriptableObject.CreateInstance<BoardConfig>();
                AssetDatabase.CreateAsset(boardConfig, "Assets/ScriptableObjects/BoardConfig.asset");
            }

            MessageBank messageBank = AssetDatabase.LoadAssetAtPath<MessageBank>("Assets/ScriptableObjects/MessageBank.asset");
            if (messageBank == null)
            {
                messageBank = ScriptableObject.CreateInstance<MessageBank>();
                AssetDatabase.CreateAsset(messageBank, "Assets/ScriptableObjects/MessageBank.asset");
            }

            QuizBank quizBank = AssetDatabase.LoadAssetAtPath<QuizBank>("Assets/ScriptableObjects/QuizBank.asset");
            if (quizBank == null)
            {
                quizBank = ScriptableObject.CreateInstance<QuizBank>();
                AssetDatabase.CreateAsset(quizBank, "Assets/ScriptableObjects/QuizBank.asset");
            }

            AssetDatabase.SaveAssets();
            CreatePlaceholderPrefabs();

            // 2. Build MainMenuScene
            UnityEngine.SceneManagement.Scene mainMenuScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            SetupMainMenuObjects(boardConfig, messageBank, quizBank);
            EditorSceneManager.SaveScene(mainMenuScene, "Assets/Scenes/MainMenuScene.unity");

            // 3. Build GameScene
            UnityEngine.SceneManagement.Scene gameScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            SetupGameSceneObjects(boardConfig, messageBank, quizBank);
            EditorSceneManager.SaveScene(gameScene, "Assets/Scenes/GameScene.unity");

            // 4. Configure Build Settings
            string[] scenePaths = new string[] { "Assets/Scenes/MainMenuScene.unity", "Assets/Scenes/GameScene.unity" };
            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[scenePaths.Length];
            for (int i = 0; i < scenePaths.Length; i++)
            {
                buildScenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
            }
            EditorBuildSettings.scenes = buildScenes;

            // Re-open MainMenu to let player start from there
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenuScene.unity");

            EditorUtility.DisplayDialog("Sukses!", "Kedua Scene (MainMenuScene & GameScene) berhasil dibuat, dikonfigurasi, dan dimasukkan ke Build Settings! Tekan Play untuk langsung mencoba game.", "OK");
            Debug.Log("[SceneSetup] Scenes generated and linked successfully!");
        }

        private static void EnsureProjectFolders()
        {
            EnsureFolder("Assets/Scenes");
            EnsureFolder("Assets/Scripts/Core");
            EnsureFolder("Assets/Scripts/Board");
            EnsureFolder("Assets/Scripts/Player");
            EnsureFolder("Assets/Scripts/Turn");
            EnsureFolder("Assets/Scripts/Dice");
            EnsureFolder("Assets/Scripts/Quiz");
            EnsureFolder("Assets/Scripts/UI");
            EnsureFolder("Assets/Scripts/Audio");
            EnsureFolder("Assets/Prefabs/Board");
            EnsureFolder("Assets/Prefabs/Player");
            EnsureFolder("Assets/Prefabs/UI");
            EnsureFolder("Assets/ScriptableObjects/PlayerSpriteSets");
            EnsureFolder("Assets/Art/MainMenu");
            EnsureFolder("Assets/Art/Board");
            EnsureFolder("Assets/Art/Characters");
            EnsureFolder("Assets/Art/UI");
            EnsureFolder("Assets/Audio/BGM");
            EnsureFolder("Assets/Audio/SFX");
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;

            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }
                current = next;
            }
        }

        private static void CreatePlaceholderPrefabs()
        {
            CreateTilePrefab();
            CreatePlayerTokenPrefab();
            CreateStatusCardPrefab();
        }

        private static void CreateTilePrefab()
        {
            const string path = "Assets/Prefabs/Board/Tile.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            GameObject tile = new GameObject("Tile", typeof(RectTransform));
            Image img = tile.AddComponent<Image>();
            img.raycastTarget = false;
            TileView view = tile.AddComponent<TileView>();
            view.background = img;

            GameObject labelGo = new GameObject("Text", typeof(RectTransform));
            labelGo.transform.SetParent(tile.transform, false);
            var label = labelGo.AddComponent<TMPro.TextMeshProUGUI>();
            label.alignment = TMPro.TextAlignmentOptions.Center;
            label.fontSize = 14f;
            label.fontStyle = TMPro.FontStyles.Bold;
            label.color = Color.white;
            StretchRect(labelGo.GetComponent<RectTransform>());
            view.label = label;

            PrefabUtility.SaveAsPrefabAsset(tile, path);
            DestroyImmediate(tile);
        }

        private static void CreatePlayerTokenPrefab()
        {
            const string path = "Assets/Prefabs/Player/PlayerToken.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            GameObject token = new GameObject("PlayerToken", typeof(RectTransform));
            Image tokenImage = token.AddComponent<Image>();
            tokenImage.color = Color.white;
            PlayerToken playerToken = token.AddComponent<PlayerToken>();
            playerToken.tokenImage = tokenImage;

            GameObject borderGo = new GameObject("Border", typeof(RectTransform));
            borderGo.transform.SetParent(token.transform, false);
            Image border = borderGo.AddComponent<Image>();
            border.color = Color.white;
            RectTransform borderRect = borderGo.GetComponent<RectTransform>();
            borderRect.anchorMin = new Vector2(0.08f, 0.08f);
            borderRect.anchorMax = new Vector2(0.92f, 0.92f);
            borderRect.offsetMin = Vector2.zero;
            borderRect.offsetMax = Vector2.zero;
            playerToken.borderImage = border;

            PrefabUtility.SaveAsPrefabAsset(token, path);
            DestroyImmediate(token);
        }

        private static void CreateStatusCardPrefab()
        {
            const string path = "Assets/Prefabs/UI/PlayerStatusCard.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            GameObject card = new GameObject("PlayerStatusCard", typeof(RectTransform));
            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(170f, 100f);
            Image bg = card.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 0.85f);

            GameObject borderGo = new GameObject("HighlightBorder", typeof(RectTransform));
            borderGo.transform.SetParent(card.transform, false);
            Image border = borderGo.AddComponent<Image>();
            border.color = new Color(0.12f, 0.73f, 0.35f);
            StretchRect(borderGo.GetComponent<RectTransform>());

            GameObject content = new GameObject("ContentLayout", typeof(RectTransform));
            content.transform.SetParent(card.transform, false);
            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.offsetMin = new Vector2(6f, 6f);
            contentRect.offsetMax = new Vector2(-6f, -6f);
            VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;

            PlayerStatusView view = card.AddComponent<PlayerStatusView>();
            view.imageAvatarBackground = bg;
            view.imageHighlightBorder = border;
            view.labelName = CreatePrefabLabel(content.transform, "LabelName", 12f, Color.white, TMPro.FontStyles.Bold);
            view.labelTile = CreatePrefabLabel(content.transform, "LabelTile", 10f, Color.yellow, TMPro.FontStyles.Normal);
            view.labelSkulls = CreatePrefabLabel(content.transform, "LabelSkulls", 9f, Color.red, TMPro.FontStyles.Normal);
            view.labelEvolution = CreatePrefabLabel(content.transform, "LabelEvolution", 8f, Color.cyan, TMPro.FontStyles.Italic);
            view.labelStatus = CreatePrefabLabel(content.transform, "LabelStatus", 9f, Color.white, TMPro.FontStyles.Bold);

            PrefabUtility.SaveAsPrefabAsset(card, path);
            DestroyImmediate(card);
        }

        private static TMPro.TextMeshProUGUI CreatePrefabLabel(Transform parent, string name, float fontSize, Color color, TMPro.FontStyles style)
        {
            GameObject textGo = new GameObject(name, typeof(RectTransform));
            textGo.transform.SetParent(parent, false);
            var text = textGo.AddComponent<TMPro.TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.color = color;
            text.fontStyle = style;
            return text;
        }

        private static void SetupMainMenuObjects(BoardConfig board, MessageBank msg, QuizBank quiz)
        {
            // Setup Canvas
            GameObject canvasGo = new GameObject("CanvasMainMenu");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            // Setup EventSystem
            GameObject eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // Setup AudioManager / SceneLoader
            GameObject persistentGo = new GameObject("PersistentManagers");
            persistentGo.AddComponent<AudioManager>();
            persistentGo.AddComponent<SceneLoader>();

            // Background Image
            GameObject bgGo = new GameObject("Background", typeof(RectTransform));
            bgGo.transform.SetParent(canvasGo.transform, false);
            Image bgImg = bgGo.AddComponent<Image>();
            bgImg.color = new Color(0.95f, 0.66f, 0.18f); // Warm board-like placeholder
            bgImg.raycastTarget = false;
            StretchRect(bgGo.GetComponent<RectTransform>());

            GameObject boardBlurGo = new GameObject("BoardBlurPlaceholder", typeof(RectTransform));
            boardBlurGo.transform.SetParent(canvasGo.transform, false);
            Image boardBlur = boardBlurGo.AddComponent<Image>();
            boardBlur.color = new Color(1f, 0.86f, 0.35f, 0.32f);
            boardBlur.raycastTarget = false;
            PositionRect(boardBlurGo.GetComponent<RectTransform>(), Vector2.zero, new Vector2(720f, 520f));

            // Title
            GameObject titleGo = new GameObject("Title", typeof(RectTransform));
            titleGo.transform.SetParent(canvasGo.transform, false);
            var titleText = titleGo.AddComponent<TMPro.TextMeshProUGUI>();
            titleText.text = "Ular Tangga Tata Tertib\nIPB University";
            titleText.alignment = TMPro.TextAlignmentOptions.Center;
            titleText.fontSize = 38f;
            titleText.fontStyle = TMPro.FontStyles.Bold;
            titleText.color = Color.white;
            titleText.raycastTarget = false;
            titleText.enableWordWrapping = false;
            PositionRect(titleGo.GetComponent<RectTransform>(), new Vector2(0f, 95f), new Vector2(620f, 105f));

            CreateDiceDecoration(canvasGo.transform, new Vector2(0f, 215f));

            // Start Button
            GameObject btnStartGo = CreateStandardButton(canvasGo.transform, "ButtonStart", "Start Game", new Vector2(0f, -25f), new Vector2(180f, 48f));
            
            // Player Select Row
            GameObject selectRow = new GameObject("PlayerSelectRow", typeof(RectTransform));
            selectRow.transform.SetParent(canvasGo.transform, false);
            PositionRect(selectRow.GetComponent<RectTransform>(), new Vector2(0f, -92f), new Vector2(300f, 44f));

            GameObject btnDecGo = CreateStandardButton(selectRow.transform, "BtnDecrease", "<", new Vector2(-110f, 0f), new Vector2(40f, 40f));
            GameObject btnIncGo = CreateStandardButton(selectRow.transform, "BtnIncrease", ">", new Vector2(110f, 0f), new Vector2(40f, 40f));

            GameObject playerLabelGo = new GameObject("PlayerLabel");
            playerLabelGo.transform.SetParent(selectRow.transform, false);
            var playerLabel = playerLabelGo.AddComponent<TMPro.TextMeshProUGUI>();
            playerLabel.text = "Player: 4";
            playerLabel.fontSize = 24f;
            playerLabel.alignment = TMPro.TextAlignmentOptions.Center;
            playerLabel.raycastTarget = false;
            playerLabel.enableWordWrapping = false;
            PositionRect(playerLabelGo.GetComponent<RectTransform>(), Vector2.zero, new Vector2(160f, 40f));

            // Quit Button
            GameObject btnQuitGo = CreateStandardButton(canvasGo.transform, "ButtonQuit", "Quit Game", new Vector2(0f, -160f));

            // Hook MainMenuUI
            GameObject mmUiGo = new GameObject("MainMenuUI");
            MainMenuUI mmUi = mmUiGo.AddComponent<MainMenuUI>();
            mmUi.btnStartGame = btnStartGo.GetComponent<Button>();
            mmUi.btnQuitGame = btnQuitGo.GetComponent<Button>();
            mmUi.btnDecreasePlayers = btnDecGo.GetComponent<Button>();
            mmUi.btnIncreasePlayers = btnIncGo.GetComponent<Button>();
            mmUi.labelPlayerCount = playerLabel;
        }

        private static void SetupGameSceneObjects(BoardConfig board, MessageBank msg, QuizBank quiz)
        {
            // Setup Canvas
            GameObject canvasGo = new GameObject("CanvasGameplay");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            // Setup EventSystem
            GameObject eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // Main Background
            GameObject bgGo = new GameObject("Background", typeof(RectTransform));
            bgGo.transform.SetParent(canvasGo.transform, false);
            Image bgImg = bgGo.AddComponent<Image>();
            bgImg.color = new Color(0.18f, 0.18f, 0.22f);
            StretchRect(bgGo.GetComponent<RectTransform>());

            // Board Panel
            GameObject boardGo = new GameObject("BoardPanel", typeof(RectTransform));
            boardGo.transform.SetParent(canvasGo.transform, false);
            Image boardImg = boardGo.AddComponent<Image>();
            boardImg.color = new Color(0.25f, 0.25f, 0.3f);
            PositionRect(boardGo.GetComponent<RectTransform>(), new Vector2(-150f, 0f), new Vector2(600f, 600f));

            // Status Card Container
            GameObject statusGridGo = new GameObject("StatusCardGrid", typeof(RectTransform));
            statusGridGo.transform.SetParent(canvasGo.transform, false);
            PositionRect(statusGridGo.GetComponent<RectTransform>(), new Vector2(250f, 150f), new Vector2(180f, 420f));
            
            // Add grid layout
            var vGrid = statusGridGo.AddComponent<VerticalLayoutGroup>();
            vGrid.spacing = 10f;
            vGrid.childAlignment = TextAnchor.UpperCenter;
            vGrid.childForceExpandHeight = false;

            // Active Turn HUD
            GameObject turnHudGo = new GameObject("TurnHUD", typeof(RectTransform));
            turnHudGo.transform.SetParent(canvasGo.transform, false);
            PositionRect(turnHudGo.GetComponent<RectTransform>(), new Vector2(250f, -120f), new Vector2(180f, 100f));
            Image turnHudImg = turnHudGo.AddComponent<Image>();
            turnHudImg.color = new Color(0.12f, 0.12f, 0.15f, 0.8f);

            GameObject hudTitle = new GameObject("HudTitle", typeof(RectTransform));
            hudTitle.transform.SetParent(turnHudGo.transform, false);
            var hudTitleTxt = hudTitle.AddComponent<TMPro.TextMeshProUGUI>();
            hudTitleTxt.text = "GILIRAN AKTIF";
            hudTitleTxt.alignment = TMPro.TextAlignmentOptions.Center;
            hudTitleTxt.fontSize = 12f;
            hudTitleTxt.fontStyle = TMPro.FontStyles.Bold;
            PositionRect(hudTitleTxt.GetComponent<RectTransform>(), new Vector2(0f, 35f), new Vector2(170f, 20f));

            GameObject hudName = new GameObject("HudName", typeof(RectTransform));
            hudName.transform.SetParent(turnHudGo.transform, false);
            var hudNameTxt = hudName.AddComponent<TMPro.TextMeshProUGUI>();
            hudNameTxt.text = "Mahasiswa 1";
            hudNameTxt.alignment = TMPro.TextAlignmentOptions.Center;
            hudNameTxt.fontSize = 16f;
            hudNameTxt.fontStyle = TMPro.FontStyles.Bold;
            hudNameTxt.color = Color.green;
            PositionRect(hudNameTxt.GetComponent<RectTransform>(), new Vector2(0f, 10f), new Vector2(170f, 25f));

            GameObject hudTimer = new GameObject("HudTimer", typeof(RectTransform));
            hudTimer.transform.SetParent(turnHudGo.transform, false);
            var hudTimerTxt = hudTimer.AddComponent<TMPro.TextMeshProUGUI>();
            hudTimerTxt.text = "Sisa Waktu: 10s";
            hudTimerTxt.alignment = TMPro.TextAlignmentOptions.Center;
            hudTimerTxt.fontSize = 14f;
            PositionRect(hudTimerTxt.GetComponent<RectTransform>(), new Vector2(0f, -20f), new Vector2(170f, 25f));

            // Dice Gauge UI Panel
            GameObject gaugePanelGo = new GameObject("DiceGaugePanel", typeof(RectTransform));
            gaugePanelGo.transform.SetParent(canvasGo.transform, false);
            PositionRect(gaugePanelGo.GetComponent<RectTransform>(), new Vector2(250f, -240f), new Vector2(180f, 120f));
            Image gaugeImg = gaugePanelGo.AddComponent<Image>();
            gaugeImg.color = new Color(0.12f, 0.12f, 0.15f, 0.8f);

            // Slider
            GameObject sliderGo = new GameObject("GaugeSlider", typeof(RectTransform));
            sliderGo.transform.SetParent(gaugePanelGo.transform, false);
            Slider slider = sliderGo.AddComponent<Slider>();
            PositionRect(sliderGo.GetComponent<RectTransform>(), new Vector2(0f, 35f), new Vector2(160f, 15f));

            GameObject sliderBg = new GameObject("Bg", typeof(RectTransform));
            sliderBg.transform.SetParent(sliderGo.transform, false);
            sliderBg.AddComponent<Image>().color = Color.grey;
            StretchRect(sliderBg.GetComponent<RectTransform>());

            GameObject sliderFillArea = new GameObject("Fill Area", typeof(RectTransform));
            sliderFillArea.transform.SetParent(sliderGo.transform, false);
            StretchRect(sliderFillArea.GetComponent<RectTransform>());

            GameObject sliderFill = new GameObject("Fill", typeof(RectTransform));
            sliderFill.transform.SetParent(sliderFillArea.transform, false);
            var fillImg = sliderFill.AddComponent<Image>();
            fillImg.color = new Color(0.9f, 0.2f, 0.2f);
            StretchRect(sliderFill.GetComponent<RectTransform>());

            slider.fillRect = sliderFill.GetComponent<RectTransform>();
            slider.value = 0f;

            // Labels
            GameObject labelStGo = new GameObject("StatusLabel", typeof(RectTransform));
            labelStGo.transform.SetParent(gaugePanelGo.transform, false);
            var labelSt = labelStGo.AddComponent<TMPro.TextMeshProUGUI>();
            labelSt.text = "Status Percobaan";
            labelSt.alignment = TMPro.TextAlignmentOptions.Center;
            labelSt.fontSize = 10f;
            PositionRect(labelStGo.GetComponent<RectTransform>(), new Vector2(0f, 15f), new Vector2(170f, 15f));

            GameObject labelRngGo = new GameObject("RangeLabel", typeof(RectTransform));
            labelRngGo.transform.SetParent(gaugePanelGo.transform, false);
            var labelRng = labelRngGo.AddComponent<TMPro.TextMeshProUGUI>();
            labelRng.text = "Dadu: 2 - 3 (0%)";
            labelRng.alignment = TMPro.TextAlignmentOptions.Center;
            labelRng.fontSize = 10f;
            labelRng.color = Color.cyan;
            PositionRect(labelRngGo.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(170f, 15f));

            GameObject labelResGo = new GameObject("ResultLabel", typeof(RectTransform));
            labelResGo.transform.SetParent(gaugePanelGo.transform, false);
            var labelRes = labelResGo.AddComponent<TMPro.TextMeshProUGUI>();
            labelRes.text = "-";
            labelRes.alignment = TMPro.TextAlignmentOptions.Center;
            labelRes.fontSize = 18f;
            labelRes.fontStyle = TMPro.FontStyles.Bold;
            labelRes.color = Color.yellow;
            PositionRect(labelResGo.GetComponent<RectTransform>(), new Vector2(-45f, -35f), new Vector2(60f, 40f));

            // Roll Button
            GameObject rollBtnGo = CreateStandardButton(gaugePanelGo.transform, "BtnRoll", "ROLL", new Vector2(40f, -35f), new Vector2(80f, 35f));

            // Bottom Instruction Bar
            GameObject instructGo = new GameObject("InstructionHUD", typeof(RectTransform));
            instructGo.transform.SetParent(canvasGo.transform, false);
            PositionRect(instructGo.GetComponent<RectTransform>(), new Vector2(-150f, -270f), new Vector2(600f, 40f));
            var instructText = instructGo.AddComponent<TMPro.TextMeshProUGUI>();
            instructText.text = "Menunggu Giliran Mulai...";
            instructText.alignment = TMPro.TextAlignmentOptions.Center;
            instructText.fontSize = 14f;
            instructText.color = Color.white;

            // Setup Overlay Panels
            GameObject redFlashGo = new GameObject("RedFlashOverlay", typeof(RectTransform));
            redFlashGo.transform.SetParent(canvasGo.transform, false);
            Image redFlash = redFlashGo.AddComponent<Image>();
            redFlash.color = new Color(1f, 0f, 0f, 0f);
            StretchRect(redFlashGo.GetComponent<RectTransform>());

            // Normal Popup Panel
            GameObject normPopupGo = CreateCommonPopup(canvasGo.transform, "NormalPopupPanel");
            normPopupGo.SetActive(false);
            // Quiz Popup Panel
            GameObject quizPopupGo = CreateQuizPopupPanel(canvasGo.transform);
            quizPopupGo.SetActive(false);
            // Prologue Panel
            GameObject prologueGo = CreateProloguePopupPanel(canvasGo.transform);
            prologueGo.SetActive(false);
            // GameOver Panel
            GameObject gameOverGo = CreateGameOverPopupPanel(canvasGo.transform);
            gameOverGo.SetActive(false);

            // Hook Managers Game Object
            GameObject managersGo = new GameObject("GameManagers");
            
            var bManager = managersGo.AddComponent<BoardManager>();
            bManager.boardConfig = board;
            bManager.boardPanel = boardGo.GetComponent<RectTransform>();
            bManager.tilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Board/Tile.prefab");

            var evolution = managersGo.AddComponent<PlayerEvolutionController>();
            var turn = managersGo.AddComponent<TurnManager>();
            var bot = managersGo.AddComponent<BotController>();

            var dice = managersGo.AddComponent<DiceGaugeController>();
            dice.gaugeSlider = slider;
            dice.labelStatus = labelSt;
            dice.labelRange = labelRng;
            dice.labelResult = labelRes;
            dice.rollButton = rollBtnGo.GetComponent<Button>();

            var gameplayUI = managersGo.AddComponent<GameplayUI>();
            gameplayUI.labelActiveTurn = hudNameTxt;
            gameplayUI.labelTimer = hudTimerTxt;
            gameplayUI.labelInstruction = instructText;
            gameplayUI.statusCardGridContainer = statusGridGo.transform;
            gameplayUI.playerStatusCardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/PlayerStatusCard.prefab");

            var pop = managersGo.AddComponent<PopupController>();
            pop.popupPanel = normPopupGo;
            pop.labelTitle = normPopupGo.transform.Find("Title").GetComponent<TMPro.TextMeshProUGUI>();
            pop.labelMessage = normPopupGo.transform.Find("Message").GetComponent<TMPro.TextMeshProUGUI>();
            pop.btnContinue = normPopupGo.GetComponentInChildren<Button>();
            pop.redFlashOverlay = redFlash;

            var qPop = managersGo.AddComponent<QuizPopup>();
            qPop.quizPanel = quizPopupGo;
            qPop.labelQuestion = quizPopupGo.transform.Find("Question").GetComponent<TMPro.TextMeshProUGUI>();
            qPop.btnOptionA = quizPopupGo.transform.Find("Options/BtnA").GetComponent<Button>();
            qPop.btnOptionB = quizPopupGo.transform.Find("Options/BtnB").GetComponent<Button>();
            qPop.feedbackContainer = quizPopupGo.transform.Find("Feedback").gameObject;
            qPop.labelFeedbackResult = quizPopupGo.transform.Find("Feedback/ResultText").GetComponent<TMPro.TextMeshProUGUI>();
            qPop.labelFeedbackExplanations = quizPopupGo.transform.Find("Feedback/ExplainText").GetComponent<TMPro.TextMeshProUGUI>();
            qPop.btnCloseQuiz = quizPopupGo.transform.Find("Feedback/BtnClose").GetComponent<Button>();

            var pPop = managersGo.AddComponent<PrologueUI>();
            pPop.prologuePanel = prologueGo;
            pPop.labelNarration = prologueGo.transform.Find("Narration").GetComponent<TMPro.TextMeshProUGUI>();
            pPop.btnStartJourney = prologueGo.GetComponentInChildren<Button>();

            var gPop = managersGo.AddComponent<GameOverUI>();
            gPop.gameOverPanel = gameOverGo;
            gPop.labelWinnerName = gameOverGo.transform.Find("WinnerTitle").GetComponent<TMPro.TextMeshProUGUI>();
            gPop.imageWinnerAvatar = gameOverGo.transform.Find("Avatar").GetComponent<Image>();
            gPop.labelMessage = gameOverGo.transform.Find("DescText").GetComponent<TMPro.TextMeshProUGUI>();
            gPop.btnPlayAgain = gameOverGo.transform.Find("Btns/BtnAgain").GetComponent<Button>();
            gPop.btnReturnToMenu = gameOverGo.transform.Find("Btns/BtnMenu").GetComponent<Button>();

            var gm = managersGo.AddComponent<GameManager>();
            gm.boardConfig = board;
            gm.messageBank = msg;
            gm.quizBank = quiz;
            gm.boardContainer = boardGo.transform;
            gm.playerTokenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/PlayerToken.prefab");
        }

        private static GameObject CreateStandardButton(Transform parent, string name, string label, Vector2 pos, Vector2? size = null)
        {
            GameObject btnGo = new GameObject(name);
            btnGo.transform.SetParent(parent, false);
            Image img = btnGo.AddComponent<Image>();
            img.color = new Color(0.12f, 0.45f, 0.85f);
            
            Button btn = btnGo.AddComponent<Button>();

            RectTransform rTrans = btnGo.GetComponent<RectTransform>();
            rTrans.anchoredPosition = pos;
            rTrans.sizeDelta = size ?? new Vector2(160f, 40f);

            GameObject txtGo = new GameObject("Text");
            txtGo.transform.SetParent(btnGo.transform, false);
            var text = txtGo.AddComponent<TMPro.TextMeshProUGUI>();
            text.text = label;
            text.fontSize = 14f;
            text.alignment = TMPro.TextAlignmentOptions.Center;
            text.color = Color.white;
            text.raycastTarget = false;
            text.enableWordWrapping = false;

            RectTransform txtRect = txtGo.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            return btnGo;
        }

        private static void CreateDiceDecoration(Transform parent, Vector2 pos)
        {
            GameObject diceRoot = new GameObject("DiceDecoration", typeof(RectTransform));
            diceRoot.transform.SetParent(parent, false);
            PositionRect(diceRoot.GetComponent<RectTransform>(), pos, new Vector2(180f, 90f));

            CreateDieFace(diceRoot.transform, "MainDie", new Vector2(-28f, 0f), new Vector2(76f, 76f), 6f, -12f);
            CreateDieFace(diceRoot.transform, "SmallDieA", new Vector2(52f, 22f), new Vector2(42f, 42f), 4f, 18f);
            CreateDieFace(diceRoot.transform, "SmallDieB", new Vector2(88f, 42f), new Vector2(34f, 34f), 3f, 28f);
        }

        private static void CreateDieFace(Transform parent, string name, Vector2 pos, Vector2 size, float pipSize, float rotation)
        {
            GameObject die = new GameObject(name, typeof(RectTransform));
            die.transform.SetParent(parent, false);
            Image dieImage = die.AddComponent<Image>();
            dieImage.color = Color.white;
            dieImage.raycastTarget = false;

            RectTransform rect = die.GetComponent<RectTransform>();
            PositionRect(rect, pos, size);
            rect.localRotation = Quaternion.Euler(0f, 0f, rotation);

            Vector2[] pipPositions =
            {
                new Vector2(-0.25f, 0.25f),
                new Vector2(0.25f, 0.25f),
                new Vector2(-0.25f, 0f),
                new Vector2(0.25f, 0f),
                new Vector2(-0.25f, -0.25f),
                new Vector2(0.25f, -0.25f)
            };

            for (int i = 0; i < pipPositions.Length; i++)
            {
                GameObject pip = new GameObject("Pip", typeof(RectTransform));
                pip.transform.SetParent(die.transform, false);
                Image pipImage = pip.AddComponent<Image>();
                pipImage.color = Color.black;
                pipImage.raycastTarget = false;

                RectTransform pipRect = pip.GetComponent<RectTransform>();
                pipRect.anchorMin = new Vector2(0.5f, 0.5f);
                pipRect.anchorMax = new Vector2(0.5f, 0.5f);
                pipRect.sizeDelta = new Vector2(pipSize, pipSize);
                pipRect.anchoredPosition = new Vector2(size.x * pipPositions[i].x, size.y * pipPositions[i].y);
            }
        }

        private static GameObject CreateCommonPopup(Transform parent, string name)
        {
            GameObject popGo = new GameObject(name);
            popGo.transform.SetParent(parent, false);
            Image popImg = popGo.AddComponent<Image>();
            popImg.color = new Color(0.15f, 0.15f, 0.18f, 0.95f);
            PositionRect(popGo.GetComponent<RectTransform>(), Vector2.zero, new Vector2(400f, 320f));

            // Title
            GameObject titleGo = new GameObject("Title");
            titleGo.transform.SetParent(popGo.transform, false);
            var title = titleGo.AddComponent<TMPro.TextMeshProUGUI>();
            title.text = "Kegiatan Positif";
            title.fontSize = 20f;
            title.alignment = TMPro.TextAlignmentOptions.Center;
            title.color = Color.yellow;
            PositionRect(titleGo.GetComponent<RectTransform>(), new Vector2(0f, 120f), new Vector2(380f, 30f));

            // Message
            GameObject msgGo = new GameObject("Message");
            msgGo.transform.SetParent(popGo.transform, false);
            var message = msgGo.AddComponent<TMPro.TextMeshProUGUI>();
            message.text = "Pesan edukasi detail...";
            message.fontSize = 13f;
            message.alignment = TMPro.TextAlignmentOptions.Center;
            message.color = Color.white;
            PositionRect(msgGo.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(360f, 180f));

            // Button
            CreateStandardButton(popGo.transform, "BtnLanjut", "Lanjut", new Vector2(0f, -120f));

            return popGo;
        }

        private static GameObject CreateQuizPopupPanel(Transform parent)
        {
            GameObject popGo = new GameObject("QuizPopupPanel");
            popGo.transform.SetParent(parent, false);
            Image popImg = popGo.AddComponent<Image>();
            popImg.color = new Color(0.12f, 0.15f, 0.22f, 0.98f);
            PositionRect(popGo.GetComponent<RectTransform>(), Vector2.zero, new Vector2(450f, 400f));

            // Title
            GameObject titleGo = new GameObject("Title");
            titleGo.transform.SetParent(popGo.transform, false);
            var title = titleGo.AddComponent<TMPro.TextMeshProUGUI>();
            title.text = "KUIS TATA TERTIB IPB";
            title.fontSize = 22f;
            title.alignment = TMPro.TextAlignmentOptions.Center;
            title.color = Color.cyan;
            PositionRect(titleGo.GetComponent<RectTransform>(), new Vector2(0f, 160f), new Vector2(420f, 30f));

            // Question
            GameObject questGo = new GameObject("Question");
            questGo.transform.SetParent(popGo.transform, false);
            var question = questGo.AddComponent<TMPro.TextMeshProUGUI>();
            question.text = "Pertanyaan kuis...";
            question.fontSize = 14f;
            question.alignment = TMPro.TextAlignmentOptions.Center;
            question.color = Color.white;
            PositionRect(questGo.GetComponent<RectTransform>(), new Vector2(0f, 60f), new Vector2(400f, 100f));

            // Options Row
            GameObject rowGo = new GameObject("Options", typeof(RectTransform));
            rowGo.transform.SetParent(popGo.transform, false);
            PositionRect(rowGo.GetComponent<RectTransform>(), new Vector2(0f, -40f), new Vector2(400f, 50f));

            CreateStandardButton(rowGo.transform, "BtnA", "Option A / Benar", new Vector2(-105f, 0f), new Vector2(200f, 45f));
            CreateStandardButton(rowGo.transform, "BtnB", "Option B / Salah", new Vector2(105f, 0f), new Vector2(200f, 45f));

            // Feedback Card inside
            GameObject feedGo = new GameObject("Feedback", typeof(RectTransform));
            feedGo.transform.SetParent(popGo.transform, false);
            PositionRect(feedGo.GetComponent<RectTransform>(), new Vector2(0f, -40f), new Vector2(430f, 260f));
            var fImg = feedGo.AddComponent<Image>();
            fImg.color = new Color(0.08f, 0.08f, 0.12f, 0.98f);

            GameObject resGo = new GameObject("ResultText");
            resGo.transform.SetParent(feedGo.transform, false);
            var resTxt = resGo.AddComponent<TMPro.TextMeshProUGUI>();
            resTxt.text = "BENAR! *";
            resTxt.alignment = TMPro.TextAlignmentOptions.Center;
            resTxt.fontSize = 20f;
            resTxt.fontStyle = TMPro.FontStyles.Bold;
            PositionRect(resGo.GetComponent<RectTransform>(), new Vector2(0f, 90f), new Vector2(410f, 30f));

            GameObject explainGo = new GameObject("ExplainText");
            explainGo.transform.SetParent(feedGo.transform, false);
            var expTxt = explainGo.AddComponent<TMPro.TextMeshProUGUI>();
            expTxt.text = "Penjelasan edukatif detail...";
            expTxt.alignment = TMPro.TextAlignmentOptions.Center;
            expTxt.fontSize = 12f;
            expTxt.color = Color.white;
            PositionRect(explainGo.GetComponent<RectTransform>(), new Vector2(0f, 10f), new Vector2(390f, 120f));

            CreateStandardButton(feedGo.transform, "BtnClose", "Lanjut Perjalanan", new Vector2(0f, -90f));

            return popGo;
        }

        private static GameObject CreateProloguePopupPanel(Transform parent)
        {
            GameObject popGo = new GameObject("ProloguePopupPanel");
            popGo.transform.SetParent(parent, false);
            Image popImg = popGo.AddComponent<Image>();
            popImg.color = new Color(0.1f, 0.1f, 0.14f, 0.98f);
            PositionRect(popGo.GetComponent<RectTransform>(), Vector2.zero, new Vector2(500f, 400f));

            // Title
            GameObject titleGo = new GameObject("Title");
            titleGo.transform.SetParent(popGo.transform, false);
            var title = titleGo.AddComponent<TMPro.TextMeshProUGUI>();
            title.text = "PROLOG PERJALANAN KAMPUS";
            title.fontSize = 22f;
            title.alignment = TMPro.TextAlignmentOptions.Center;
            title.color = Color.yellow;
            PositionRect(titleGo.GetComponent<RectTransform>(), new Vector2(0f, 150f), new Vector2(460f, 40f));

            // Narration
            GameObject narrGo = new GameObject("Narration");
            narrGo.transform.SetParent(popGo.transform, false);
            var narration = narrGo.AddComponent<TMPro.TextMeshProUGUI>();
            narration.text = "Narasi prolog...";
            narration.fontSize = 14f;
            narration.alignment = TMPro.TextAlignmentOptions.Center;
            narration.color = Color.white;
            PositionRect(narrGo.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(440f, 220f));

            // Start Journey Button
            CreateStandardButton(popGo.transform, "BtnStartJourney", "Mulai Perjalanan", new Vector2(0f, -140f));

            return popGo;
        }

        private static GameObject CreateGameOverPopupPanel(Transform parent)
        {
            GameObject popGo = new GameObject("GameOverPopupPanel");
            popGo.transform.SetParent(parent, false);
            Image popImg = popGo.AddComponent<Image>();
            popImg.color = new Color(0.12f, 0.08f, 0.08f, 0.98f);
            PositionRect(popGo.GetComponent<RectTransform>(), Vector2.zero, new Vector2(500f, 450f));

            // Title
            GameObject titleGo = new GameObject("Title");
            titleGo.transform.SetParent(popGo.transform, false);
            var title = titleGo.AddComponent<TMPro.TextMeshProUGUI>();
            title.text = "PERMAINAN SELESAI";
            title.fontSize = 18f;
            title.alignment = TMPro.TextAlignmentOptions.Center;
            title.color = Color.yellow;
            PositionRect(titleGo.GetComponent<RectTransform>(), new Vector2(0f, 190f), new Vector2(460f, 30f));

            // Winner title
            GameObject winTitleGo = new GameObject("WinnerTitle");
            winTitleGo.transform.SetParent(popGo.transform, false);
            var winTitle = winTitleGo.AddComponent<TMPro.TextMeshProUGUI>();
            winTitle.text = "MAHASISWA 1 MENANG!";
            winTitle.alignment = TMPro.TextAlignmentOptions.Center;
            winTitle.fontSize = 24f;
            winTitle.fontStyle = TMPro.FontStyles.Bold;
            winTitle.color = Color.green;
            PositionRect(winTitleGo.GetComponent<RectTransform>(), new Vector2(0f, 150f), new Vector2(460f, 35f));

            // Winner avatar card placeholder
            GameObject avaGo = new GameObject("Avatar");
            avaGo.transform.SetParent(popGo.transform, false);
            Image avaImg = avaGo.AddComponent<Image>();
            avaImg.color = Color.white;
            PositionRect(avaGo.GetComponent<RectTransform>(), new Vector2(0f, 40f), new Vector2(100f, 100f));

            // Description
            GameObject descGo = new GameObject("DescText");
            descGo.transform.SetParent(popGo.transform, false);
            var desc = descGo.AddComponent<TMPro.TextMeshProUGUI>();
            desc.text = "Deskripsi kemenangan...";
            desc.alignment = TMPro.TextAlignmentOptions.Center;
            desc.fontSize = 13f;
            desc.color = Color.white;
            PositionRect(descGo.GetComponent<RectTransform>(), new Vector2(0f, -60f), new Vector2(440f, 60f));

            // Buttons Row
            GameObject rowGo = new GameObject("Btns", typeof(RectTransform));
            rowGo.transform.SetParent(popGo.transform, false);
            PositionRect(rowGo.GetComponent<RectTransform>(), new Vector2(0f, -160f), new Vector2(400f, 50f));

            CreateStandardButton(rowGo.transform, "BtnAgain", "Main Lagi", new Vector2(-105f, 0f));
            CreateStandardButton(rowGo.transform, "BtnMenu", "Kembali Ke Menu", new Vector2(105f, 0f));

            return popGo;
        }

        private static void StretchRect(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void PositionRect(RectTransform rect, Vector2 pos, Vector2 size)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
        }
    }
}
