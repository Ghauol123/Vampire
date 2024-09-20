using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreEntryPrefabCreator : MonoBehaviour
{
    public GameObject CreateScoreEntryPrefab()
    {
        // Create the root GameObject
        GameObject scoreEntry = new GameObject("ScoreEntry");

        // Add a Vertical Layout Group component for layout
        VerticalLayoutGroup layoutGroup = scoreEntry.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childControlHeight = true;
        layoutGroup.childControlWidth = true;
        layoutGroup.spacing = 5f;

        // Add a RectTransform to adjust the size and position
        RectTransform rectTransform = scoreEntry.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(300, 100); // Adjust as needed

        // Create and configure ScoreText
        GameObject scoreTextObject = CreateTextMeshProUGUI("ScoreText", "Score: ", scoreEntry);
        
        // Create and configure CharacterText
        GameObject characterTextObject = CreateTextMeshProUGUI("CharacterText", "Character: ", scoreEntry);
        
        // Create and configure PlayTimeText
        GameObject playTimeTextObject = CreateTextMeshProUGUI("PlayTimeText", "Play Time: ", scoreEntry);
        
        // Create and configure TimestampText
        GameObject timestampTextObject = CreateTextMeshProUGUI("TimestampText", "Date: ", scoreEntry);

        return scoreEntry;
    }

    private GameObject CreateTextMeshProUGUI(string name, string defaultText, GameObject parent)
    {
        // Create a new GameObject
        GameObject textObject = new GameObject(name);

        // Set it as a child of the parent
        textObject.transform.SetParent(parent.transform);

        // Add the TextMeshProUGUI component
        TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();

        // Set default text
        tmpText.text = defaultText;

        // Customize text settings (font size, color, etc.) as needed
        tmpText.fontSize = 24;
        tmpText.alignment = TextAlignmentOptions.Center;

        // Set RectTransform properties
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(300, 30); // Adjust size as needed

        return textObject;
    }
}
