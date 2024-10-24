using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using TMPro;

public class CharacterAvatar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject infoPanel; // Panel để hiển thị thông tin nhân vật
    public Text characterNameText;
    public CharacterData characterData; // Dữ liệu nhân vật
    public Text weaponNameText;
    public Text inforWeaponText;
    public Image avatarWeapon;
    public Text characterInformation;

    public TextMeshProUGUI HPText;
    public TextMeshProUGUI ATKText;
    public TextMeshProUGUI SPDText;
    public TextMeshProUGUI CRTText;

    public RuntimeAnimatorController animatorController;
    public Sprite title_Character;
    public GameObject characterAnimation;
    void Start()
    {
        infoPanel.SetActive(false); // Ẩn panel khi bắt đầu
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoPanel.SetActive(true);
        characterNameText.text = characterData.Name;
        title_Character = characterData.sprite;
        animatorController = characterData.animatorController;
        weaponNameText.text = characterData.StartingWeapon.baseStats.name;
        inforWeaponText.text = characterData.StartingWeapon.baseStats.description;
        characterInformation.text = characterData.InformationCharacter;
        avatarWeapon.sprite = characterData.StartingWeapon.baseStats.Icon;
        characterAnimation.GetComponent<Animator>().runtimeAnimatorController = animatorController;
        characterAnimation.GetComponent<SpriteRenderer>().sprite = title_Character;

        HPText.text = "HP: " + characterData.stats.maxHeal;
        ATKText.text = "ATK: " + characterData.stats.might;
        SPDText.text = "SPD: " + characterData.stats.moveSpeed;
        CRTText.text = "CRT: " + characterData.stats.criticalChance;

        Func_PlayUIAnim();


        // Cập nhật các thông số khác nếu cần
        // Ví dụ: characterStatsText.text = "HP: " + characterData.stats.maxHeal;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // infoPanel.SetActive(false);
        Func_StopUIAnim();
    }
    public Image m_Image;

    public Sprite[] m_SpriteArray;
    public float m_Speed = .02f;

    private int m_IndexSprite;
    Coroutine m_CorotineAnim;
    bool IsDone;
    public void Func_PlayUIAnim()
    {
        IsDone = false;
        StartCoroutine(Func_PlayAnimUI());
    }

    public void Func_StopUIAnim()
    {
        IsDone = true;
        StopCoroutine(Func_PlayAnimUI());
    }
    IEnumerator Func_PlayAnimUI()
    {
        yield return new WaitForSeconds(m_Speed);
        if (m_IndexSprite >= m_SpriteArray.Length)
        {
            m_IndexSprite = 0;
        }
        m_Image.sprite = m_SpriteArray[m_IndexSprite];
        m_IndexSprite += 1;
        if (IsDone == false)
            m_CorotineAnim = StartCoroutine(Func_PlayAnimUI());
    }
}
