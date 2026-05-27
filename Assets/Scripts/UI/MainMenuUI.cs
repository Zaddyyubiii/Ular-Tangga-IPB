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
            if (nameInputsContainer != null)
            {
                Destroy(nameInputsContainer);
            }

            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null) return;

            nameInputsContainer = new GameObject("NameInputsContainer");
            nameInputsContainer.transform.SetParent(canvas.transform, false);
            RectTransform containerRect = nameInputsContainer.AddComponent<RectTransform>();
            containerRect.anchoredPosition = new Vector2(0f, -40f);
            containerRect.sizeDelta = new Vector2(350f, 150f);

            nameInputFields.Clear();

            float startY = (playerCountSelected - 1) * 17.5f; // Center inputs vertically
            for (int i = 0; i < playerCountSelected; i++)
            {
                float yPos = startY - (i * 35f);
                GameObject inputGo = CreateProceduralInputField(
                    nameInputsContainer.transform, 
                    $"Nama Pemain {i + 1}...", 
                    new Vector2(0f, yPos), 
                    new Vector2(250f, 30f)
                );

                var inputField = inputGo.GetComponent<TMPro.TMP_InputField>();
                nameInputFields.Add(inputField);
            }

            // Adjust vertical positions of buttons dynamically to keep layout beautiful
            float rowOffset = playerCountSelected * 17.5f;
            MoveRect(canvas.transform.Find("PlayerSelectRow") as RectTransform, new Vector2(0f, 95f), new Vector2(300f, 44f));
            MoveRect(canvas.transform.Find("ButtonStart") as RectTransform, new Vector2(0f, -45f - rowOffset), new Vector2(180f, 48f));
            MoveRect(canvas.transform.Find("ButtonQuit") as RectTransform, new Vector2(0f, -105f - rowOffset), new Vector2(160f, 40f));
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
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null) return;

            foreach (TMPro.TextMeshProUGUI text in canvas.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
            {
                // Text should not steal pointer hits from nearby buttons.
                text.raycastTarget = false;
            }

            RectTransform title = canvas.transform.Find("Title") as RectTransform;
            if (title != null)
            {
                title.anchoredPosition = new Vector2(0f, 95f);
                title.sizeDelta = new Vector2(620f, 105f);

                TMPro.TextMeshProUGUI titleText = title.GetComponent<TMPro.TextMeshProUGUI>();
                if (titleText != null)
                {
                    titleText.fontSize = 38f;
                    titleText.enableWordWrapping = false;
                }
            }

            RectTransform dadu = canvas.transform.Find("DaduDecoration") as RectTransform;
            if (dadu != null)
            {
                dadu.anchoredPosition = new Vector2(0f, 215f);
                dadu.sizeDelta = new Vector2(260f, 80f);

                TMPro.TextMeshProUGUI daduText = dadu.GetComponent<TMPro.TextMeshProUGUI>();
                if (daduText != null)
                {
                    daduText.text = "DADU";
                    daduText.fontSize = 42f;
                    daduText.enableWordWrapping = false;
                }
            }

            MoveRect(canvas.transform.Find("ButtonStart") as RectTransform, new Vector2(0f, -25f), new Vector2(180f, 48f));
            MoveRect(canvas.transform.Find("PlayerSelectRow") as RectTransform, new Vector2(0f, -92f), new Vector2(300f, 44f));
            MoveRect(canvas.transform.Find("ButtonQuit") as RectTransform, new Vector2(0f, -160f), new Vector2(160f, 40f));
        }

        private void MoveRect(RectTransform rect, Vector2 position, Vector2 size)
        {
            if (rect == null) return;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }
    }
}
