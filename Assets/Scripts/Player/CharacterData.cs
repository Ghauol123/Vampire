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
    public Sprite title_Character;
    [SerializeField]
    public List<CostumeData> costumes;

    [SerializeField]
    public CostumeData defaultCostume;

    [SerializeField]
    private int level = 1;
    public int Level { get => level; private set => level = Mathf.Clamp(value, 1, 7); }

    [SerializeField]
    private Stats baseStats;
    public Stats CurrentStats { get; private set; }

    private const float UPGRADE_PERCENTAGE = 0.05f;
    private const int MAX_LEVEL = 7;

        [SerializeField]
    private int upgradeCost = 100; // Giá cơ bản để nâng cấp, có thể điều chỉnh
    public int UpgradeCost => upgradeCost * Level; // Giá tăng theo cấp độ


    [System.Serializable]
    //struct dùng để lưu trữ các giá trị của stats, thường để biểu diễn các giá trị cố định và nhẹ nhàng
    // struct không thể kế thừa từ class khác cũng như class không thể kế thừa từ struct, struct có thể thực hiện interface
    public struct Stats{
        public float maxHeal, recovery, armor;
        [Range(-1,10)] public float moveSpeed, might, area;
        // [Range(-1,5)] public float speed, duration;
        [Range(-1,10)] public int amount;
        [Range(-1,1)] public float cooldown;
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
        public static Stats ApplyUpgrade(Stats original, float percentage)
        {
            Stats upgraded = original;
            upgraded.maxHeal *= (1 + percentage);
            upgraded.recovery *= (1 + percentage);
            upgraded.armor *= (1 + percentage);
            upgraded.moveSpeed *= (1 + percentage);
            upgraded.might *= (1 + percentage);
            upgraded.area *= (1 + percentage);
            upgraded.amount = Mathf.RoundToInt(upgraded.amount * (1 + percentage));
            upgraded.cooldown *= (1 - percentage); // Cooldown giảm khi nâng cấp
            upgraded.magnet *= (1 + percentage);
            upgraded.revival = Mathf.RoundToInt(upgraded.revival * (1 + percentage));
            upgraded.criticalChance = Mathf.Min(1, upgraded.criticalChance * (1 + percentage));
            upgraded.criticalMultiplier *= (1 + percentage);
            upgraded.expMultiplier *= (1 + percentage);
            upgraded.healMultiplier *= (1 + percentage);
            return upgraded;
        }
    }
    public  Stats stats = new Stats{
        maxHeal = 100, moveSpeed = 1, might = 1, amount = 1, area = 1, cooldown =1, healMultiplier = 1
    };
    private void OnEnable()
    {
        UpdateStats();
    }

    public bool UpgradeLevel()
    {
        if (Level >= MAX_LEVEL) return false;
        Level++;
        UpdateStats();
        return true;
    }

    private void UpdateStats()
    {
        CurrentStats = baseStats;
        for (int i = 1; i < Level; i++)
        {
            CurrentStats = Stats.ApplyUpgrade(CurrentStats, UPGRADE_PERCENTAGE);
        }
    }

    // Phương thức này sẽ được gọi khi người chơi muốn nâng cấp nhân vật
    public async Task<bool> TryUpgrade()
    {
        if (Level >= MAX_LEVEL)
        {
            Debug.Log("Đã đạt cấp độ tối đa.");
            return false;
        }

        int currentCost = UpgradeCost;

        // Kiểm tra xem FirebaseLoadCoin đã được khởi tạo chưa
        if (FirebaseLoadCoin.instance == null || string.IsNullOrEmpty(FirebaseLoadCoin.instance.userId))
        {
            Debug.LogError("FirebaseLoadCoin chưa được khởi tạo hoặc người dùng chưa đăng nhập.");
            return false;
        }

        // Lấy số tiền hiện tại của người chơi
        int currentCoin = await FirebaseLoadCoin.instance.GetCurrentCoinFromFirebase();

        if (currentCoin >= currentCost)
        {
            // Trừ tiền
            currentCoin -= currentCost;

            // Cập nhật số tiền mới vào Firebase
            await FirebaseLoadCoin.instance.UpdateCoinInFirebase(currentCoin);

            // Nâng cấp nhân vật
            if (UpgradeLevel())
            {
                Debug.Log($"Nâng cấp thành công. Cấp độ mới: {Level}");
                return true;
            }
        }
        else
        {
            Debug.Log("Không đủ tiền để nâng cấp.");
        }

        return false;
    }
}
