using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void LoadGameData(GameData gameData);
    void SaveGameData(ref GameData gameData); // ref is used to pass the reference of the object
}
