using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterScriptableObject cst;

    float currentHeal;
    float currentRecovery;
    float currentSpeed;
    float currentMight;
    float currentProjectileSpeed;

    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

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
        currentHeal = cst.Maxheal;
        currentMight = cst.Might;
        currentProjectileSpeed = cst.ProjectileSpeed;
        currentRecovery = cst.Recovery;
        currentSpeed = cst.MoveSpeed;
    }
    private void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrese;
        // đặt max kinh nghiệp đầu tiên là max kinh nghiệm ở level 0
    }
    private void Update()
    {
        if (invicibleTime > 0)
        {
            invicibleTime -= Time.deltaTime;

        }
        else if(isInvicible)
        {
            isInvicible = false;
        }
    }
    public void IncreaseExperience(int amount)
    {
        experience += amount;
        levelCheckerUp();
    }
    public void IncreaseHeal(float heal){
        if(currentHeal < cst.Maxheal){
            currentHeal += heal;
            if(currentHeal > cst.Maxheal){
                currentHeal = cst.Maxheal;
            }
        }    
        else if(currentHeal == cst.Maxheal){
            return;
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
}
