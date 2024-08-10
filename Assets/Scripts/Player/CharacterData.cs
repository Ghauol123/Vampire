using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CharacterData", menuName = "CharacterData", order = 0)]
[System.Serializable]
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
    [SerializeField]
    public WeaponData StartingWeapon {get => startingWeapon; private set => startingWeapon = value;}
    [SerializeField]
    public Sprite sprite;
    [SerializeField]
    public RuntimeAnimatorController animatorController;
    [SerializeField]
    public WeaponScriptableObject weaponScriptableObject;

    [System.Serializable]
    public struct Stats{
        public float maxHeal, recovery, armor;
        [Range(-1,10)] public float moveSpeed, might, area;
        // [Range(-1,5)] public float speed, duration;
        [Range(-1,10)] public int amount;
        [Range(-1,1)] public float cooldown;
        // [Min(-1)] public float luck,growth,greed,curse;
        public float magnet;
        public int revival;

        public static Stats operator +(Stats s1, Stats s2){
            s1.maxHeal += s2.maxHeal;
            s1.recovery += s2.recovery;
            s1.armor += s2.armor;
            s1.moveSpeed += s2.moveSpeed;
            s1.might += s2.might;
            s1.area += s2.area;
            // s1.speed += s2.speed;
            // s1.duration += s2.duration;
            s1.amount += s2.amount;
            s1.cooldown += s2.cooldown;
            // s1.luck += s2.luck;
            // s1.growth += s2.growth;
            // s1.greed += s2.greed;
            // s1.curse += s2.curse;
            s1.magnet += s2.magnet;
            return s1;
        }
    }
    public  Stats stats = new Stats{
        maxHeal = 100, moveSpeed = 1, might = 1, amount = 1, area = 1, cooldown =1,
    };
}
