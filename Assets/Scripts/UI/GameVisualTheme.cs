using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [CreateAssetMenu(fileName = "GameVisualTheme", menuName = "UlarTangga/GameVisualTheme")]
    public class GameVisualTheme : ScriptableObject
    {
        [Header("Nature Colors")]
        public Color grassGreen = new Color(0.36f, 0.73f, 0.39f);    // #5DBB63
        public Color deepGrass = new Color(0.18f, 0.49f, 0.31f);     // #2E7D4F
        public Color softGrass = new Color(0.56f, 0.86f, 0.45f);     // #8EDC74
        public Color dirtPath = new Color(0.84f, 0.61f, 0.32f);      // #D79B52
        public Color lightDirt = new Color(0.94f, 0.75f, 0.44f);     // #F0C070

        [Header("Wood & Parchment Colors")]
        public Color woodBrown = new Color(0.60f, 0.35f, 0.18f);     // #9A5A2E
        public Color darkWood = new Color(0.36f, 0.20f, 0.12f);      // #5B321F
        public Color parchment = new Color(0.96f, 0.79f, 0.51f);     // #F5C982
        public Color parchmentLight = new Color(1.00f, 0.85f, 0.57f); // #FFD991
        public Color parchmentDark = new Color(0.72f, 0.42f, 0.21f);  // #B86B36

        [Header("Sky & Text Colors")]
        public Color skyBlue = new Color(0.37f, 0.78f, 0.95f);       // #5EC7F2
        public Color deepBlue = new Color(0.11f, 0.31f, 0.54f);      // #1B4E89
        public Color creamText = new Color(1.00f, 0.95f, 0.76f);     // #FFF1C1
        public Color darkText = new Color(0.29f, 0.16f, 0.10f);      // #4A2A1A

        [Header("Feedback Colors")]
        public Color successGreen = new Color(0.12f, 0.73f, 0.35f);
        public Color warningOrange = new Color(0.95f, 0.60f, 0.10f);
        public Color dangerRed = new Color(0.80f, 0.20f, 0.15f);

        [Header("Player Colors (Normal & Active)")]
        public Color player1Normal = new Color(0.62f, 0.18f, 0.18f); // #9E2F2F
        public Color player1Active = new Color(0.91f, 0.30f, 0.30f); // #E84C4C

        public Color player2Normal = new Color(0.18f, 0.36f, 0.66f); // #2F5DA8
        public Color player2Active = new Color(0.31f, 0.55f, 1.00f); // #4F8CFF

        public Color player3Normal = new Color(0.18f, 0.55f, 0.34f); // #2F8B57
        public Color player3Active = new Color(0.22f, 0.82f, 0.48f); // #38D27A

        public Color player4Normal = new Color(0.72f, 0.47f, 0.13f); // #B87822
        public Color player4Active = new Color(1.00f, 0.76f, 0.28f); // #FFC247

        public Color GetPlayerColor(int id, bool isActive)
        {
            switch (id)
            {
                case 1: return isActive ? player1Active : player1Normal;
                case 2: return isActive ? player2Active : player2Normal;
                case 3: return isActive ? player3Active : player3Normal;
                case 4: return isActive ? player4Active : player4Normal;
                default: return Color.gray;
            }
        }

        public void StylePanelAsParchment(GameObject panel)
        {
            var img = panel.GetComponent<Image>();
            if (img != null)
            {
                img.color = parchment;
            }
            var outline = panel.GetComponent<Outline>();
            if (outline == null) outline = panel.AddComponent<Outline>();
            outline.effectColor = darkWood;
            outline.effectDistance = new Vector2(3f, 3f);
        }

        public void StylePanelAsWood(GameObject panel)
        {
            var img = panel.GetComponent<Image>();
            if (img != null)
            {
                img.color = woodBrown;
            }
            var outline = panel.GetComponent<Outline>();
            if (outline == null) outline = panel.AddComponent<Outline>();
            outline.effectColor = darkWood;
            outline.effectDistance = new Vector2(4f, 4f);
        }

        public void StyleButtonAsWood(Button button, TMPro.TextMeshProUGUI label = null)
        {
            var img = button.GetComponent<Image>();
            if (img != null)
            {
                img.color = woodBrown;
                
                ColorBlock cb = button.colors;
                cb.normalColor = woodBrown;
                cb.highlightedColor = lightDirt; // warm hover color
                cb.pressedColor = darkWood;
                cb.selectedColor = woodBrown;
                cb.disabledColor = new Color(woodBrown.r * 0.5f, woodBrown.g * 0.5f, woodBrown.b * 0.5f, 0.5f);
                button.colors = cb;
            }
            
            var outline = button.GetComponent<Outline>();
            if (outline == null) outline = button.gameObject.AddComponent<Outline>();
            outline.effectColor = darkWood;
            outline.effectDistance = new Vector2(2f, 2f);

            if (label != null)
            {
                label.color = creamText;
            }
        }

        public void StyleButtonAsParchment(Button button, TMPro.TextMeshProUGUI label = null)
        {
            var img = button.GetComponent<Image>();
            if (img != null)
            {
                img.color = parchmentLight;
                
                ColorBlock cb = button.colors;
                cb.normalColor = parchmentLight;
                cb.highlightedColor = parchment;
                cb.pressedColor = parchmentDark;
                cb.selectedColor = parchmentLight;
                cb.disabledColor = new Color(parchmentLight.r * 0.5f, parchmentLight.g * 0.5f, parchmentLight.b * 0.5f, 0.5f);
                button.colors = cb;
            }
            
            var outline = button.GetComponent<Outline>();
            if (outline == null) outline = button.gameObject.AddComponent<Outline>();
            outline.effectColor = darkText;
            outline.effectDistance = new Vector2(2f, 2f);

            if (label != null)
            {
                label.color = darkText;
            }
        }

        public void StyleInputFieldAsParchment(TMPro.TMP_InputField inputField)
        {
            var img = inputField.GetComponent<Image>();
            if (img != null)
            {
                img.color = parchmentLight;
            }
            
            var outline = inputField.GetComponent<Outline>();
            if (outline == null) outline = inputField.gameObject.AddComponent<Outline>();
            outline.effectColor = darkWood;
            outline.effectDistance = new Vector2(2f, 2f);

            if (inputField.textComponent != null)
            {
                inputField.textComponent.color = darkText;
            }
            if (inputField.placeholder != null)
            {
                var placeholderText = inputField.placeholder.GetComponent<TMPro.TextMeshProUGUI>();
                if (placeholderText != null)
                {
                    placeholderText.color = new Color(darkText.r, darkText.g, darkText.b, 0.5f);
                }
            }
        }
    }
}
