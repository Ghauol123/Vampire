using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CostumeButton : MonoBehaviour
{
    public CostumeData costumeData;
    
    [SerializeField]
    public Image costumeImage;
    [SerializeField]
    public TextMeshProUGUI costumeName;

    
    [SerializeField]
    private TextMeshProUGUI UpgradeMoneyText;
    
    private UISpriteAnimation spriteAnimation;
    private int costumeIndex;
    private CharacterSelected characterSelected;
    CharacterButtonHandler characterButtonHandler;
    private void Start() {
        characterButtonHandler = FindObjectOfType<CharacterButtonHandler>();
    }
    public void SetupButton()
    {
        if (costumeData != null)
        {
            costumeImage.sprite = costumeData.CostumeSprite;
            // Setup UISpriteAnimation
            SetupSpriteAnimation();
        }
    }
        public void SetupButton(CostumeData costumedata, int index, CharacterSelected charSelected)
    {
        costumeData = costumedata;
        costumeName.text = costumeData.CostumeName;
        costumeImage.sprite = costumeData.CostumeSprite;
        costumeIndex = index;
        characterSelected = charSelected;
        SetupSpriteAnimation();

        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }
        private void OnButtonClick()
    {
        characterSelected.SelectSkin(costumeIndex);
        characterSelected.playbutton.SetActive(true);
    }

    public void SetupSpriteAnimation()
    {
        // Get or add UISpriteAnimation component
        spriteAnimation = costumeImage.gameObject.GetComponent<UISpriteAnimation>();
        if (spriteAnimation == null)
        {
            spriteAnimation = costumeImage.gameObject.AddComponent<UISpriteAnimation>();
        }

        // Configure UISpriteAnimation
        spriteAnimation.Init(costumeImage, costumeData, 0.02f);
    }
}
