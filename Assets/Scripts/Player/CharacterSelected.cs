using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelected : MonoBehaviour
{
    public static CharacterSelected instance;
    public CharacterData characterData;

    public GameObject[] skinObjects;
    public CostumeData costumeData;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        foreach (var skinObject in skinObjects)
        {
            skinObject.SetActive(false);
        }

        // Ensure a default costume is set
        if (costumeData == null && characterData != null && characterData.costumes.Count > 0)
        {
            costumeData = characterData.costumes[0];
        }
    }
        public static CostumeData GetSelectedCostume()
    {
        return instance.costumeData ?? instance.characterData.costumes[0];
    }
    public static CharacterData GetData(){
        if(instance && instance.characterData) return instance.characterData;
        else{
            CharacterData[] characters = Resources.FindObjectsOfTypeAll<CharacterData>();
            if(characters.Length > 0){
                return characters[Random.Range(0,characters.Length)];
            }
        }
        return null;
    }

    public void SelectCharacter(CharacterData character){ 
        characterData = character;
        PopulateSkinObjects();
    }
    
    public void DestroyInstance(){
        instance = null;
        Destroy(gameObject);
    }

    public void PopulateSkinObjects()
    {
        if (characterData == null) return;

        int skinCount = characterData.costumes.Count;

        for (int i = 0; i < skinObjects.Length; i++)
        {
            if (i < skinCount)
            {
                skinObjects[i].SetActive(true);
                Image image = skinObjects[i].GetComponent<Image>();

                CostumeData costume = characterData.costumes[i];

                if (image != null)
                {
                    image.sprite = costume.CostumeSprite;
                }

                UISpriteAnimation spriteAnimation = skinObjects[i].GetComponent<UISpriteAnimation>();
                
                if (spriteAnimation != null)
                {
                    spriteAnimation.costumeData = costume;
                }
            }
            else
            {
                skinObjects[i].SetActive(false);
            }
        }
    }

    // Method to select a skin and save its CostumeData
    public void SelectSkin(int skinIndex)
    {
        if (characterData != null && skinIndex >= 0 && skinIndex < characterData.costumes.Count)
        {
            costumeData = characterData.costumes[skinIndex];
            Debug.Log($"Selected costume: {costumeData.name}");
        }
        else
        {
            Debug.LogWarning("Invalid skin index or character not selected.");
        }
    }

}
