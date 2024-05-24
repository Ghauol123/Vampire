using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraWeapon : Weapon
{
    protected Aura currentAura;
    protected override void Update()
    {
    }
    public override void onEquip()
    {
        // try to replace the aura the weapon has with a new one
        if(currentStats.auraPrefabs){
            if(currentAura) Destroy(currentAura);
            currentAura =  Instantiate(currentStats.auraPrefabs, transform);
            currentAura.weapon = this;
            currentAura.owner = owner;
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }
    }
    public override void OnUnEquip()
    {
        if(currentAura) Destroy(currentAura);
    }
    public override bool DoLevelUp()
    {
        if(!base.DoLevelUp()) return false;
        // If there is an aura attached to this weapon, we update the aura
        if(currentAura){
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }
        return true;
    }
}
