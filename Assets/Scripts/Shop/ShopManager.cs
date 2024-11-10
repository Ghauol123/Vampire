using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public Transform buySkinTransform;
    public Transform upgradeCharTransform;
    public Button buySkinButton;
    public Button upgradeCharButton;

    private void Start()
    {
        // Thêm listeners cho các nút
        buySkinButton.onClick.AddListener(ShowBuySkinPanel);
        upgradeCharButton.onClick.AddListener(ShowUpgradeCharPanel);

        // Mặc định hiện upgrade panel, ẩn buy skin panel
        ShowUpgradeCharPanel();
    }

    private void ShowBuySkinPanel()
    {
        buySkinTransform.gameObject.SetActive(true);
        upgradeCharTransform.gameObject.SetActive(false);
    }

    private void ShowUpgradeCharPanel()
    {
        buySkinTransform.gameObject.SetActive(false);
        upgradeCharTransform.gameObject.SetActive(true);
    }
}
