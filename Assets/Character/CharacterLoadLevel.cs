using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoadLevel : MonoBehaviour
{
    public List<CharacterData> characters; // Assign these in the Unity Editor
    private FirebaseController firebaseController;
    // Start is called before the first frame update
    void Start()
    {
        // Ensure firebaseController is assigned
        if (firebaseController == null)
        {
            firebaseController = FindObjectOfType<FirebaseController>();
            if (firebaseController == null)
            {
                Debug.LogError("FirebaseController not found.");
                return;
            }
        }

        characters = firebaseController.characterDataList;
        if (characters == null || characters.Count == 0)
        {
            Debug.LogError("Character data list is null or empty.");
            return;
        }

        for (int i = 0; i < characters.Count; i++)
        {
            LoadCharacterLevel(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        private void LoadCharacterLevel(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= characters.Count)
        {
            Debug.LogError("Invalid character index.");
            return;
        }

        CharacterData character = characters[characterIndex];
        firebaseController.LoadCharacterLevel(character.Name, level =>
        {
            character.SetLevel(level);
            character.AdjustStatsBasedOnLevel(); // Adjust stats based on the loaded level
            Debug.Log($"Character {character.Name} level set to {character.Level}");
        });
    }
}
