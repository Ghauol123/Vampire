using System.Threading.Tasks;
using UnityEngine;

public class StatUpgrade
{
    public string StatType { get; private set; }
    public int Level { get; private set; }
    public int UpgradeCost { get; private set; }
    private float UpgradeMultiplier { get; set; }

    public StatUpgrade(string statType, int initialLevel, int initialCost, float upgradeMultiplier)
    {
        StatType = statType;
        Level = initialLevel;
        UpgradeCost = initialCost;
        UpgradeMultiplier = upgradeMultiplier;
    }

    public async Task<bool> TryUpgrade()
    {
        int currentCoin = await FirebaseLoadCoin.instance.GetCurrentCoinFromFirebase();
        if (currentCoin >= UpgradeCost)
        {
            currentCoin -= UpgradeCost;
            await FirebaseLoadCoin.instance.UpdateCoinInFirebase(currentCoin);

            Level++;
            UpgradeCost = Mathf.RoundToInt(UpgradeCost * UpgradeMultiplier);

            return true;
        }
        else
        {
            Debug.Log($"Không đủ tiền để nâng cấp {StatType}.");
            return false;
        }
    }
}
