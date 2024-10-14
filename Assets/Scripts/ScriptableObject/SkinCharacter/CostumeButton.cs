using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CostumeButton : MonoBehaviour
{
    public CostumeData costumeData;
    
    [SerializeField]
    private Image costumeImage;
    
    // [SerializeField]
    // private TextMeshProUGUI costumeName;
    
    [SerializeField]
    private TextMeshProUGUI costumePrice;

    private UISpriteAnimation spriteAnimation;
    private int costumeIndex;
    private CharacterSelected characterSelected;
    public void SetupButton()
    {
        if (costumeData != null)
        {
            costumeImage.sprite = costumeData.CostumeSprite;
            costumePrice.text = costumeData.Price.ToString();

            // Setup UISpriteAnimation
            SetupSpriteAnimation();
        }
    }
        public void SetupButton(CostumeData costumedata, int index, CharacterSelected charSelected)
    {
        costumeData = costumedata;
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

    private void SetupSpriteAnimation()
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
