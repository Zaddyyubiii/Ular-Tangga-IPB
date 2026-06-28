using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public static class SpriteCache
    {
        private static Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

        public static Sprite GetSprite(int playerId, int row, int col)
        {
            int pId = ((playerId - 1) % 4) + 1;
            string path = $"PlayerSprites/p{pId}_r{row}_c{col}";

            if (cache.TryGetValue(path, out Sprite s)) return s;

            // Load as Texture2D and create Sprite to guarantee it works (prevents Unity Sprite loading bugs)
            Texture2D tex = Resources.Load<Texture2D>(path);
            if (tex != null)
            {
                tex.filterMode = FilterMode.Point;
                Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                cache[path] = newSprite;
                return newSprite;
            }
            return null;
        }
    }

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
            
            // Fix prefab structure: swap tokenImage and borderImage if tokenImage is the root,
            // because Unity Canvas renders children on top. We want the sprite (tokenImage) on top!
            if (tokenImage != null && borderImage != null && tokenImage.gameObject == this.gameObject)
            {
                Image temp = tokenImage;
                tokenImage = borderImage;
                borderImage = temp;
            }

            if (tokenImage != null)
            {
                tokenImage.preserveAspect = true;
                // tokenImage is now the child, so it's safe to adjust its anchors
                RectTransform tokenRt = tokenImage.GetComponent<RectTransform>();
                if (tokenRt != null)
                {
                    // Make it fill the 65x65 root container perfectly
                    tokenRt.anchorMin = new Vector2(0f, 0f);
                    tokenRt.anchorMax = new Vector2(1f, 1f);
                    tokenRt.offsetMin = Vector2.zero;
                    tokenRt.offsetMax = Vector2.zero;
                }
            }

            if (borderImage != null)
            {
                // User requested NO BACKGROUND. Just disable it entirely.
                borderImage.enabled = false;
            }

            Debug.Log($"Token initialized for Player {playerData.id} with color: {playerData.playerColor}");
            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            if (data == null) return;

            // Apply player color to shadow/marker
            if (borderImage != null)
            {
                borderImage.color = new Color(data.playerColor.r, data.playerColor.g, data.playerColor.b, 0.6f);
            }

            // Keep sprite original colors, do NOT tint with playerColor
            if (tokenImage != null)
            {
                tokenImage.color = Color.white;
            }

            // Get Idle Sprite (Row 1 is idle, Col based on stage 1-4)
            int stageCol = Mathf.Clamp(data.currentEvolutionStage, 1, 4);
            Sprite idleSprite = SpriteCache.GetSprite(data.id, 1, stageCol);
            if (idleSprite != null && tokenImage != null)
            {
                tokenImage.sprite = idleSprite;
                tokenImage.color = Color.white; // Keep sprite original colors
            }
            else if (data.currentSprite != null && tokenImage != null)
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

            int stageCol = Mathf.Clamp(data.currentEvolutionStage, 1, 4);
            int[] walkFrames = { 3, 4, 2, 4 };

            for (int i = 0; i < pathPositions.Count; i++)
            {
                Vector2 startPos = rectTransform.anchoredPosition;
                Vector2 targetPos = pathPositions[i];
                float elapsed = 0f;

                // Cartoony animation metrics
                float jumpHeight = 35f; // Bouncy pixel lift
                float tiltDirection = targetPos.x > startPos.x ? -12f : 12f; // Tilt based on movement direction

                bool isJump = Vector2.Distance(startPos, targetPos) > 100f;
                bool isLadder = isJump && targetPos.y > startPos.y;
                bool isSnake = isJump && targetPos.y < startPos.y;

                if (isLadder) {
                    Sprite happy = SpriteCache.GetSprite(data.id, 5, stageCol);
                    if (happy != null) tokenImage.sprite = happy;
                } else if (isSnake) {
                    Sprite shocked = SpriteCache.GetSprite(data.id, 6, stageCol);
                    if (shocked != null) tokenImage.sprite = shocked;
                }

                float frameTimer = 0f;
                int frameIdx = 0;

                // Play soft tick SFX safely
                if (Audio.AudioManager.Instance != null)
                {
                    Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.tokenMoveClip);
                }

                while (elapsed < stepDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / stepDuration;

                    if (!isJump) {
                        frameTimer += Time.deltaTime;
                        if (frameTimer > 0.15f) {
                            frameTimer = 0f;
                            frameIdx = (frameIdx + 1) % 4;
                            Sprite walkSp = SpriteCache.GetSprite(data.id, walkFrames[frameIdx], stageCol);
                            if (walkSp != null) tokenImage.sprite = walkSp;
                        }
                    }

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

            // Restore idle sprite
            UpdateVisuals();
        }
    }
}
