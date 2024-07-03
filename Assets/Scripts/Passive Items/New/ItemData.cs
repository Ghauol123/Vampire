using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemData", menuName = "ItemData", order = 0)]
public abstract class ItemData : ScriptableObject {
    public Sprite icon;
    public int maxLevel;
    public abstract Item.LevelData GetLevelData(int level);
}


