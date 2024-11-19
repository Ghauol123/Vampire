using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    private List<CharacterData> characterDataList; // List of CharacterData ScriptableObjects

    [SerializeField]
    private List<CostumeData> costumeDataList; // List of CostumeData ScriptableObjects

    private void Start() {
        // // Load all CharacterData ScriptableObjects
        // characterDataList = new List<CharacterData>(Resources.LoadAll<CharacterData>("CharacterData"));

        // // Load all CostumeData ScriptableObjects
        // costumeDataList = new List<CostumeData>(Resources.LoadAll<CostumeData>("CostumeData"));
        characterDataList = FirebaseController.instance.characterDataList;
    }

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

        public List<CharacterData> GetAllCharacters()
    {
        return characterDataList;
    }
}
