using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeWeapon : ProjectileWeapon
{
    protected override float GetSpawnAngle(){
        int offset = currentAttackCount > 0 ? currentStats.number - currentAttackCount : 0;
        return 90f - Math.Sign(movement.lastMovedVector.x) * (5*offset); 
    }
    protected override Vector2 GetSpawnOffset(float spawnAngle = 0){
        return new Vector2(
            UnityEngine.Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            UnityEngine.Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}
