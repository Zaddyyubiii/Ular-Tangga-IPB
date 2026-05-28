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
        
        [Header("Runtime Configurations")]
        public RuntimeBoardConfig runtimeBoardConfig;
        public int currentSeed;

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
            if (runtimeBoardConfig == null) runtimeBoardConfig = ConvertToRuntimeConfig(boardConfig);
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

        public void GenerateBoardWithSeed(int seed)
        {
            currentSeed = seed;
            Debug.Log("Generated Board Seed: " + seed);
            runtimeBoardConfig = BoardRandomizer.GenerateBoard(boardConfig, seed, Core.GameManager.Instance != null ? Core.GameManager.Instance.messageBank : null);
            GenerateBoard();
        }

        private RuntimeBoardConfig ConvertToRuntimeConfig(BoardConfig config)
        {
            RuntimeBoardConfig runtime = new RuntimeBoardConfig();
            if (config == null) return runtime;

            runtime.snakes = new List<TileDefinition>(config.snakes);
            runtime.ladders = new List<TileDefinition>(config.ladders);

            for (int i = 1; i <= 99; i++)
            {
                TileDefinition def = config.GetTileDefinition(i);
                if (def.type != TileType.Normal)
                {
                    runtime.tiles[i] = def;
                }
            }

            return runtime;
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

            // Outer frame (acts as subtle shadow/border)
            Image frame = tileGo.AddComponent<Image>();
            frame.color = new Color(0f, 0f, 0f, 0.22f);
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
            innerRect.anchorMin = Vector2.zero;
            innerRect.anchorMax = Vector2.one;
            innerRect.offsetMin = new Vector2(2f, 2f);
            innerRect.offsetMax = new Vector2(-2f, -2f);

            // White circular backdrop for number overlay (looks like a physical retro board game!)
            GameObject numBgGo = new GameObject("NumBg");
            numBgGo.transform.SetParent(tileGo.transform, false);
            Image numBg = numBgGo.AddComponent<Image>();
            numBg.color = new Color(1f, 1f, 1f, 0.55f); // Semi-transparent white
            
            RectTransform numBgRt = numBgGo.GetComponent<RectTransform>();
            numBgRt.anchorMin = new Vector2(0f, 1f);
            numBgRt.anchorMax = new Vector2(0f, 1f);
            numBgRt.pivot = new Vector2(0f, 1f);
            numBgRt.anchoredPosition = new Vector2(4f, -4f);
            numBgRt.sizeDelta = new Vector2(16f, 16f); // Clean circular-card shape

            // Number text
            GameObject numGo = new GameObject("Num");
            numGo.transform.SetParent(numBgGo.transform, false);
            TMPro.TextMeshProUGUI numText = numGo.AddComponent<TMPro.TextMeshProUGUI>();
            numText.alignment = TMPro.TextAlignmentOptions.Center;
            numText.fontSize = 9f;
            numText.fontStyle = TMPro.FontStyles.Bold;
            numText.raycastTarget = false;
            RectTransform numRect = numText.GetComponent<RectTransform>();
            numRect.anchorMin = Vector2.zero;
            numRect.anchorMax = Vector2.one;
            numRect.offsetMin = Vector2.zero;
            numRect.offsetMax = Vector2.zero;

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
            TileDefinition def = runtimeBoardConfig.GetTileDefinition(number);
            ApplyTileStyle(number, def, inner, numText, iconText);

            // Hide number circle backdrop on START and DUTA for aesthetic reasons
            if (number == 0 || number == 100)
            {
                numBgGo.SetActive(false);
            }

            TileView view = tileGo.GetComponent<TileView>();
            if (view == null) view = tileGo.AddComponent<TileView>();
            view.Initialize(number, def);
            view.background = inner;
            view.label = iconText;

            tileObjects[number] = tileGo;
        }

        private static readonly Color[] PastelRainbowColors = new Color[]
        {
            new Color(0.965f, 0.824f, 0.824f), // Soft Red/Pink
            new Color(0.980f, 0.886f, 0.749f), // Soft Peach/Orange
            new Color(0.976f, 0.965f, 0.765f), // Soft Yellow
            new Color(0.824f, 0.949f, 0.843f), // Soft Mint Green
            new Color(0.824f, 0.882f, 0.965f), // Soft Sky Blue
            new Color(0.890f, 0.824f, 0.965f)  // Soft Lavender
        };

        private void ApplyTileStyle(int number, TileDefinition def, Image bg, TMPro.TextMeshProUGUI numText, TMPro.TextMeshProUGUI iconText)
        {
            // Crisp dark-gray number text on light backdrop
            numText.color = new Color(0.12f, 0.12f, 0.12f, 0.85f);

            if (number == 0)
            {
                bg.color = new Color(0.1f, 0.52f, 0.28f); // Soft Vibrant Forest Green
                numText.text = "";
                iconText.text = "START";
                iconText.fontSize = 8.5f;
                iconText.color = Color.white;
                return;
            }
            if (number == 100)
            {
                bg.color = new Color(0.92f, 0.75f, 0.15f); // Luxurious Gold
                numText.text = "";
                iconText.text = "HOME";
                iconText.fontSize = 8.5f;
                iconText.color = Color.white;
                return;
            }

            numText.text = number.ToString();

            switch (def.type)
            {
                case TileType.Question:
                    bg.color = new Color(0.2f, 0.55f, 0.9f); // Radiant Soft Blue
                    iconText.text = "?";
                    iconText.fontSize = 22f;
                    iconText.color = new Color(1f, 0.95f, 0.4f); // Golden yellow "?"
                    break;

                case TileType.Skull:
                    bg.color = new Color(0.8f, 0.15f, 0.15f); // Soft Deep Crimson
                    iconText.text = "☠️"; // Proper high-quality warning icon
                    iconText.fontSize = 18f;
                    iconText.color = Color.white;
                    break;

                case TileType.Snake:
                    bg.color = new Color(0.85f, 0.32f, 0.25f); // Warning orange-red
                    iconText.text = "";
                    break;

                case TileType.Ladder:
                    bg.color = new Color(0.88f, 0.55f, 0.15f); // Soft gold
                    iconText.text = "";
                    break;

                default:
                    // Diagonal pastel rainbow wave - warm physical board feel
                    int z = number - 1;
                    int r = z / 10;
                    int c = z % 10;
                    int waveIndex = (r + c) % PastelRainbowColors.Length;
                    bg.color = PastelRainbowColors[waveIndex];
                    iconText.text = "";
                    break;
            }
        }

        private void DrawSnakesAndLadders()
        {
            if (boardPanel == null) return;

            foreach (var ladder in runtimeBoardConfig.ladders)
            {
                Vector2 s = GetTilePosition(ladder.tileIndex);
                Vector2 e = GetTilePosition(ladder.targetTileIndex);
                DrawLadder(s, e, ladder.severity);
            }

            foreach (var snake in runtimeBoardConfig.snakes)
            {
                Vector2 s = GetTilePosition(snake.tileIndex);
                Vector2 e = GetTilePosition(snake.targetTileIndex);
                DrawSnake(s, e, snake.severity);
            }
        }

        private void DrawLadder(Vector2 start, Vector2 end, int severity)
        {
            Vector2 dir = (end - start).normalized;
            Vector2 perp = new Vector2(-dir.y, dir.x);
            float dist = Vector2.Distance(start, end);
            float railOffset = 6f;

            Color railColor;
            Color rungColor;

            // Choose beautiful dynamic rail/rung colors based on severity
            if (severity == 0) // Light / Short
            {
                railColor = new Color(0.25f, 0.72f, 0.35f, 0.95f); // Soft Vibrant Green
                rungColor = new Color(0.15f, 0.58f, 0.22f, 0.95f);
            }
            else if (severity == 1) // Medium
            {
                railColor = new Color(0.2f, 0.5f, 0.9f, 0.95f); // Soft Vibrant Blue
                rungColor = new Color(0.12f, 0.35f, 0.75f, 0.95f);
            }
            else // Severe / Long
            {
                railColor = new Color(0.92f, 0.32f, 0.25f, 0.95f); // Soft Vibrant Coral Red
                rungColor = new Color(0.78f, 0.2f, 0.15f, 0.95f);
            }

            // Left rail
            CreateThickLine(start + perp * railOffset, end + perp * railOffset, railColor, 4.5f, "LadderRail");
            // Right rail
            CreateThickLine(start - perp * railOffset, end - perp * railOffset, railColor, 4.5f, "LadderRail");

            // Rungs
            int rungs = Mathf.Max(2, Mathf.RoundToInt(dist / 26f));
            for (int i = 1; i < rungs; i++)
            {
                float t = (float)i / rungs;
                Vector2 mid = Vector2.Lerp(start, end, t);
                CreateThickLine(mid + perp * railOffset, mid - perp * railOffset, rungColor, 3f, "LadderRung");
            }

            // Arrow indicator at top
            CreateArrow(end, dir, new Color(1f, 0.95f, 0.35f, 0.95f));
        }

        private void DrawSnake(Vector2 start, Vector2 end, int severity)
        {
            Vector2 dir = end - start;
            float dist = dir.magnitude;
            Vector2 perp = new Vector2(-dir.y, dir.x).normalized;

            // Winding snake using high-resolution segmented sine wave rendering
            int segments = Mathf.Max(18, Mathf.RoundToInt(dist / 7f));
            Vector2 prevPt = start;

            // Choose beautiful primary/secondary colors based on severity
            Color primaryColor = new Color(0.08f, 0.55f, 0.15f, 0.95f); // Vibrant Green
            Color secondaryColor = new Color(0.18f, 0.78f, 0.22f, 0.95f);

            if (severity == 1) // Medium
            {
                primaryColor = new Color(0.12f, 0.45f, 0.75f, 0.95f); // Vibrant Blue
                secondaryColor = new Color(0.25f, 0.7f, 0.95f, 0.95f);
            }
            else if (severity >= 2) // Long
            {
                primaryColor = new Color(0.8f, 0.12f, 0.12f, 0.95f); // Vibrant Crimson
                secondaryColor = new Color(0.95f, 0.65f, 0.15f, 0.95f);
            }

            for (int i = 1; i <= segments; i++)
            {
                float t = (float)i / segments;
                // Linear base position
                Vector2 basePt = Vector2.Lerp(start, end, t);
                // Winding sine-wave perpendicular offset (tapered towards the tail)
                float wave = Mathf.Sin(t * Mathf.PI * 3f) * 16f * (1f - t * 0.45f);
                Vector2 pt = basePt + perp * wave;

                // Tapered body thickness from head (11f) to tail (3.5f)
                float thickness = Mathf.Lerp(11f, 3.5f, t);
                Color segmentColor = (i % 2 == 0) ? primaryColor : secondaryColor;

                CreateThickLine(prevPt, pt, segmentColor, thickness, "SnakeSegment");
                prevPt = pt;
            }

            // Head circle at start tile
            CreateCircle(start, 9.5f, primaryColor, "SnakeHead");

            // Forked red tongue facing backwards (direction of body slope)
            Vector2 headForward = -dir.normalized;
            Vector2 tongueRight = new Vector2(-headForward.y, headForward.x).normalized;

            CreateThickLine(start, start + headForward * 9f + tongueRight * 3f, new Color(0.9f, 0.1f, 0.1f, 0.95f), 2.5f, "SnakeTongue");
            CreateThickLine(start, start + headForward * 9f - tongueRight * 3f, new Color(0.9f, 0.1f, 0.1f, 0.95f), 2.5f, "SnakeTongue");

            // Tail indicator at the end tile
            Vector2 tailDir = (end - prevPt).normalized;
            CreateArrow(end, -tailDir, secondaryColor);
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
