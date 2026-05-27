using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Board
{
    public class BoardManager : MonoBehaviour
    {
        public static BoardManager Instance;

        [Header("Board Configuration")]
        public BoardConfig boardConfig;

        [Header("Grid Layout")]
        public RectTransform boardPanel;
        public float tileWidth = 64f;
        public float tileHeight = 64f;
        public float startX = -288f;
        public float startY = -288f;

        [Header("Prefabs")]
        public GameObject tilePrefab;

        private Dictionary<int, Vector2> tilePositions = new Dictionary<int, Vector2>();
        private Dictionary<int, GameObject> tileObjects = new Dictionary<int, GameObject>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void GenerateBoard()
        {
            if (boardConfig == null) boardConfig = ScriptableObject.CreateInstance<BoardConfig>();
            if (boardPanel == null) return;

            for (int i = boardPanel.childCount - 1; i >= 0; i--)
                Destroy(boardPanel.GetChild(i).gameObject);

            boardConfig.ValidateConfig();
            tilePositions.Clear();
            tileObjects.Clear();

            // Draw background grid lines
            DrawGridLines();

            // Tile 0 (Start) left of tile 1
            Vector2 pos0 = new Vector2(startX - tileWidth - 6f, startY);
            tilePositions[0] = pos0;
            SpawnTileUI(0, pos0);

            // Tiles 1-100
            for (int n = 1; n <= 100; n++)
            {
                Vector2 pos = CalcPos(n);
                tilePositions[n] = pos;
                SpawnTileUI(n, pos);
            }

            // Draw snakes and ladders on top
            DrawSnakesAndLadders();
        }

        public Vector2 GetTilePosition(int n)
        {
            return tilePositions.TryGetValue(n, out Vector2 p) ? p : Vector2.zero;
        }

        private Vector2 CalcPos(int n)
        {
            int z = n - 1;
            int row = z / 10;
            int colInRow = z % 10;
            int col = (row % 2 == 0) ? colInRow : (9 - colInRow);
            return new Vector2(startX + col * tileWidth, startY + row * tileHeight);
        }

        private void DrawGridLines()
        {
            // Subtle background lines
            for (int r = 0; r <= 10; r++)
            {
                CreateLine(
                    new Vector2(startX - tileWidth * 0.5f, startY - tileHeight * 0.5f + r * tileHeight),
                    new Vector2(startX + 9.5f * tileWidth, startY - tileHeight * 0.5f + r * tileHeight),
                    new Color(1f, 1f, 1f, 0.07f), 1.5f);
            }
            for (int c = 0; c <= 10; c++)
            {
                CreateLine(
                    new Vector2(startX - tileWidth * 0.5f + c * tileWidth, startY - tileHeight * 0.5f),
                    new Vector2(startX - tileWidth * 0.5f + c * tileWidth, startY + 9.5f * tileHeight),
                    new Color(1f, 1f, 1f, 0.07f), 1.5f);
            }
        }

        private void SpawnTileUI(int number, Vector2 pos)
        {
            if (boardPanel == null) return;

            GameObject tileGo = new GameObject($"Tile_{number}");
            tileGo.transform.SetParent(boardPanel, false);

            // Outer frame (slightly larger, acts as border)
            Image frame = tileGo.AddComponent<Image>();
            frame.color = new Color(0f, 0f, 0f, 0.35f);
            frame.raycastTarget = false;

            RectTransform rTrans = tileGo.GetComponent<RectTransform>();
            rTrans.sizeDelta = new Vector2(tileWidth - 2f, tileHeight - 2f);
            rTrans.anchorMin = new Vector2(0.5f, 0.5f);
            rTrans.anchorMax = new Vector2(0.5f, 0.5f);
            rTrans.anchoredPosition = pos;

            // Inner colored background
            GameObject innerGo = new GameObject("Inner");
            innerGo.transform.SetParent(tileGo.transform, false);
            Image inner = innerGo.AddComponent<Image>();
            inner.raycastTarget = false;
            RectTransform innerRect = innerGo.GetComponent<RectTransform>();
            innerRect.anchorMin = new Vector2(0f, 0f);
            innerRect.anchorMax = new Vector2(1f, 1f);
            innerRect.offsetMin = new Vector2(2f, 2f);
            innerRect.offsetMax = new Vector2(-2f, -2f);

            // Number text
            GameObject numGo = new GameObject("Num");
            numGo.transform.SetParent(tileGo.transform, false);
            TMPro.TextMeshProUGUI numText = numGo.AddComponent<TMPro.TextMeshProUGUI>();
            numText.alignment = TMPro.TextAlignmentOptions.TopLeft;
            numText.fontSize = 8f;
            numText.fontStyle = TMPro.FontStyles.Bold;
            numText.raycastTarget = false;
            RectTransform numRect = numGo.GetComponent<RectTransform>();
            numRect.anchorMin = Vector2.zero;
            numRect.anchorMax = Vector2.one;
            numRect.offsetMin = new Vector2(3f, 0f);
            numRect.offsetMax = new Vector2(-1f, -2f);

            // Center icon text
            GameObject iconGo = new GameObject("Icon");
            iconGo.transform.SetParent(tileGo.transform, false);
            TMPro.TextMeshProUGUI iconText = iconGo.AddComponent<TMPro.TextMeshProUGUI>();
            iconText.alignment = TMPro.TextAlignmentOptions.Center;
            iconText.fontSize = 16f;
            iconText.fontStyle = TMPro.FontStyles.Bold;
            iconText.raycastTarget = false;
            RectTransform iconRect = iconGo.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            // Apply tile style
            TileDefinition def = boardConfig.GetTileDefinition(number);
            ApplyTileStyle(number, def, inner, numText, iconText);

            TileView view = tileGo.GetComponent<TileView>();
            if (view == null) view = tileGo.AddComponent<TileView>();
            view.Initialize(number, def);
            view.background = inner;
            view.label = iconText;

            tileObjects[number] = tileGo;
        }

        private void ApplyTileStyle(int number, TileDefinition def, Image bg, TMPro.TextMeshProUGUI numText, TMPro.TextMeshProUGUI iconText)
        {
            // Number text color
            numText.color = new Color(1f, 1f, 1f, 0.9f);

            if (number == 0)
            {
                bg.color = new Color(0.08f, 0.55f, 0.25f);
                numText.text = "";
                iconText.text = "START";
                iconText.fontSize = 9f;
                iconText.color = Color.white;
                return;
            }
            if (number == 100)
            {
                bg.color = new Color(0.85f, 0.65f, 0.05f);
                numText.text = "";
                iconText.text = "DUTA";
                iconText.fontSize = 9f;
                iconText.color = Color.white;
                return;
            }

            numText.text = number.ToString();

            switch (def.type)
            {
                case TileType.Question:
                    bg.color = new Color(0.08f, 0.45f, 0.85f);
                    iconText.text = "?";
                    iconText.fontSize = 22f;
                    iconText.color = new Color(1f, 1f, 0.5f);
                    numText.color = new Color(1f, 1f, 1f, 0.7f);
                    break;

                case TileType.Skull:
                    bg.color = new Color(0.55f, 0.05f, 0.05f);
                    iconText.text = "X";
                    iconText.fontSize = 20f;
                    iconText.color = new Color(1f, 0.3f, 0.3f);
                    numText.color = new Color(1f, 0.7f, 0.7f, 0.8f);
                    break;

                case TileType.Snake:
                    bg.color = new Color(0.75f, 0.15f, 0.05f);
                    iconText.text = "v";
                    iconText.fontSize = 18f;
                    iconText.color = new Color(1f, 0.85f, 0.2f);
                    numText.color = new Color(1f, 0.9f, 0.9f, 0.8f);
                    break;

                case TileType.Ladder:
                    bg.color = new Color(0.6f, 0.42f, 0.05f);
                    iconText.text = "^";
                    iconText.fontSize = 18f;
                    iconText.color = new Color(1f, 0.95f, 0.5f);
                    numText.color = new Color(1f, 0.97f, 0.8f, 0.8f);
                    break;

                default:
                    // Checkerboard alternating colors - warm board feel
                    int z = number - 1;
                    int r = z / 10;
                    int c = z % 10;
                    bool evenCell = (r + c) % 2 == 0;
                    bg.color = evenCell
                        ? new Color(0.88f, 0.72f, 0.38f)   // warm tan
                        : new Color(0.72f, 0.55f, 0.22f);  // darker tan
                    iconText.text = "";
                    numText.color = new Color(0.15f, 0.1f, 0.05f, 0.9f);
                    break;
            }
        }

        private void DrawSnakesAndLadders()
        {
            if (boardPanel == null) return;

            foreach (var ladder in boardConfig.ladders)
            {
                Vector2 s = GetTilePosition(ladder.tileIndex);
                Vector2 e = GetTilePosition(ladder.targetTileIndex);
                DrawLadder(s, e);
            }

            foreach (var snake in boardConfig.snakes)
            {
                Vector2 s = GetTilePosition(snake.tileIndex);
                Vector2 e = GetTilePosition(snake.targetTileIndex);
                DrawSnake(s, e);
            }
        }

        private void DrawLadder(Vector2 start, Vector2 end)
        {
            Vector2 dir = (end - start).normalized;
            Vector2 perp = new Vector2(-dir.y, dir.x);
            float dist = Vector2.Distance(start, end);
            float railOffset = 5f;
            Color railColor = new Color(0.75f, 0.55f, 0.1f, 0.92f);
            Color rungColor = new Color(0.6f, 0.42f, 0.05f, 0.92f);

            // Left rail
            CreateThickLine(start + perp * railOffset, end + perp * railOffset, railColor, 4f, "LadderRail");
            // Right rail
            CreateThickLine(start - perp * railOffset, end - perp * railOffset, railColor, 4f, "LadderRail");

            // Rungs
            int rungs = Mathf.Max(2, Mathf.RoundToInt(dist / 28f));
            for (int i = 1; i < rungs; i++)
            {
                float t = (float)i / rungs;
                Vector2 mid = Vector2.Lerp(start, end, t);
                CreateThickLine(mid + perp * railOffset, mid - perp * railOffset, rungColor, 3f, "LadderRung");
            }

            // Arrow indicator at top
            CreateArrow(end, dir, new Color(1f, 0.9f, 0.2f, 0.95f));
        }

        private void DrawSnake(Vector2 start, Vector2 end)
        {
            // Draw 3-segment bezier-like snake with multiple lines
            Color snakeColor = new Color(0.15f, 0.6f, 0.15f, 0.92f);
            Color snakeDark  = new Color(0.08f, 0.4f, 0.08f, 0.92f);

            Vector2 mid = (start + end) * 0.5f + new Vector2(tileWidth * 1.5f, 0f);

            // Draw body (3 segments) of the snake
            CreateThickLine(start, mid, snakeColor, 7f, "SnakeBody");
            CreateThickLine(mid, end, snakeDark, 7f, "SnakeBody");

            // Head circle at start
            CreateCircle(start, 9f, new Color(0.05f, 0.5f, 0.05f, 1f), "SnakeHead");

            // Tail arrow at end
            Vector2 tailDir = (end - start).normalized;
            CreateArrow(end, -tailDir, new Color(0.2f, 0.8f, 0.2f, 0.9f));
        }

        private void CreateThickLine(Vector2 start, Vector2 end, Color color, float thickness, string label, bool drawOnTop = true)
        {
            GameObject go = new GameObject(label);
            go.transform.SetParent(boardPanel, false);
            if (!drawOnTop)
            {
                go.transform.SetAsFirstSibling();
            }

            Image img = go.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = false;

            RectTransform rt = go.GetComponent<RectTransform>();
            Vector2 dir = end - start;
            float dist = dir.magnitude;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            rt.sizeDelta = new Vector2(dist, thickness);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = start + dir * 0.5f;
            rt.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void CreateLine(Vector2 start, Vector2 end, Color color, float thickness)
        {
            CreateThickLine(start, end, color, thickness, "GridLine", false);
        }

        private void CreateCircle(Vector2 center, float radius, Color color, string label, bool drawOnTop = true)
        {
            GameObject go = new GameObject(label);
            go.transform.SetParent(boardPanel, false);
            if (!drawOnTop)
            {
                go.transform.SetAsFirstSibling();
            }

            Image img = go.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = false;

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(radius * 2f, radius * 2f);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = center;
        }

        private void CreateArrow(Vector2 tip, Vector2 direction, Color color, bool drawOnTop = true)
        {
            GameObject go = new GameObject("Arrow");
            go.transform.SetParent(boardPanel, false);
            if (!drawOnTop)
            {
                go.transform.SetAsFirstSibling();
            }

            Image img = go.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = false;

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(10f, 10f);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = tip;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rt.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
