using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelected : MonoBehaviour
{
    public static CharacterSelected instance;
    public CharacterScriptableObject cst;
    private void Awake() {
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
    public void SelectCharacter(CharacterScriptableObject character){
        cst = character;
    }
    public void DestroyInstance(){
        instance = null;
        Destroy(gameObject);
    }
}
