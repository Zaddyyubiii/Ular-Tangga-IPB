using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerToken : MonoBehaviour
    {
        public PlayerData data;
        
        [Header("UI Visual Components")]
        public Image tokenImage;
        public Image borderImage;
        
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (tokenImage == null) tokenImage = GetComponent<Image>();
        }

        public void Initialize(PlayerData playerData)
        {
            this.data = playerData;
            name = "Token_" + data.playerName;
            Debug.Log($"Token initialized for Player {playerData.id} with color: {playerData.playerColor}");
            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            if (data == null) return;

            // Apply player color to border/marker first
            if (borderImage != null)
            {
                borderImage.color = data.playerColor;
            }

            // Apply player color to token background
            if (tokenImage != null)
            {
                tokenImage.color = data.playerColor;
            }

            // Apply evolution sprite if available, keeping sprite original colors
            if (data.currentSprite != null && tokenImage != null)
            {
                tokenImage.sprite = data.currentSprite;
                tokenImage.color = Color.white; // Keep sprite original colors
            }

            // Dim or transparency if dropped out
            if (data.isDroppedOut)
            {
                if (tokenImage != null) tokenImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
                if (borderImage != null) borderImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            }
        }

        public IEnumerator MoveTileByTile(List<Vector2> pathPositions, float stepDuration)
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();

            for (int i = 0; i < pathPositions.Count; i++)
            {
                Vector2 startPos = rectTransform.anchoredPosition;
                Vector2 targetPos = pathPositions[i];
                float elapsed = 0f;

                // Play soft tick SFX (AudioManager call will be safe)
                if (Audio.AudioManager.Instance != null)
                {
                    Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.tokenMoveClip);
                }

                while (elapsed < stepDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / stepDuration;
                    rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                    yield return null;
                }
                rectTransform.anchoredPosition = targetPos;
            }
        }
    }
}
