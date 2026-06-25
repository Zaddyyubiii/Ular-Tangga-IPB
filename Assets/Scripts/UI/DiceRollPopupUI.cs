using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Player;
using Dice;

namespace UI
{
    public class DiceRollPopupUI : MonoBehaviour
    {
        public static DiceRollPopupUI Instance;

        [Header("UI Root & Animators")]
        public GameObject root;
        public CanvasGroup canvasGroup;
        public RectTransform cardTransform;

        [Header("Visual Components")]
        public Image cardBackground;
        public Image accentBorder;
        public TMP_Text playerNameText;
        public TMP_Text mainText;
        public TMP_Text diceNumberText;
        public TMP_Text timingText;
        public TMP_Text chargeText;
        public GameObject loadingIndicator;
        public Image diceIcon;

        [Header("Duration Settings")]
        public float resultDuration = 2f;
        public float botRollingDuration = 1f;
        public float animationDuration = 0.2f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                ValidateReferences();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void ValidateReferences()
        {
            if (root == null) Debug.LogError("[DiceRollPopupUI] Root GameObject is not assigned.");
            if (canvasGroup == null) Debug.LogError("[DiceRollPopupUI] CanvasGroup is not assigned.");
            if (cardTransform == null) Debug.LogError("[DiceRollPopupUI] Card Transform is not assigned.");
            if (playerNameText == null) Debug.LogError("[DiceRollPopupUI] PlayerNameText is not assigned.");
            if (mainText == null) Debug.LogError("[DiceRollPopupUI] MainText is not assigned.");
            if (diceNumberText == null) Debug.LogError("[DiceRollPopupUI] DiceNumberText is not assigned.");
        }

        private void Start()
        {
            if (root != null)
            {
                root.SetActive(false); // Hide at start
            }
            ConfigureLayout();
        }

        private void ConfigureLayout()
        {
            if (cardTransform != null)
            {
                // Align to bottom-center, exactly above the dice gauge
                cardTransform.anchorMin = new Vector2(0.5f, 0f);
                cardTransform.anchorMax = new Vector2(0.5f, 0f);
                cardTransform.pivot = new Vector2(0.5f, 0f);
                cardTransform.anchoredPosition = new Vector2(0f, 85f);
                cardTransform.sizeDelta = new Vector2(300f, 160f);
            }
        }

        private void SetVisualsActive(bool active)
        {
            if (cardBackground != null) cardBackground.enabled = active;
            if (accentBorder != null && accentBorder != cardBackground) accentBorder.enabled = active;
            
            var outline = cardBackground != null ? cardBackground.GetComponent<Outline>() : null;
            if (outline != null) outline.enabled = active;

            if (playerNameText != null) playerNameText.gameObject.SetActive(active);
            if (mainText != null) mainText.gameObject.SetActive(active);
            if (diceNumberText != null) diceNumberText.gameObject.SetActive(active);
            if (timingText != null) timingText.gameObject.SetActive(active);
            if (chargeText != null) chargeText.gameObject.SetActive(active);
            if (loadingIndicator != null) loadingIndicator.SetActive(active && loadingIndicator.activeSelf);
            if (diceIcon != null) diceIcon.gameObject.SetActive(active);
        }

        public IEnumerator ShowBotRollingIndicator(PlayerData bot)
        {
            Debug.Log($"Showing BOT rolling indicator for {bot.playerName}");
            
            // Sync with React overlay in parallel
            if (GameplayUI.Instance != null)
            {
                GameplayUI.Instance.ShowDiceRollingIndicator(bot);
            }

            SetupBase(bot);

            #if !(UNITY_WEBGL && !UNITY_EDITOR)
            if (mainText != null) mainText.text = $"{bot.playerName} sedang melempar dadu...";
            if (diceNumberText != null) diceNumberText.gameObject.SetActive(false);
            if (timingText != null) timingText.gameObject.SetActive(false);
            if (chargeText != null) chargeText.gameObject.SetActive(false);
            if (loadingIndicator != null) loadingIndicator.SetActive(true);
            #endif

            yield return AnimateIn();

            yield return new WaitForSeconds(botRollingDuration);

            yield return AnimateOut();

            if (root != null) root.SetActive(false);
            
            Debug.Log($"Bot rolling indicator finished for {bot.playerName}");
        }

        public IEnumerator ShowDiceResult(PlayerData player, int val, string timing, float charge)
        {
            Debug.Log($"Showing dice result popup for {player.playerName}: {val}");

            // Sync with React overlay in parallel
            if (GameplayUI.Instance != null)
            {
                GameplayUI.Instance.ShowDiceResult(player, val, timing, charge);
            }

            SetupBase(player);

            #if !(UNITY_WEBGL && !UNITY_EDITOR)
            if (mainText != null) mainText.text = $"{player.playerName} mendapatkan";
            
            if (diceNumberText != null)
            {
                diceNumberText.gameObject.SetActive(true);
                diceNumberText.text = val.ToString();
            }

            if (timingText != null)
            {
                if (!string.IsNullOrEmpty(timing))
                {
                    timingText.gameObject.SetActive(true);
                    timingText.text = timing;
                    if (timing.Contains("Perfect")) timingText.color = new Color(1f, 0.85f, 0.15f); // Beautiful Gold
                    else if (timing.Contains("Good")) timingText.color = new Color(0.2f, 0.95f, 0.4f); // Neon Green
                    else timingText.color = Color.white;
                }
                else
                {
                    timingText.gameObject.SetActive(false);
                }
            }

            if (chargeText != null)
            {
                if (charge > 0f)
                {
                    chargeText.gameObject.SetActive(true);
                    chargeText.text = $"Charge: {Mathf.RoundToInt(charge)}%";
                }
                else
                {
                    chargeText.gameObject.SetActive(false);
                }
            }

            if (loadingIndicator != null) loadingIndicator.SetActive(false);
            #endif

            yield return AnimateIn();

            yield return new WaitForSeconds(resultDuration);

            yield return AnimateOut();

            if (root != null) root.SetActive(false);

            Debug.Log($"Dice result popup finished for {player.playerName}");
        }

        public IEnumerator ShowDiceResult(PlayerData player, DiceResult result)
        {
            yield return ShowDiceResult(player, result.value, result.timingQuality, result.chargePercent);
        }

        public IEnumerator ShowDiceResult(PlayerData player, int diceValue)
        {
            yield return ShowDiceResult(player, diceValue, "", 0f);
        }

        public void ForceHide()
        {
            if (root != null) root.SetActive(false);
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            if (GameplayUI.Instance != null)
            {
                GameplayUI.Instance.ClearDiceResult();
            }
        }

        private void SetupBase(PlayerData player)
        {
            if (root != null) root.SetActive(true);

            #if UNITY_WEBGL && !UNITY_EDITOR
            SetVisualsActive(false);
            #else
            SetVisualsActive(true);

            // Dark card / black navy style:
            Color bgCol = new Color(0.08f, 0.09f, 0.16f); // Slate-900 / dark navy (#141729 or #0d0f1a)
            Color borderCol = player.playerColor;

            if (cardBackground != null)
            {
                cardBackground.color = bgCol;
                
                var outline = cardBackground.GetComponent<Outline>();
                if (outline == null) outline = cardBackground.gameObject.AddComponent<Outline>();
                outline.effectColor = borderCol;
                outline.effectDistance = new Vector2(4f, 4f);
                outline.enabled = true;
            }

            if (playerNameText != null)
            {
                playerNameText.text = player.playerName;
                playerNameText.color = borderCol; // Signature color
            }

            if (mainText != null)
            {
                mainText.color = Color.white;
            }

            if (diceNumberText != null)
            {
                diceNumberText.color = Color.yellow;
            }

            if (timingText != null)
            {
                // we keep colors for Perfect (Gold), Good (Green), Normal (White)
                if (timingText.text.Contains("Perfect")) timingText.color = new Color(1f, 0.85f, 0.15f);
                else if (timingText.text.Contains("Good")) timingText.color = new Color(0.2f, 0.95f, 0.4f);
                else timingText.color = Color.white;
            }

            if (chargeText != null)
            {
                chargeText.color = new Color(0.7f, 0.75f, 0.85f);
            }
            #endif
        }

        private IEnumerator AnimateIn()
        {
            float elapsed = 0f;
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            if (cardTransform != null) cardTransform.localScale = new Vector3(0.9f, 0.9f, 1f);

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                float ease = Mathf.SmoothStep(0f, 1f, t);

                if (canvasGroup != null) canvasGroup.alpha = ease;
                if (cardTransform != null) cardTransform.localScale = Vector3.Lerp(new Vector3(0.9f, 0.9f, 1f), Vector3.one, ease);
                yield return null;
            }

            if (canvasGroup != null) canvasGroup.alpha = 1f;
            if (cardTransform != null) cardTransform.localScale = Vector3.one;
        }

        private IEnumerator AnimateOut()
        {
            float elapsed = 0f;
            if (canvasGroup != null) canvasGroup.alpha = 1f;
            if (cardTransform != null) cardTransform.localScale = Vector3.one;

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                float ease = Mathf.SmoothStep(0f, 1f, t);

                if (canvasGroup != null) canvasGroup.alpha = 1f - ease;
                if (cardTransform != null) cardTransform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.95f, 0.95f, 1f), ease);
                yield return null;
            }

            if (canvasGroup != null) canvasGroup.alpha = 0f;
            if (cardTransform != null) cardTransform.localScale = new Vector3(0.95f, 0.95f, 1f);
        }

        private Color GetDicePopupBackground(Color playerColor)
        {
            return Color.Lerp(playerColor, Color.black, 0.85f); // Deep background tint
        }

        private Color GetDicePopupAccent(Color playerColor)
        {
            return Color.Lerp(playerColor, Color.white, 0.25f); // Clean border highlight tint
        }

        private Color GetReadableTextColor(Color backgroundColor)
        {
            float luminance = 0.299f * backgroundColor.r + 0.587f * backgroundColor.g + 0.114f * backgroundColor.b;
            return luminance > 0.6f ? Color.black : Color.white;
        }
    }
}
