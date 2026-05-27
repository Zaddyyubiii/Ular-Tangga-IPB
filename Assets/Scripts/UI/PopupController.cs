using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PopupController : MonoBehaviour
    {
        public static PopupController Instance;

        [Header("UI Panel Containers")]
        public GameObject popupPanel;
        public TMPro.TextMeshProUGUI labelTitle;
        public TMPro.TextMeshProUGUI labelMessage;
        public Button btnContinue;

        [Header("Animation Overlays")]
        public Image redFlashOverlay;
        public GameObject bombPlaceholderGo; // Procedural bomb card/symbol

        public const float POPUP_AUTO_CLOSE_DELAY = 5f;

        private Action onContinueCallback;
        private Coroutine autoCloseCoroutine;
        private bool isPopupOpen;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (popupPanel != null) popupPanel.SetActive(false);
            if (redFlashOverlay != null) redFlashOverlay.gameObject.SetActive(false);
            if (bombPlaceholderGo != null) bombPlaceholderGo.SetActive(false);

            if (btnContinue != null)
            {
                btnContinue.onClick.AddListener(OnContinueClicked);
            }
        }

        public void ShowPopup(string title, string message, Action callback, bool playExplosion = false)
        {
            var curPlayer = Core.GameManager.Instance != null ? Core.GameManager.Instance.GetCurrentPlayer() : null;
            bool showContinue = curPlayer == null || !curPlayer.isBot;
            ShowPopup(title, message, callback, showContinue, POPUP_AUTO_CLOSE_DELAY, playExplosion);
        }

        public void ShowPopup(
            string title,
            string message,
            Action onClose,
            bool showContinueButton = true,
            float autoCloseDelay = -1f,
            bool playExplosion = false
        )
        {
            if (autoCloseDelay <= 0f)
            {
                autoCloseDelay = POPUP_AUTO_CLOSE_DELAY;
            }

            if (autoCloseCoroutine != null)
            {
                StopCoroutine(autoCloseCoroutine);
                autoCloseCoroutine = null;
            }

            // Pause turn timer during popup
            if (Turn.TurnManager.Instance != null)
            {
                Turn.TurnManager.Instance.StopTimer();
            }

            labelTitle.text = title;
            labelMessage.text = message;
            onContinueCallback = onClose;

            isPopupOpen = true;
            popupPanel.SetActive(true);

            // Toggle Continue button based on parameters
            if (btnContinue != null)
            {
                btnContinue.gameObject.SetActive(showContinueButton);
            }

            // Pop animation (fade / scale)
            popupPanel.transform.localScale = Vector3.zero;
            StartCoroutine(PopScaleCo(popupPanel.transform, Vector3.one, 0.25f));

            if (playExplosion)
            {
                StartCoroutine(PlayBombExplosionCo());
            }

            var curPlayer = Core.GameManager.Instance != null ? Core.GameManager.Instance.GetCurrentPlayer() : null;
            bool isBot = curPlayer != null && curPlayer.isBot;

            // Logs matching section 11
            Debug.Log($"Popup opened: {title}. Auto close in {autoCloseDelay} seconds.");
            if (isBot)
            {
                Debug.Log("Bot popup will auto close in 5 seconds.");
            }

            autoCloseCoroutine = StartCoroutine(AutoCloseRoutine(autoCloseDelay));
        }

        private IEnumerator AutoCloseRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log("Popup auto closed after 5 seconds.");
            ClosePopup();
        }

        private void OnContinueClicked()
        {
            Debug.Log("Popup closed manually before auto close.");
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.clickClip);
            }
            ClosePopup();
        }

        public void ClosePopup()
        {
            if (!isPopupOpen) return;
            isPopupOpen = false;

            if (autoCloseCoroutine != null)
            {
                StopCoroutine(autoCloseCoroutine);
                autoCloseCoroutine = null;
            }

            if (popupPanel != null) popupPanel.SetActive(false);
            if (bombPlaceholderGo != null) bombPlaceholderGo.SetActive(false);

            // Reset scale if it was shaking or animating
            if (popupPanel != null) popupPanel.transform.localPosition = Vector3.zero;

            Action callback = onContinueCallback;
            onContinueCallback = null;

            Debug.Log("Popup onClose callback executed.");
            callback?.Invoke();
        }

        private IEnumerator PopScaleCo(Transform trans, Vector3 targetScale, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                trans.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsed / duration);
                yield return null;
            }
            trans.localScale = targetScale;
        }

        private IEnumerator PlayBombExplosionCo()
        {
            // Play explosion SFX (safe check)
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.skullClip);
            }

            if (popupPanel == null) yield break;

            // Create procedural bomb symbol if null
            if (bombPlaceholderGo == null)
            {
                bombPlaceholderGo = CreateProceduralBombCard();
            }

            bombPlaceholderGo.SetActive(true);
            bombPlaceholderGo.transform.localScale = Vector3.zero;
            if (redFlashOverlay != null)
            {
                redFlashOverlay.gameObject.SetActive(true);
            }

            float duration = 0.5f;
            float elapsed = 0f;

            // Flash overlay and pop bomb emoji
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                bombPlaceholderGo.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.5f, t);
                
                // Pulsate and fade red overlay
                float alpha = Mathf.PingPong(elapsed * 4f, 0.7f);
                if (redFlashOverlay != null)
                {
                    redFlashOverlay.color = new Color(1f, 0f, 0f, alpha);
                }

                // Shake popup panel slightly
                if (popupPanel != null)
                {
                    popupPanel.transform.localPosition = new Vector3(
                        UnityEngine.Random.Range(-10f, 10f),
                        UnityEngine.Random.Range(-10f, 10f),
                        0f
                    );
                }

                yield return null;
            }

            // Settle down
            if (popupPanel != null) popupPanel.transform.localPosition = Vector3.zero;
            bombPlaceholderGo.transform.localScale = Vector3.one;

            // Fade out red overlay
            elapsed = 0f;
            while (elapsed < 0.2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / 0.2f;
                if (redFlashOverlay != null)
                {
                    redFlashOverlay.color = new Color(1f, 0f, 0f, Mathf.Lerp(0.5f, 0f, t));
                }
                yield return null;
            }
            if (redFlashOverlay != null)
            {
                redFlashOverlay.gameObject.SetActive(false);
            }
        }

        private GameObject CreateProceduralBombCard()
        {
            GameObject cardGo = new GameObject("ProceduralBombCard");
            cardGo.transform.SetParent(popupPanel.transform, false);
            cardGo.transform.SetSiblingIndex(1); // below texts

            RectTransform rect = cardGo.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100f, 100f);
            rect.anchoredPosition = new Vector2(0f, 80f);

            Image img = cardGo.AddComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            // Add Skull text symbol inside
            GameObject txtGo = new GameObject("SkullSymbol");
            txtGo.transform.SetParent(cardGo.transform, false);
            TMPro.TextMeshProUGUI text = txtGo.AddComponent<TMPro.TextMeshProUGUI>();
            text.text = "💥☠️💥";
            text.alignment = TMPro.TextAlignmentOptions.Center;
            text.fontSize = 40f;

            RectTransform txtRect = txtGo.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            return cardGo;
        }
    }
}
