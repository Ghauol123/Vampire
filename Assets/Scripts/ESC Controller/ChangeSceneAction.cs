using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneAction : IAction
{
    private string previousScene;
    private string newScene;

    public ChangeSceneAction(string previousScene, string newScene)
    {
        this.previousScene = previousScene;
        this.newScene = newScene;
    }

    public void Execute()
    {
        // Tải cảnh mới (do SceneController.SceneChange xử lý)
        SceneManager.LoadScene(newScene);
    }

    public void Undo()
    {
        // Quay trở lại cảnh trước
        SceneManager.LoadScene(previousScene);
    }
}

