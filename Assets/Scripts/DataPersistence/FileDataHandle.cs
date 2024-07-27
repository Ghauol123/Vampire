using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandle 
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandle(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load(){
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadGame = null;
        if(File.Exists(fullPath)){
            try
            {
                string dataStore = "";
                using (FileStream streamReader = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(streamReader))
                    {
                        dataStore = reader.ReadToEnd();
                    }
                }
                loadGame = JsonUtility.FromJson<GameData>(dataStore);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error loading file: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("File does not exist: " + fullPath);
        }
        return loadGame;
    }

    public void Save(GameData gameData){
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            // Create the directory if it does not exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            // Write the data to the file
            string dataStore = JsonUtility.ToJson(gameData);
            using (FileStream streamWriter = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(streamWriter))
                {
                    writer.Write(dataStore);
                }
            }
            Debug.Log("File saved successfully: " + fullPath);
        }
        catch (System.UnauthorizedAccessException e)
        {
            Debug.LogWarning("Unauthorized Access: " + e.Message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error saving file: " + e.Message);
        }
    }
}
