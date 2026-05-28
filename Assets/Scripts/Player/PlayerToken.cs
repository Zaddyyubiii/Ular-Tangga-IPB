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

                // Cartoony animation metrics
                float jumpHeight = 35f; // Bouncy pixel lift
                float tiltDirection = targetPos.x > startPos.x ? -12f : 12f; // Tilt based on movement direction

                // Play soft tick SFX safely
                if (Audio.AudioManager.Instance != null)
                {
                    Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.tokenMoveClip);
                }

                while (elapsed < stepDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / stepDuration;

                    // 1. Horizontal/Vertical Interpolation with Ease-In-Out
                    float tEase = Mathf.SmoothStep(0f, 1f, t);
                    Vector2 currentPos = Vector2.Lerp(startPos, targetPos, tEase);

                    // 2. Parabolic vertical height simulation via sine wave
                    float heightOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;
                    currentPos.y += heightOffset;
                    rectTransform.anchoredPosition = currentPos;

                    // 3. Playful squash and stretch (stretch high in midair, squash flat on landing)
                    float scaleX = 1f;
                    float scaleY = 1f;
                    if (t < 0.5f)
                    {
                        // Stretch up while ascending
                        float ratio = t / 0.5f;
                        scaleX = Mathf.Lerp(1f, 0.82f, ratio);
                        scaleY = Mathf.Lerp(1f, 1.25f, ratio);
                    }
                    else
                    {
                        // Squash down on impact as we descend
                        float ratio = (t - 0.5f) / 0.5f;
                        scaleX = Mathf.Lerp(0.82f, 1.15f, ratio);
                        scaleY = Mathf.Lerp(1.25f, 0.85f, ratio);
                    }
                    rectTransform.localScale = new Vector3(scaleX, scaleY, 1f);

                    // 4. Dynamic rotational tilt to look alive
                    float tiltAngle = Mathf.Sin(t * Mathf.PI) * tiltDirection;
                    rectTransform.localRotation = Quaternion.Euler(0f, 0f, tiltAngle);

                    yield return null;
                }

                // Snap back to exact target with clean scale and rotation
                rectTransform.anchoredPosition = targetPos;
                rectTransform.localScale = Vector3.one;
                rectTransform.localRotation = Quaternion.identity;
            }
        }
    }
}
