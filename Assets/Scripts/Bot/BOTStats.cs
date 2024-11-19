using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOTStats : MonoBehaviour
{
    public static BOTStats instancebot;
    BOTInventory bOTInventory;
    BOTPickup botPickUp;
    PlayerPickUp playerPickUp;
    public CharacterData cst;
    [HideInInspector]
    public float[] position;
    public CharacterData.Stats baseStat;
    [SerializeField]
    public CharacterData.Stats actualStats;
        public CharacterData.Stats Stats
    {
        get{return actualStats;}
        set{
            actualStats = value;
        }
    }

    public SpriteRenderer spriteRenderer;
    public Animator animator; 
    protected CostumeData costumeData;
    public int score = 0;
    public int coin = 0;
    public int killnumber = 0;
    protected void Awake()
    {
           if (instancebot == null)
        {
            instancebot = this;
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeNewGame();
    }
    public void InitializeNewGame()
    {
        // Select a random character for the bot
        CharacterData botCharacter = SelectRandomCharacter();

        // Initialize the bot with the selected character's data
        cst = botCharacter;
        costumeData = botCharacter.costumes[0]; // Assuming the first costume is used
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Set sprite and animator for the bot
        spriteRenderer.sprite = costumeData.CostumeSprite;
        animator.runtimeAnimatorController = costumeData.CostumeAnimator;

        bOTInventory = GetComponent<BOTInventory>();
        playerPickUp = GetComponentInChildren<PlayerPickUp>();
        baseStat = actualStats = cst.currentStat;
        playerPickUp.SetMagnet(actualStats.magnet);

        // Add starting weapon to the bot's inventory
        bOTInventory.availableWeapons.Insert(0, cst.StartingWeapon);
        bOTInventory.Add(cst.StartingWeapon);
    }

    private CharacterData SelectRandomCharacter()
    {
        // Return a random character from the list of all available characters
        List<CharacterData> allCharacters = FirebaseController.instance.characterDataList;
        int randomIndex = UnityEngine.Random.Range(0, allCharacters.Count);
        return allCharacters[randomIndex];
    }

    private List<CharacterData> GetAllCharacters()
    {
        // Return a list of all available characters
        // This is a placeholder, replace with your actual character list
        return new List<CharacterData> { /* your characters here */ };
    }
        public void RecalculatedStats()
    {
        actualStats = baseStat;
        foreach(BOTInventory.Slot s in bOTInventory.passiveSlot)
        {
            Passive p = s.item as Passive;
            if(p)
            {
                actualStats += p.GetBoots();
                Debug.Log("Cộng thông số");
            }
        }
        playerPickUp.SetMagnet(actualStats.magnet);
    }
    public void ApplyStatsUpgrade(){
         actualStats = baseStat;
        foreach(BOTInventory.StatSlot s in bOTInventory.statSlot)
        {
            Stats p = s.item as Stats;
            if(p)
            {
                actualStats += p.GetBoots();
                Debug.Log("Cộng thông số");
            }
        }
    }
       public void IncreaseKillnumber(int killnumber)
    {
        this.killnumber += killnumber;
    }
}
