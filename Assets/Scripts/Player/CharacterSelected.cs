using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelected : MonoBehaviour
{
    //  public GameObject[] skins;
    // public int selectedCharacter;
    public static CharacterSelected instance;
    public CharacterData characterData;
    
    MainMenu mainMenu;
    PlayerStats playerStats;
    // public GameObject[] skins;

    private void Awake() {
        // selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Debug.LogWarning("Extra"+ this + "Delete");
            Destroy(gameObject);
        }
        mainMenu = FindObjectOfType<MainMenu>();
        
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
    public void SelectCharacter(CharacterData characters){ 
        characterData = characters;
    }

    public void DestroyInstance(){
        instance = null;
        Destroy(gameObject);
    }
}
