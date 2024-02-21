using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemsScriptableObject", menuName = "ScriptableObject/PassiveItems", order = 0)]
public class PassiveItemsScriptableObject : ScriptableObject
{
    [SerializeField]
    float multiple;
    public float Multiple
    {
        get { return multiple; }
        set { multiple = value; }
    }
    [SerializeField]
    int level;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }
    [SerializeField]
    GameObject nextLevelPrefabs;
    public GameObject NextLevelPrefabs
    {
        get { return nextLevelPrefabs; }
        set { nextLevelPrefabs = value; }
    }
    [SerializeField]
    Sprite icon;
    public Sprite Icon
    {
        get { return icon; }
        set { icon = value; }
    }
}
