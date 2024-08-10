using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStats;

public class GameData
{
    public CharacterData.Stats baseStat;
    public CharacterData.Stats actualStat;
    public Vector3 playerPosition;
    public SerializableDictionary<string, bool> expCollected;
    public float magnet;
    public int experience;
    public int level;
    public float currentHealth;
    public int score;
    public string spriteRendererSpriteName;
    public string animatorControllerName;
    public List<levelRange> levelRanges;
    public float timeSurvival;
    public List<WeaponData> weaponsInSlots;
    public List<PassiveData> passiveItemsInSlots;
    public List<WeaponData> availableWeapons;
    public List<PassiveData> availablePassiveItems;
    public List<int> weaponLevels; // Store the levels of weapons
    public List<int> passiveLevels; // Store the levels of passive items
    public List<EnemiesData> enemiesData;
    public List<WaveDataSave> waveDataSaves = new List<WaveDataSave>();



    public GameData(){
        baseStat = actualStat = new CharacterData.Stats();
        playerPosition = Vector3.zero;
        experience = 0;
        level = 0;
        currentHealth = 0;
        score = 0;
        spriteRendererSpriteName = "";
        animatorControllerName = "";
        levelRanges = new List<levelRange>();
        timeSurvival = 0f;
        weaponsInSlots = new List<WeaponData>();
        passiveItemsInSlots = new List<PassiveData>();
        availableWeapons = new List<WeaponData>();
        availablePassiveItems = new List<PassiveData>();
        expCollected = new SerializableDictionary<string, bool>();
        magnet = 0;
        weaponLevels = new List<int>();
        passiveLevels = new List<int>();
        enemiesData = new List<EnemiesData>();
        waveDataSaves = new List<WaveDataSave>();
    }
}

