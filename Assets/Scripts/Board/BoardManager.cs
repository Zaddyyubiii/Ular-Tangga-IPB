using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Board
{
    public class BoardManager : MonoBehaviour
    {
        public static BoardManager Instance;

        [Header("Board Configuration Reference")]
        public BoardConfig boardConfig;

        [Header("Grid Layout Parameters")]
        public RectTransform boardPanel;
        public float tileWidth = 60f;
        public float tileHeight = 60f;
        public float startX = -270f;
        public float startY = -270f;

        [Header("Prefabs (Optional, will auto-generate if null)")]
        public GameObject tilePrefab;

        // Caches for generated tile positions
        private Dictionary<int, Vector2> tilePositions = new Dictionary<int, Vector2>();
        private Dictionary<int, GameObject> tileObjects = new Dictionary<int, GameObject>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void GenerateBoard()
        {
            if (boardConfig == null)
            {
                boardConfig = ScriptableObject.CreateInstance<BoardConfig>();
            }

            if (boardPanel != null)
            {
                for (int i = boardPanel.childCount - 1; i >= 0; i--)
                {
                    Destroy(boardPanel.GetChild(i).gameObject);
                }
            }

            boardConfig.ValidateConfig();
            tilePositions.Clear();
            tileObjects.Clear();

            // 1. Calculate positions and spawn tiles
            // Tile 0 (Start) is just to the left of Tile 1
            Vector2 pos0 = new Vector2(startX - tileWidth - 10f, startY);
            tilePositions[0] = pos0;
            SpawnTileUI(0, pos0);

            // Tiles 1 - 100
            for (int tileNum = 1; tileNum <= 100; tileNum++)
            {
                Vector2 pos = CalculateSerpentinePosition(tileNum);
                tilePositions[tileNum] = pos;
                SpawnTileUI(tileNum, pos);
            }

            // 2. Draw Snakes and Ladders visually using UI RectTransform lines
            DrawSnakesAndLadders();
        }

        public Vector2 GetTilePosition(int tileNumber)
        {
            if (tilePositions.TryGetValue(tileNumber, out Vector2 pos))
            {
                return pos;
            }
            return Vector2.zero;
        }

        private Vector2 CalculateSerpentinePosition(int tileNumber)
        {
            int zeroBased = tileNumber - 1;
            int row = zeroBased / 10;
            int colInRow = zeroBased % 10;

            int col;
            if (row % 2 == 0) // Even row (0, 2, 4, 6, 8) -> Left to Right
            {
                col = colInRow;
            }
            else // Odd row (1, 3, 5, 7, 9) -> Right to Left
            {
                col = 9 - colInRow;
            }

            float x = startX + (col * tileWidth);
            float y = startY + (row * tileHeight);

            return new Vector2(x, y);
        }

        private void SpawnTileUI(int number, Vector2 pos)
        {
            if (boardPanel == null) return;

            GameObject tileGo;
            if (tilePrefab != null)
            {
                tileGo = Instantiate(tilePrefab, boardPanel);
            }
            else
            {
                // Generate procedural tile UI gameobject
                tileGo = new GameObject($"Tile_{number}");
                tileGo.transform.SetParent(boardPanel, false);
                
                Image img = tileGo.AddComponent<Image>();
                img.raycastTarget = false;

                // Spawning Text inside
                GameObject txtGo = new GameObject("Text");
                txtGo.transform.SetParent(tileGo.transform, false);
                TMPro.TextMeshProUGUI text = txtGo.AddComponent<TMPro.TextMeshProUGUI>();
                text.alignment = TMPro.TextAlignmentOptions.Center;
                text.fontSize = 14f;
                text.fontStyle = TMPro.FontStyles.Bold;
                text.color = Color.white;

                RectTransform txtRect = txtGo.GetComponent<RectTransform>();
                txtRect.anchorMin = Vector2.zero;
                txtRect.anchorMax = Vector2.one;
                txtRect.offsetMin = Vector2.zero;
                txtRect.offsetMax = Vector2.zero;
            }

            RectTransform rTrans = tileGo.GetComponent<RectTransform>();
            rTrans.sizeDelta = new Vector2(tileWidth - 4f, tileHeight - 4f); // slight margin
            rTrans.anchoredPosition = pos;

            // Stylize tile based on type
            TileDefinition def = boardConfig.GetTileDefinition(number);
            TileView view = tileGo.GetComponent<TileView>();
            if (view == null) view = tileGo.AddComponent<TileView>();
            view.Initialize(number, def);

            Image tileImage = tileGo.GetComponent<Image>();
            TMPro.TextMeshProUGUI tileText = tileGo.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            if (tileText != null)
            {
                if (number == 0) tileText.text = "START";
                else if (number == 100) tileText.text = "DUTA";
                else tileText.text = number.ToString();
            }

            if (tileImage != null)
            {
                if (number == 0)
                {
                    tileImage.color = new Color(0.12f, 0.73f, 0.35f); // Green for start
                }
                else if (number == 100)
                {
                    tileImage.color = new Color(0.85f, 0.65f, 0.12f); // Golden for finish
                }
                else
                {
                    switch (def.type)
                    {
                        case TileType.Question:
                            tileImage.color = new Color(0f, 0.63f, 0.95f); // Beautiful sky blue
                            if (tileText != null) tileText.text = $"{number}\n?";
                            break;
                        case TileType.Skull:
                            tileImage.color = new Color(0.15f, 0.15f, 0.15f); // Deep charcoal black
                            if (tileText != null) tileText.text = $"{number}\nX";
                            break;
                        case TileType.Snake:
                            tileImage.color = new Color(0.9f, 0.35f, 0.15f); // Reddish-orange
                            break;
                        case TileType.Ladder:
                            tileImage.color = new Color(0.95f, 0.8f, 0.1f); // Warm yellow-gold
                            break;
                        default:
                            // Alternating standard checker board colors
                            int zeroBased = number - 1;
                            int r = zeroBased / 10;
                            int c = zeroBased % 10;
                            if ((r + c) % 2 == 0)
                                tileImage.color = new Color(0.95f, 0.75f, 0.3f); // Warm Orange-yellow
                            else
                                tileImage.color = new Color(0.98f, 0.88f, 0.5f); // Soft Light Yellow
                            break;
                    }
                }
            }

            tileObjects[number] = tileGo;
        }

        private void DrawSnakesAndLadders()
        {
            if (boardPanel == null) return;

            // Draw Ladders
            foreach (var ladder in boardConfig.ladders)
            {
                Vector2 start = GetTilePosition(ladder.tileIndex);
                Vector2 end = GetTilePosition(ladder.targetTileIndex);
                CreateUILine(start, end, new Color(0.58f, 0.34f, 0.13f, 0.7f), 10f, "LADDER");
            }

            // Draw Snakes
            foreach (var snake in boardConfig.snakes)
            {
                Vector2 start = GetTilePosition(snake.tileIndex);
                Vector2 end = GetTilePosition(snake.targetTileIndex);
                CreateUILine(start, end, new Color(0.18f, 0.67f, 0.32f, 0.7f), 8f, "SNAKE");
            }
        }

        private void CreateUILine(Vector2 start, Vector2 end, Color color, float thickness, string label)
        {
            GameObject lineGo = new GameObject($"{label}_{start}_to_{end}");
            lineGo.transform.SetParent(boardPanel, false);
            lineGo.transform.SetAsFirstSibling(); // Draw behind players

            Image img = lineGo.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = false;

            RectTransform rTrans = lineGo.GetComponent<RectTransform>();
            Vector2 dir = end - start;
            float distance = dir.magnitude;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            rTrans.sizeDelta = new Vector2(distance, thickness);
            rTrans.anchorMin = new Vector2(0.5f, 0.5f);
            rTrans.anchorMax = new Vector2(0.5f, 0.5f);
            rTrans.anchoredPosition = start + dir * 0.5f;
            rTrans.localRotation = Quaternion.Euler(0, 0, angle);

            // Draw rungs for ladders visually
            if (label == "LADDER")
            {
                int rungs = Mathf.Max(2, Mathf.RoundToInt(distance / 25f));
                for (int i = 1; i < rungs; i++)
                {
                    GameObject rungGo = new GameObject("Rung");
                    rungGo.transform.SetParent(lineGo.transform, false);
                    Image rungImg = rungGo.AddComponent<Image>();
                    rungImg.color = color;
                    
                    RectTransform rungRect = rungGo.GetComponent<RectTransform>();
                    rungRect.sizeDelta = new Vector2(4f, thickness * 2.5f);
                    float offset = -distance * 0.5f + (distance / rungs) * i;
                    rungRect.anchoredPosition = new Vector2(offset, 0f);
                }
            }
        }
    }
}
