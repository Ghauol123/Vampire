using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
[System.Serializable]
public static class SaveSystem
{
    public static void SavePlayer(PlayerStats player){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.data";
        System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Create);
        PlayerStats data = new PlayerStats(player);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static PlayerStats LoadPlayer(){
        string path = Application.persistentDataPath + "/player.data";
        if(System.IO.File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Open);
            PlayerStats data = formatter.Deserialize(stream) as PlayerStats;
            stream.Close();
            return data;
        }
        else{
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
