using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI Controls")]
        public TMPro.TextMeshProUGUI labelPlayerCount;
        public Button btnDecreasePlayers;
        public Button btnIncreasePlayers;
        public Button btnStartGame;
        public Button btnQuitGame;

        private int playerCountSelected = 4; // Default to 4

        [Header("Dynamic Name Inputs Container")]
        private GameObject nameInputsContainer;
        private System.Collections.Generic.List<TMPro.TMP_InputField> nameInputFields = new System.Collections.Generic.List<TMPro.TMP_InputField>();

        private void Awake()
        {
            RepairGeneratedLayout();
        }

        private void Start()
        {
            // Bind buttons
            if (btnDecreasePlayers != null) btnDecreasePlayers.onClick.AddListener(DecreasePlayers);
            if (btnIncreasePlayers != null) btnIncreasePlayers.onClick.AddListener(IncreasePlayers);
            if (btnStartGame != null) btnStartGame.onClick.AddListener(StartGame);
            if (btnQuitGame != null) btnQuitGame.onClick.AddListener(QuitGame);

            // Hide Quit button on WebGL
#if UNITY_WEBGL
            if (btnQuitGame != null) btnQuitGame.gameObject.SetActive(false);
#endif

            UpdatePlayerLabel();
            RefreshNameInputFields();

            // Play background music (safe)
            if (Audio.AudioManager.Instance != null && Audio.AudioManager.Instance.mainMenuBGM != null)
            {
                Audio.AudioManager.Instance.PlayBGM(Audio.AudioManager.Instance.mainMenuBGM);
            }
        }

        private void DecreasePlayers()
        {
            PlayClickSound();
            if (playerCountSelected > 1)
            {
                playerCountSelected--;
                UpdatePlayerLabel();
                RefreshNameInputFields();
            }
        }

        private void IncreasePlayers()
        {
            PlayClickSound();
            if (playerCountSelected < 4)
            {
                playerCountSelected++;
                UpdatePlayerLabel();
                RefreshNameInputFields();
            }
        }

        private void UpdatePlayerLabel()
        {
            if (labelPlayerCount != null)
            {
                labelPlayerCount.text = playerCountSelected.ToString();
            }
        }

        private void RefreshNameInputFields()
        {
            Transform container = null;
            if (btnStartGame != null)
            {
                container = btnStartGame.transform.parent;
            }
            if (container == null)
            {
                Canvas canvas = FindAnyObjectByType<Canvas>();
                if (canvas != null)
                {
                    container = canvas.transform.Find("CenterCard");
                }
            }
            if (container == null) return;

            // 1. Clean up old container if it exists
            Transform oldContainer = container.Find("NameInputsContainer");
            if (oldContainer != null)
            {
                DestroyImmediate(oldContainer.gameObject);
            }

            // 2. Create the NameInputsContainer under CenterCard (container)
            nameInputsContainer = new GameObject("NameInputsContainer");
            nameInputsContainer.transform.SetParent(container, false);
            
            RectTransform panelRect = nameInputsContainer.AddComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(350f, 100f);

            // Add VerticalLayoutGroup to inputs panel for the text fields
            VerticalLayoutGroup vPanel = nameInputsContainer.AddComponent<VerticalLayoutGroup>();
            vPanel.spacing = 10f;
            vPanel.childAlignment = TextAnchor.MiddleCenter;
            vPanel.childControlWidth = false;
            vPanel.childControlHeight = false;
            vPanel.childForceExpandWidth = false;
            vPanel.childForceExpandHeight = false;

            // Add ContentSizeFitter so the inputs panel height grows/shrinks dynamically
            ContentSizeFitter panelFitter = nameInputsContainer.AddComponent<ContentSizeFitter>();
            panelFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            panelFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            nameInputFields.Clear();

            // 3. Spawn the custom name input fields (centered in VerticalLayoutGroup)
            for (int i = 0; i < playerCountSelected; i++)
            {
                GameObject inputGo = CreateProceduralInputField(
                    nameInputsContainer.transform, 
                    $"Nama Pemain {i + 1}...", 
                    Vector2.zero, 
                    new Vector2(350f, 42f) // Beautiful standard size for inputs
                );

                var inputField = inputGo.GetComponent<TMPro.TMP_InputField>();
                nameInputFields.Add(inputField);
            }

            // 4. Ensure CenterCard (container) layout components are set up
            VerticalLayoutGroup mainLayout = container.gameObject.GetComponent<VerticalLayoutGroup>();
            if (mainLayout == null)
            {
                mainLayout = container.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            mainLayout.childAlignment = TextAnchor.MiddleCenter;
            mainLayout.spacing = 18f;
            mainLayout.padding = new RectOffset(40, 40, 40, 40);
            mainLayout.childControlWidth = false;
            mainLayout.childControlHeight = false;
            mainLayout.childForceExpandWidth = false;
            mainLayout.childForceExpandHeight = false;

            ContentSizeFitter mainFitter = container.gameObject.GetComponent<ContentSizeFitter>();
            if (mainFitter == null)
            {
                mainFitter = container.gameObject.AddComponent<ContentSizeFitter>();
            }
            mainFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            mainFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // 5. Exclude top decorative line from layout
            Transform cardTopLine = container.Find("CardTopLine");
            if (cardTopLine != null)
            {
                LayoutElement le = cardTopLine.gameObject.GetComponent<LayoutElement>();
                if (le == null) le = cardTopLine.gameObject.AddComponent<LayoutElement>();
                le.ignoreLayout = true;
                
                RectTransform lineRect = cardTopLine as RectTransform;
                lineRect.anchorMin = new Vector2(0f, 1f);
                lineRect.anchorMax = new Vector2(1f, 1f);
                lineRect.pivot = new Vector2(0.5f, 1f);
                lineRect.anchoredPosition = Vector2.zero;
                lineRect.sizeDelta = new Vector2(0f, 4f);
            }

            // 6. Set Sibling Indices to enforce order from top to bottom
            Transform ipbBadge = container.Find("IPBBadge");
            Transform diceDecoration = container.Find("DiceDecoration");
            Transform title = container.Find("Title");
            Transform playerSelectRow = container.Find("PlayerSelectRow");
            Transform btnQuit = btnQuitGame != null ? btnQuitGame.transform : null;

            int siblingIdx = 0;
            if (ipbBadge != null) ipbBadge.SetSiblingIndex(siblingIdx++);
            if (diceDecoration != null) diceDecoration.SetSiblingIndex(siblingIdx++);
            if (title != null) title.SetSiblingIndex(siblingIdx++);
            if (playerSelectRow != null) playerSelectRow.SetSiblingIndex(siblingIdx++);
            nameInputsContainer.transform.SetSiblingIndex(siblingIdx++);
            if (btnStartGame != null) btnStartGame.transform.SetSiblingIndex(siblingIdx++);
            if (btnQuit != null) btnQuit.SetSiblingIndex(siblingIdx++);

            // 7. Rebuild layouts immediately
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(container as RectTransform);

            Debug.Log($"Refreshing name inputs. Human player count: {playerCountSelected}");
            Debug.Log("Main menu layout rebuilt after player name input refresh.");
        }

        private GameObject CreateProceduralInputField(Transform parent, string placeholderText, Vector2 pos, Vector2 size)
        {
            GameObject inputGo = new GameObject("InputField_Player");
            inputGo.transform.SetParent(parent, false);

            RectTransform rect = inputGo.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;

            Image bgImage = inputGo.AddComponent<Image>();
            bgImage.color = new Color(0.12f, 0.12f, 0.16f, 0.95f); // Beautiful dark card slate color

            // Create Text Area
            GameObject textArea = new GameObject("TextArea");
            textArea.transform.SetParent(inputGo.transform, false);
            RectTransform textRect = textArea.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10f, 3f);
            textRect.offsetMax = new Vector2(-10f, -3f);

            // Create Placeholder Text
            GameObject placeholderGo = new GameObject("Placeholder");
            placeholderGo.transform.SetParent(textArea.transform, false);
            TMPro.TextMeshProUGUI placeholderTxt = placeholderGo.AddComponent<TMPro.TextMeshProUGUI>();
            placeholderTxt.text = placeholderText;
            placeholderTxt.fontSize = 13f;
            placeholderTxt.color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
            placeholderTxt.alignment = TMPro.TextAlignmentOptions.Left;
            placeholderTxt.raycastTarget = false;
            placeholderTxt.textWrappingMode = TMPro.TextWrappingModes.NoWrap;

            RectTransform placeholderRect = placeholderGo.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = Vector2.zero;
            placeholderRect.offsetMax = Vector2.zero;

            // Create Text Component
            GameObject textGo = new GameObject("Text");
            textGo.transform.SetParent(textArea.transform, false);
            TMPro.TextMeshProUGUI textComponent = textGo.AddComponent<TMPro.TextMeshProUGUI>();
            textComponent.fontSize = 13f;
            textComponent.color = Color.white;
            textComponent.alignment = TMPro.TextAlignmentOptions.Left;
            textComponent.raycastTarget = false;
            textComponent.textWrappingMode = TMPro.TextWrappingModes.NoWrap;

            RectTransform textComponentRect = textGo.GetComponent<RectTransform>();
            textComponentRect.anchorMin = Vector2.zero;
            textComponentRect.anchorMax = Vector2.one;
            textComponentRect.offsetMin = Vector2.zero;
            textComponentRect.offsetMax = Vector2.zero;

            // TMP Input Field Setup
            TMPro.TMP_InputField inputField = inputGo.AddComponent<TMPro.TMP_InputField>();
            inputField.textViewport = textRect;
            inputField.textComponent = textComponent;
            inputField.placeholder = placeholderTxt;
            inputField.characterLimit = 14;

            return inputGo;
        }

        private void StartGame()
        {
            PlayClickSound();
            Debug.Log($"[MainMenu] Starting game with {playerCountSelected} real players");

            // Sanitize and save names
            GameSetup.HumanPlayerCount = playerCountSelected;
            for (int i = 0; i < 4; i++)
            {
                if (i < playerCountSelected)
                {
                    string inputName = nameInputFields[i] != null ? nameInputFields[i].text : "";
                    GameSetup.PlayerNames[i] = SanitizePlayerName(inputName, i, false);
                }
                else
                {
                    GameSetup.PlayerNames[i] = SanitizePlayerName("", i, true);
                }
                Debug.Log($"Player {i + 1} name set to: {GameSetup.PlayerNames[i]}");
            }

            // Store selection globally in PlayerPrefs
            Core.GameManager.numRealPlayers = playerCountSelected;
            PlayerPrefs.SetInt("NumRealPlayers", playerCountSelected);
            for (int i = 0; i < 4; i++)
            {
                PlayerPrefs.SetString($"PlayerName_{i}", GameSetup.PlayerNames[i]);
            }
            PlayerPrefs.Save();

            // Load GameScene
            Core.SceneLoader.Instance.LoadScene("GameScene");
        }

        private string SanitizePlayerName(string input, int playerIndex, bool isBot)
        {
            string trimmed = input.Trim();

            if (string.IsNullOrEmpty(trimmed))
            {
                return isBot ? $"Bot {playerIndex + 1}" : $"Mahasiswa {playerIndex + 1}";
            }

            if (trimmed.Length > 14)
            {
                trimmed = trimmed.Substring(0, 14);
            }

            return trimmed;
        }

        private void QuitGame()
        {
            PlayClickSound();
            Debug.Log("[MainMenu] Quitting Game...");
            Application.Quit();
        }

        private void PlayClickSound()
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.clickClip);
            }
        }

        private void RepairGeneratedLayout()
        {
            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null) return;

            foreach (TMPro.TextMeshProUGUI text in canvas.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
            {
                // Text should not steal pointer hits from nearby buttons.
                text.raycastTarget = false;
            }

            Transform container = null;
            if (btnStartGame != null)
            {
                container = btnStartGame.transform.parent;
            }
            if (container == null)
            {
                container = canvas.transform.Find("CenterCard");
            }

            if (container != null)
            {
                // 1. Exclude top decorative line from layout
                Transform cardTopLine = container.Find("CardTopLine");
                if (cardTopLine != null)
                {
                    LayoutElement le = cardTopLine.gameObject.GetComponent<LayoutElement>();
                    if (le == null) le = cardTopLine.gameObject.AddComponent<LayoutElement>();
                    le.ignoreLayout = true;
                    
                    RectTransform lineRect = cardTopLine as RectTransform;
                    lineRect.anchorMin = new Vector2(0f, 1f);
                    lineRect.anchorMax = new Vector2(1f, 1f);
                    lineRect.pivot = new Vector2(0.5f, 1f);
                    lineRect.anchoredPosition = Vector2.zero;
                    lineRect.sizeDelta = new Vector2(0f, 4f);
                }

                // 2. Configure Title Text
                Transform title = container.Find("Title");
                if (title != null)
                {
                    TMPro.TextMeshProUGUI titleText = title.GetComponent<TMPro.TextMeshProUGUI>();
                    if (titleText != null)
                    {
                        titleText.fontSize = 38f;
                        titleText.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
                        titleText.alignment = TMPro.TextAlignmentOptions.Center;
                    }
                    
                    RectTransform titleRect = title as RectTransform;
                    titleRect.sizeDelta = new Vector2(460f, 100f);
                }

                // 3. Configure DiceDecoration
                Transform dadu = container.Find("DiceDecoration");
                if (dadu != null)
                {
                    RectTransform daduRect = dadu as RectTransform;
                    daduRect.sizeDelta = new Vector2(180f, 90f);
                }

                // 4. Configure select row size
                Transform selectRow = container.Find("PlayerSelectRow");
                if (selectRow != null)
                {
                    RectTransform selectRect = selectRow as RectTransform;
                    selectRect.sizeDelta = new Vector2(300f, 44f);
                }

                // 5. Configure Button sizes
                if (btnStartGame != null)
                {
                    RectTransform startRect = btnStartGame.GetComponent<RectTransform>();
                    startRect.sizeDelta = new Vector2(240f, 48f);
                }
                if (btnQuitGame != null)
                {
                    RectTransform quitRect = btnQuitGame.GetComponent<RectTransform>();
                    quitRect.sizeDelta = new Vector2(200f, 44f);
                }
            }
        }
    }
}
