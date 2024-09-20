using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    private List<CharacterData> characterDataList; // List of CharacterData ScriptableObjects

    [SerializeField]
    private List<CostumeData> costumeDataList; // List of CostumeData ScriptableObjects

    // Get CharacterData by character name
    public CharacterData GetCharacterData(string characterName)
    {
        foreach (var character in characterDataList)
        {
            if (character.Name == characterName)
            {
                return character;
            }
        }
        Debug.LogWarning("Character not found: " + characterName);
        return null;
    }

    // Get CostumeData by costume name
    public CostumeData GetCostumeData(string costumeName)
    {
        foreach (var costume in costumeDataList)
        {
            if (costume.CostumeName == costumeName)
            {
                return costume;
            }
        }
        Debug.LogWarning("Costume not found: " + costumeName);
        return null;
    }

    // Update character costumes (this could involve updating the UI or other elements)
    public void UpdateCharacterCostumes(CharacterData characterData)
    {
        // Implement your logic to update the character's appearance based on the new costumes
        Debug.Log("Updated character costumes for: " + characterData.Name);
    }
        public List<CharacterData> GetAllCharacters()
    {
        return characterDataList;
    }
}
