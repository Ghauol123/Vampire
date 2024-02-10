using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObject/Enemy", order = 0)]
public class EnemyScriptableObject : ScriptableObject
{
    [SerializeField]
    float moveSpeed;
    public float MoveSpeed {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }
    [SerializeField]
    int maxHealh;
    public int maxhealt {
        get { return maxHealh; }
        set { maxHealh = value; }
    }
    [SerializeField]
    int damage;
    public int Damage {
        get { return damage; }
        set { damage = value; }
    }
}
