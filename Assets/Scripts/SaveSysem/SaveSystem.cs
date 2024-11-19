// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Runtime.Serialization.Formatters.Binary;
// using System.IO;

// public static class SaveSystem
// {
//     private static string path = Application.persistentDataPath + "/savefile.dat";

//     public static void SaveGame(CharacterData characterData, PlayerStats playerStats)
//     {
//         BinaryFormatter formatter = new BinaryFormatter();
//         FileStream stream = null;

//         try
//         {
//             stream = new FileStream(path, FileMode.Create);

//             PlayerData data = new PlayerData(characterData, playerStats);

//             formatter.Serialize(stream, data);
//             Debug.Log("Game saved successfully.");
//             Debug.Log(path);
//             Debug.Log(data.spriteRendererSpriteName);
//             Debug.Log(data.animatorControllerName);
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError("Error saving game: " + e.Message);
//         }
//         finally
//         {
//             if (stream != null)
//             {
//                 stream.Close();
//             }
//         }
//     }

//     public static PlayerData LoadGame()
//     {
//         if (File.Exists(path))
//         {
//             BinaryFormatter formatter = new BinaryFormatter();
//             FileStream stream = null;

//             try
//             {
//                 stream = new FileStream(path, FileMode.Open);

//                 if (stream.Length > 0)
//                 {
//                     PlayerData data = formatter.Deserialize(stream) as PlayerData;
//             Debug.Log(data.spriteRendererSpriteName);
//             Debug.Log(data.animatorControllerName);

//                     Debug.Log("Game loaded successfully.");
//                     return data;
//                 }
//                 else
//                 {
//                     Debug.LogWarning("Save file is empty.");
//                 }
//             }
//             catch (System.Exception e)
//             {
//                 Debug.LogError("Error loading game: " + e.Message);
//             }
//             finally
//             {
//                 if (stream != null)
//                 {
//                     stream.Close();
//                 }
//             }
//         }
//         else
//         {
//             Debug.LogError("Save file not found in " + path);
//         }

//         return null;
//     }

//     public static bool SaveExists()
//     {
//         return File.Exists(path);
//     }

//     public static void DeleteSave()
//     {
//         if (File.Exists(path))
//         {
//             File.Delete(path);
//             Debug.Log("Save file deleted.");
//         }
//         else
//         {
//             Debug.LogWarning("No save file to delete.");
//         }
//     }
// }
