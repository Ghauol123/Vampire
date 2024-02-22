using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
[CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "ScriptableObject/Character", order = 0)]
public class CharacterScriptableObject : ScriptableObject
{
    [SerializeField]
    private GameObject startingWeapon;
    public GameObject StartingWeapon
    {
        get { return startingWeapon; }
        set { startingWeapon = value; }
    }
    [SerializeField]
    private float maxheal;
    public float Maxheal
    {
        get { return maxheal; }
        set { maxheal = value; }
    }
    [SerializeField]
    private float recovery;
    public float Recovery
    {
        get { return recovery; }
        set { recovery = value; }
    }
    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }
    [SerializeField]
    private float might;
    public float Might
    {
        get { return might; }
        set { might = value; }
    }
    [SerializeField]
    private float projectileSpeed;
    public float ProjectileSpeed
    {
        get { return projectileSpeed; }
        set { projectileSpeed = value; }
    }
    [SerializeField]
    private float magnet;
    public float Magnet
    {
        get { return magnet; }
        set { magnet = value; }
    }
    [SerializeField]
    Sprite icon;
    public Sprite Icon
    {
        get { return icon; }
        set { icon = value; }
    }
    [SerializeField]
    string nameCharacter;
    public string NameCharacter
    {
        get { return nameCharacter; }
        set { nameCharacter = value; }
    }
}
