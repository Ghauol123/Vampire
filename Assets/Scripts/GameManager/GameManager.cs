using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState currentState;
    public GameState previousState;
    [Header("UI")]
    public GameObject pauseScreen;
    // Start is called before the first frame update
    void Start()
    {   
        pauseScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState){
            case GameState.GamePlay:
                TestChangeState();
                break;
            case GameState.Paused:
                TestChangeState();
                break;
        }
    }
    public void ChangeState(GameState newState){
        currentState = newState;
    }
    public void PauseGame(){
        if(currentState !=  GameState.Paused){
            previousState = currentState;
            ChangeState(GameState.Paused);
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("Game is Paused");
        }
    }
    public void ResumeGame(){
        if(currentState == GameState.Paused){
            ChangeState(previousState);
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
            Debug.Log("Game Resume");
        }
    }
    public void TestChangeState(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(currentState == GameState.Paused){
                ResumeGame();
            }
            else{
                PauseGame();
            }
        }
    }
}
