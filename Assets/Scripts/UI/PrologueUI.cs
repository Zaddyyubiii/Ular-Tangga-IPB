using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PrologueUI : MonoBehaviour
    {
        public static PrologueUI Instance;

        [Header("UI Component Bindings")]
        public GameObject prologuePanel;
        public TMPro.TextMeshProUGUI labelNarration;
        public Button btnStartJourney;

        private System.Action onPrologueFinishedCallback;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (prologuePanel != null) prologuePanel.SetActive(false);
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

            prologuePanel.SetActive(true);
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
    }
}
