using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentAudioManager : MonoBehaviour
{
    private static PersistentAudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        // Check the current scene name using SceneManager
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log(currentScene);

        // Replace "Game" with the actual name of your game scene
        if (currentScene == "Game")
        {
            Destroy(gameObject);
        }
    }
}
