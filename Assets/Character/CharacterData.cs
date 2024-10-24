using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
[CreateAssetMenu(fileName = "CharacterData", menuName = "CharacterData", order = 0)]
[System.Serializable]
public class CharacterData : ScriptableObject
{
    [SerializeField]
    Sprite icon;
    public Sprite Icon {get => icon; private set => icon = value;}
    [SerializeField]
    private string charname; // Thêm thuộc tính tên
    public string Name {get => charname; private set => charname = value;}
    [SerializeField]
    private string informationCharacter;
    public string InformationCharacter { get => informationCharacter; private set => informationCharacter = value; }
    [SerializeField]
    WeaponData startingWeapon;
    [SerializeField]
    public WeaponData StartingWeapon {get => startingWeapon; private set => startingWeapon = value;}
    [SerializeField]
    public Sprite sprite;
    [SerializeField]
    public RuntimeAnimatorController animatorController;

    [SerializeField]
    public Sprite title_Character;
    [SerializeField]
    public List<CostumeData> costumes;
        [SerializeField]
    private int level = 1; // Thêm thuộc tính cấp độ
    public int Level 
    {
        get => level; 
        set => level = value;
    }
        public void UpgradeLevel()
    {
        if (Level < 7)
        {
            Level++;
        }
    }

    public void SetLevel(int newLevel)
    {
        Level = newLevel;
    }
    [System.Serializable]
    //struct dùng để lưu trữ các giá trị của stats, thường để biểu diễn các giá trị cố định và nhẹ nhàng
    // struct không thể kế thừa từ class khác cũng như class không thể kế thừa từ struct, struct có thể thực hiện interface
    public struct Stats{
        public float maxHeal, recovery, armor;
        [Range(-1,10)] public float moveSpeed, might, area;
        // [Range(-1,5)] public float speed, duration;
        [Range(-1,10)] public int amount;
        [Range(-1,10)] public float cooldown;
        // [Min(-1)] public float luck,growth,greed,curse;
        public float magnet;
        public int revival;
        [Range(-1, 1)] public float criticalChance;     // Xác suất chí mạng, giá trị từ 0 đến 1
        [Min(1)] public float criticalMultiplier;      // Hệ số nhân chí mạng, giá trị >= 1
        [Min(0)] public float expMultiplier;
        [Min(0)] public float curse;                   // Hệ số nhân sát thương, giá trị >= 0
        public float healMultiplier;                   // Hệ số nhân hồi máu, giá trị >= 0
        // đây là phương thức cộng 2 stats với nhau
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
            s1.curse += s2.curse;
            s1.magnet += s2.magnet;
            s1.revival += s2.revival;
            // các thuộc tính của chí mạng
            s1.criticalChance = Mathf.Max(0, s1.criticalChance + s2.criticalChance);
            s1.criticalMultiplier += s2.criticalMultiplier;
            s1.expMultiplier += s2.expMultiplier;
            s1.healMultiplier += s2.healMultiplier; 
            return s1;
        }
    }
    public  Stats stats = new Stats{
        maxHeal = 100, moveSpeed = 1, might = 1, amount = 1, area = 1, cooldown =1, healMultiplier = 1
    };

    public void AdjustStatsBasedOnLevel()
    {
        // Example logic to adjust stats based on level
        // You can customize this logic based on your game's requirements
        stats.maxHeal = stats.maxHeal + (Level - 1) * 10; // Increase maxHeal by 10 for each level
        stats.moveSpeed = 1 + (Level - 1) * 0.1f; // Increase moveSpeed by 0.1 for each level
        // Add more adjustments for other stats as needed
    }
}
