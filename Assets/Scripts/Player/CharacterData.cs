using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CharacterData", menuName = "CharacterData", order = 0)]

public class CharacterData : ScriptableObject
{
    [SerializeField]
    Sprite icon;
    public Sprite Icon {get => icon; private set => icon = value;}
    [SerializeField]
    new string name;
    public string Name {get => name; private set => name = value;}
    [SerializeField]
    WeaponData startingWeapon;
    public WeaponData StartingWeapon {get => startingWeapon; private set => startingWeapon = value;}
    [System.Serializable]
    public struct Stats{
        public float maxHeal, recovery, moveSpeed;
        public float might, speed, magnet;
        public Stats(float maxHeal = 1000,float recovery=0,float moveSpeed=1f,float might=1f,float speed=1f,float magnet=30f){
            this.maxHeal = maxHeal;
            this.recovery = recovery;
            this.moveSpeed = moveSpeed;
            this.might = might;
            this.speed = speed;
            this.magnet = magnet;
        }
        public static Stats operator +(Stats s1, Stats s2){
            s1.maxHeal += s2.maxHeal;
            s1.recovery += s2.maxHeal;
            s1.moveSpeed += s2.maxHeal;
            s1.might += s2.maxHeal;
            s1.speed += s2.maxHeal;
            s1.magnet += s2.maxHeal;
            return s1;
        }
    }
    public  Stats stats = new Stats(1000);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
