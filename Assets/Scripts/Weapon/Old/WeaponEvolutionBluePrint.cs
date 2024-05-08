using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponEvolutionBluePrint", menuName = "ScriptableObject/evolvedWeapon", order = 0)]
public class WeaponEvolutionBluePrint : ScriptableObject {
    public WeaponScriptableObject baseWeaponData;
    public PassiveItemsScriptableObject catalystPassiveItemData;
    public WeaponScriptableObject evolvelWeaponData;
    public GameObject evovelWeapon;
}

