using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelected : MonoBehaviour
{
    //  public GameObject[] skins;
    // public int selectedCharacter;
    public static CharacterSelected instance;
    public CharacterScriptableObject cst;
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
    }
    public static CharacterScriptableObject GetData(){
        return instance.cst;
    }
    public void SelectCharacter(CharacterScriptableObject characters){ 
        cst = characters;
    }

    public void DestroyInstance(){
        instance = null;
        Destroy(gameObject);
    }
}
