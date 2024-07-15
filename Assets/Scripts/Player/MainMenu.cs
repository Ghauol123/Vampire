using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        // Start a new game
        SaveSystem.DeleteSave();
        SceneManager.LoadScene("Menu");
    }

    public void Continue()
    {
        // Continue from the last save
        if (SaveSystem.SaveExists())
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            Debug.LogWarning("No save file found!");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
