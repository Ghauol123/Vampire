using UnityEngine;

public class QuitGame : MonoBehaviour
{
    [SerializeField] private CharacterData[] characterDataList;

    private void Start()
    {
        characterDataList = FindObjectsOfType<CharacterData>();
    }

    public void Quit()
    {
        // Reset all character data to base stats
        foreach (var characterData in characterDataList)
        {
            characterData.ResetToBaseStats();
        }

        // Quit the game
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
