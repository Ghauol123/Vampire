using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    public void OnNewGame()
    {
        Debug.Log("New Game");
        SceneManager.LoadScene("Menu");
        DataPersistenceManager.instance.isNewgame = true;
        DataPersistenceManager.instance.NewGameData();
    }

    public void OnContinue()
    {
        Debug.Log("Continue");
        DataPersistenceManager.instance.LoadGameData();
        DataPersistenceManager.instance.isNewgame = false;
        SceneManager.LoadScene("Game");
    }
    public void ReturnMenu(){
        SceneManager.LoadScene("Main Screen");
        DataPersistenceManager.instance.SaveGameData();
    }
}
