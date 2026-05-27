using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Board
{
    public class TileView : MonoBehaviour
    {
        public int tileNumber;
        public TileType tileType;
        public Image background;
        public TextMeshProUGUI label;

        public void Initialize(int number, TileDefinition definition)
        {
            tileNumber = number;
            tileType = definition != null ? definition.type : TileType.Normal;

            if (background == null)
            {
                background = GetComponent<Image>();
            }

            if (label == null)
            {
                label = GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }
}
