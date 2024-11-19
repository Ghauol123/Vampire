using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelected : MonoBehaviour
{
    public static CharacterSelected instance;
    public CharacterData characterData;
    public string nameMap;

    public GameObject[] skinObjects;
    public CostumeData costumeData;
    public GameObject skinChoose;
    public GameObject chooseCharacter;
    public GameObject mapChoose;
    public GameObject gameModeChoose;

    public GameObject skinButtonPrefab; // Prefab for the skin button
    public Transform skinButtonContainer; // Container to hold the instantiated buttons
    public GameObject playbutton;
    public GameMode gamemode;

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
        if(chooseCharacter !=null){
            chooseCharacter.SetActive(true);
        }
        playbutton.SetActive(false);
        if (skinChoose != null)
        {
            skinChoose.SetActive(false);
        }
        foreach (var skinObject in skinObjects)
        {
            skinObject.SetActive(false);
        }
        if(mapChoose !=null){
            mapChoose.SetActive(false);
        }
        if(gameModeChoose != null){
            gameModeChoose.SetActive(false);
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

        // Clear existing skin buttons
        foreach (Transform child in skinButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Create new skin buttons
        for (int i = 0; i < characterData.costumes.Count; i++)
        {
            GameObject buttonObj = Instantiate(skinButtonPrefab, skinButtonContainer);
            CostumeButton costumeButton = buttonObj.GetComponent<CostumeButton>();
            if (costumeButton != null)
            {
                costumeButton.SetupButton(characterData.costumes[i], i, this);
            }
        }

        // Set the first costume as default if not already set
        if (costumeData == null && characterData.costumes.Count > 0)
        {
            costumeData = characterData.costumes[0];
        }
    }

    public void DestroyInstance()
    {
        instance = null;
        Destroy(gameObject);
    }

    public void SelectSkin(int skinIndex)
    {
        if (characterData != null && skinIndex >= 0 && skinIndex < characterData.costumes.Count)
        {
            costumeData = characterData.costumes[skinIndex];
            if (skinChoose != null)
            {
                skinChoose.SetActive(false);
            }
            if(mapChoose != null){
                gameModeChoose.SetActive(true);
            }
            Debug.Log($"Selected costume: {costumeData.name}");
        }
        else
        {
            Debug.LogWarning("Invalid skin index or character not selected.");
        }
    }
    public void selectMap(string map){
        this.nameMap = map;
        
        // if(map == "Easy"){
        //     gamemode = GameMode.SinglePlayer;
        // }
        // else{
        //     gamemode = GameMode.BotMode;
        // }
        playbutton.SetActive(true);
    }
        public void SelectMode(GameMode mode)
    {
        this.gamemode = mode;
        Debug.Log($"Selected game mode: {mode}");
    }
    public void SelectSinglePlayerMode()
    {
        SelectMode(GameMode.SinglePlayer);
        mapChoose.SetActive(true);
        gameModeChoose.SetActive(false);
    }

    public void SelectBotMode()
    {
        SelectMode(GameMode.BotMode);
        mapChoose.SetActive(true);
        gameModeChoose.SetActive(false);
    }
    // Quay lại màn hình chọn nhân vật khi nhấn ESC
    public void RevertToCharacterSelection()
    {
        if (skinChoose != null)
        {
            skinChoose.SetActive(false); // Ẩn màn hình chọn skin
        }
        if(mapChoose != null){
            mapChoose.SetActive(false);
        }
        if (chooseCharacter != null)
        {
            chooseCharacter.SetActive(true); // Hiển thị lại màn hình chọn nhân vật
        }

        characterData = null; // Reset characterData
        costumeData = null; // Optional: reset luôn costumeData nếu cần
        playbutton.SetActive(false); // Ẩn nút Play khi quay lại màn hình chọn nhân vật
    }
}