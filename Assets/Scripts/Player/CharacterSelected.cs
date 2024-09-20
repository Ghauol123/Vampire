using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelected : MonoBehaviour
{
    public static CharacterSelected instance;
    public CharacterData characterData;

    public GameObject[] skinObjects;
    public CostumeData costumeData;
    public GameObject skinChoose;
    public GameObject chooseCharacter;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (skinChoose != null)
        {
            skinChoose.SetActive(false);
        }
        foreach (var skinObject in skinObjects)
        {
            skinObject.SetActive(false);
        }

        if (costumeData == null && characterData != null && characterData.costumes.Count > 0)
        {
            costumeData = characterData.costumes[0];
        }
    }

    private void Update()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (instance == null && (currentSceneName == "Game" || currentSceneName == "CharSelect"))
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this || (currentSceneName != "Game" && currentSceneName != "CharSelect"))
        {
            Destroy(gameObject);
        }

        // Nhấn ESC để quay lại màn hình chọn nhân vật
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RevertToCharacterSelection();
        }
    }

    public static CostumeData GetSelectedCostume()
    {
        return instance?.costumeData ?? instance?.characterData.costumes[0];
    }

    public static CharacterData GetData()
    {
        if (instance && instance.characterData) 
            return instance.characterData;

        CharacterData[] characters = Resources.FindObjectsOfTypeAll<CharacterData>();
        if (characters.Length > 0)
        {
            return characters[Random.Range(0, characters.Length)];
        }
        return null;
    }

    public void SelectCharacter(CharacterData character)
    {
        if (character == null)
        {
            Debug.LogWarning("No character selected.");
            return;
        }

        characterData = character;

        if (chooseCharacter != null)
        {
            chooseCharacter.SetActive(false);
        }

        if (skinChoose != null)
        {
            skinChoose.SetActive(true);
        }

        PopulateSkinObjects();
    }

    public void DestroyInstance()
    {
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

    // Quay lại màn hình chọn nhân vật khi nhấn ESC
    public void RevertToCharacterSelection()
    {
        if (skinChoose != null)
        {
            skinChoose.SetActive(false); // Ẩn màn hình chọn skin
        }

        if (chooseCharacter != null)
        {
            chooseCharacter.SetActive(true); // Hiển thị lại màn hình chọn nhân vật
        }

        characterData = null; // Reset characterData
        costumeData = null; // Optional: reset luôn costumeData nếu cần
    }
}
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// public class CharacterSelected : MonoBehaviour
// {
//     public static CharacterSelected instance;
//     public CharacterData characterData;

//     public GameObject[] skinObjects;
//     public CostumeData costumeData;
//     public GameObject skinChoose;
//     public GameObject chooseCharacter;

//     private void Awake()
//     {
//         if (instance == null)
//         {
//             instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else if (instance != this)
//         {
//             Destroy(gameObject);
//         }
//     }

//     private void Start()
//     {
//         if (skinChoose != null)
//         {
//             skinChoose.SetActive(false);
//         }
//         foreach (var skinObject in skinObjects)
//         {
//             skinObject.SetActive(false);
//         }

//         if (costumeData == null && characterData != null && characterData.costumes.Count > 0)
//         {
//             costumeData = characterData.costumes[0];
//         }
//     }

//     private void Update()
//     {
//         string currentSceneName = SceneManager.GetActiveScene().name;

//         if (instance == null && (currentSceneName == "Game" || currentSceneName == "CharSelect"))
//         {
//             instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else if (instance != this || (currentSceneName != "Game" && currentSceneName != "CharSelect"))
//         {
//             Destroy(gameObject);
//         }

//         // Nhấn ESC để quay lại hành động trước đó
//         if (Input.GetKeyDown(KeyCode.Escape))
//         {
//             ActionManager.Instance.UndoLastAction();
//         }
//     }

//     public static CostumeData GetSelectedCostume()
//     {
//         return instance?.costumeData ?? instance?.characterData.costumes[0];
//     }

//     public static CharacterData GetData()
//     {
//         if (instance && instance.characterData) 
//             return instance.characterData;

//         CharacterData[] characters = Resources.FindObjectsOfTypeAll<CharacterData>();
//         if (characters.Length > 0)
//         {
//             return characters[Random.Range(0, characters.Length)];
//         }
//         return null;
//     }

//     private bool isSelectingCharacter = false;

// public void SelectCharacter(CharacterData character)
// {
//     if (isSelectingCharacter)
//         return;

//     if (character == null)
//     {
//         Debug.LogWarning("No character selected.");
//         return;
//     }

//     isSelectingCharacter = true;

//     SelectCharacterAction selectCharacterAction = new SelectCharacterAction(this.characterData, character);
//     ActionManager.Instance.PerformAction(selectCharacterAction);

//     characterData = character;

//     if (chooseCharacter != null)
//     {
//         chooseCharacter.SetActive(false);
//     }

//     if (skinChoose != null)
//     {
//         skinChoose.SetActive(true);
//     }

//     PopulateSkinObjects();

//     isSelectingCharacter = false;
// }
//     public void DestroyInstance()
//     {
//         instance = null;
//         Destroy(gameObject);
//     }

//     public void PopulateSkinObjects()
//     {
//         if (characterData == null) return;

//         int skinCount = characterData.costumes.Count;

//         for (int i = 0; i < skinObjects.Length; i++)
//         {
//             if (i < skinCount)
//             {
//                 skinObjects[i].SetActive(true);
//                 Image image = skinObjects[i].GetComponent<Image>();

//                 CostumeData costume = characterData.costumes[i];

//                 if (image != null)
//                 {
//                     image.sprite = costume.CostumeSprite;
//                 }

//                 UISpriteAnimation spriteAnimation = skinObjects[i].GetComponent<UISpriteAnimation>();

//                 if (spriteAnimation != null)
//                 {
//                     spriteAnimation.costumeData = costume;
//                 }
//             }
//             else
//             {
//                 skinObjects[i].SetActive(false);
//             }
//         }
//     }

// public void SelectSkin(CostumeData costume)
// {
//     if (characterData != null && costume != null && characterData.costumes.Contains(costume))
//     {
//         // Create the action and perform it
//         SelectSkinAction selectSkinAction = new SelectSkinAction(this.costumeData, costume);
//         ActionManager.Instance.PerformAction(selectSkinAction);

//         costumeData = costume;
//         Debug.Log($"Selected costume: {costumeData.name}");
//     }
//     else
//     {
//         Debug.LogWarning("Invalid costume or character not selected.");
//     }
// }


//     public void RevertToCharacterSelection()
//     {
//         if (skinChoose != null)
//         {
//             skinChoose.SetActive(false); // Ẩn màn hình chọn skin
//         }

//         if (chooseCharacter != null)
//         {
//             chooseCharacter.SetActive(true); // Hiển thị lại màn hình chọn nhân vật
//         }

//         characterData = null; // Reset characterData
//         costumeData = null; // Optional: reset luôn costumeData nếu cần
//     }
// }
