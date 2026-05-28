using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [System.Serializable]
    public class ReactPrologueState
    {
        public string narrationText;
    }

    public class PrologueUI : MonoBehaviour
    {
        public static PrologueUI Instance;

        #if UNITY_WEBGL && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void ShowPrologueToReact(string prologueJson);
        #else
        private static void ShowPrologueToReact(string prologueJson) { }
        #endif

        [Header("UI Component Bindings")]
        public GameObject prologuePanel;
        public TMPro.TextMeshProUGUI labelNarration;
        public Button btnStartJourney;

        private System.Action onPrologueFinishedCallback;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (prologuePanel != null) prologuePanel.SetActive(false);
        }

        private void Start()
        {
            if (btnStartJourney != null)
            {
                btnStartJourney.onClick.AddListener(OnStartJourneyClicked);
            }
        }

        public void ShowPrologue(string text, System.Action callback)
        {
            onPrologueFinishedCallback = callback;
            if (labelNarration != null)
            {
                labelNarration.text = text;
            }

            ReactPrologueState rPrologue = new ReactPrologueState();
            rPrologue.narrationText = text;
            ShowPrologueToReact(JsonUtility.ToJson(rPrologue));
            if (labelNarration != null)
            {
                labelNarration.text = text;
            }

            #if UNITY_WEBGL && !UNITY_EDITOR
            // React handles narrative overlay
            #else
            prologuePanel.SetActive(true);
            #endif
            Debug.Log("[Prologue] Showing prologue overlay.");
        }

        private void OnStartJourneyClicked()
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.clickClip);
            }

            prologuePanel.SetActive(false);
            Debug.Log("[Prologue] Prologue dismissed, starting journey!");
            onPrologueFinishedCallback?.Invoke();
        }

        public void ClosePrologueFromReact()
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            // React handles the display
            #else
            if (prologuePanel != null) prologuePanel.SetActive(false);
            #endif
            
            Debug.Log("[Prologue] Prologue dismissed from React, starting journey!");
            onPrologueFinishedCallback?.Invoke();
        }
    }
}
