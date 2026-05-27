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

        private Action onContinueCallback;
        private Coroutine autoCloseCoroutine;

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
            onContinueCallback = callback;

            popupPanel.SetActive(true);

            // Pop animation (fade / scale)
            popupPanel.transform.localScale = Vector3.zero;
            StartCoroutine(PopScaleCo(popupPanel.transform, Vector3.one, 0.25f));

            if (playExplosion)
            {
                StartCoroutine(PlayBombExplosionCo());
            }

            // Trigger bot auto-close if current player is a bot
            var curPlayer = Core.GameManager.Instance != null ? Core.GameManager.Instance.GetCurrentPlayer() : null;
            if (curPlayer != null && curPlayer.isBot)
            {
                autoCloseCoroutine = StartCoroutine(AutoCloseCo(UnityEngine.Random.Range(0.8f, 1.5f), curPlayer.playerName));
            }
        }

        private IEnumerator AutoCloseCo(float delay, string botName)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log($"Bot Player {botName} auto closed popup");
            OnContinueClicked();
        }

        private void OnContinueClicked()
        {
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.clickClip);
            }

            popupPanel.SetActive(false);
            if (bombPlaceholderGo != null) bombPlaceholderGo.SetActive(false);

            // Trigger callback
            onContinueCallback?.Invoke();
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
