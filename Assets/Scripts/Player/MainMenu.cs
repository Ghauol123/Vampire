using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    public void OnNewGame()
    {
        Debug.Log("New Game");
        SceneManager.LoadScene("CharSelect");
        // DataPersistenceManager.instance.isNewgame = true;
        // DataPersistenceManager.instance.NewGameData();
    }
    public void ReturnMenu(){
        SceneManager.LoadScene("Main Screen");
        // DataPersistenceManager.instance.SaveGameData();
    }
}
