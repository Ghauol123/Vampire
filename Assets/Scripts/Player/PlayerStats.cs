using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterScriptableObject cst;
    // [HideInInspector]
    public float currentHeal;
    [HideInInspector]
    public float currentRecovery;
    [HideInInspector]
    public float currentSpeed;
    [HideInInspector]
    public float currentMight;
    [HideInInspector]
    public float currentProjectileSpeed;
    [HideInInspector]
    public float currentManget;

    Vector2 newPosition;

    public List<GameObject> startWeapon;

    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    SpriteRenderer spr;
    Animator playerAnimation;

    [System.Serializable]
    public class levelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrese;
    }
    [Header("I-Frames")]
    public float invicibleDuration;
    float invicibleTime;
    bool isInvicible;
    public List<levelRange> levelRanges;
    private void Awake()
    {
        // cst = CharacterSelected.GetData();
        // CharacterSelected.instance.DestroyInstance();
        currentHeal = cst.Maxheal;
        currentMight = cst.Might;
        currentProjectileSpeed = cst.ProjectileSpeed;
        currentRecovery = cst.Recovery;
        currentSpeed = cst.MoveSpeed;
        currentManget = cst.Magnet;
        newPosition = new Vector2(transform.position.x, -0.40f);
        SpawnWeapon(cst.StartingWeapon);
    }
    private void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrese;
        // đặt max kinh nghiệp đầu tiên là max kinh nghiệm ở level 0
        // playerAnimation = GetComponent<Animator>();
        // playerAnimation.runtimeAnimatorController = cst._Animation;
        // spr = GetComponent<SpriteRenderer>();
        // spr.sprite = cst.Image;
    }
    private void Update()
    {
        if (invicibleTime > 0)
        {
            invicibleTime -= Time.deltaTime;

        }
        else if (isInvicible)
        {
            isInvicible = false;
        }
        Recovery();
    }
    public void IncreaseExperience(int amount)
    {
        experience += amount;
        levelCheckerUp();
    }
    public void IncreaseHeal(float heal)
    {
        if (currentHeal < cst.Maxheal)
        {
            currentHeal += heal;
            if (currentHeal > cst.Maxheal)
            {
                currentHeal = cst.Maxheal;
            }
        }
    }
    public void Recovery()
    {
        if (currentHeal < cst.Maxheal)
        {
            currentHeal += currentRecovery * Time.deltaTime;
            if (currentHeal > cst.Maxheal)
            {
                currentHeal = cst.Maxheal;
            }
        }
    }
    public void levelCheckerUp()
    {
        if (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;
            int experienceCapIncrese = 0;
            foreach (levelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrese = range.experienceCapIncrese;
                    break;
                }
            }
            experienceCap += experienceCapIncrese;
        }
    }
    public void TakeDamage(float dmg)
    {
        if (isInvicible == false)
        {
            currentHeal -= dmg;
            invicibleTime = invicibleDuration;
            isInvicible = true;
            if (currentHeal <= 0)
            {
                Kill();
            }
        }
    }
    public void Kill()
    {
        Destroy(gameObject);
    }
    public void SpawnWeapon(GameObject weapon)
    {
        GameObject spawnWeapon = Instantiate(weapon, newPosition, Quaternion.identity);
        spawnWeapon.transform.SetParent(transform);
        startWeapon.Add(spawnWeapon);
    }

}
