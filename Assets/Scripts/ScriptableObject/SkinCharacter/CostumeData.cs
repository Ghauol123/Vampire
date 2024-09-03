using UnityEngine;

[CreateAssetMenu(fileName = "CostumeData", menuName = "CostumeData", order = 1)]
public class CostumeData : ScriptableObject
{
    [SerializeField]
    private string costumeName;
    public string CostumeName { get => costumeName; private set => costumeName = value; }

    [SerializeField]
    private Sprite costumeSprite;
    public Sprite CostumeSprite { get => costumeSprite; private set => costumeSprite = value; }

    [SerializeField]
    private RuntimeAnimatorController costumeAnimator;
    public RuntimeAnimatorController CostumeAnimator { get => costumeAnimator; private set => costumeAnimator = value; }

    [SerializeField]
    private Sprite[] m_SpriteArray;
    public Sprite[] MSpriteArray { get => m_SpriteArray; private set => m_SpriteArray = value; }
    // Thêm các thuộc tính hoặc hiệu ứng khác nếu cần

    [SerializeField]
    private CharacterData characterData;
    public CharacterData CharacterData {get => characterData; private set => characterData = value;}
}
