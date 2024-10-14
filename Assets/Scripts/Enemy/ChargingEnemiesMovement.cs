using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemiesMovement : EnemyMovement
{
    Vector2 chargeDirection;
    // Start is called before the first frame update
    protected override void Start(){
        base.Start();
        chargeDirection = (player.position - transform.position).normalized;
    }
    public void Move(){
        transform.position += (Vector3)chargeDirection * enemyStats.Actual.moveSpeed * Time.deltaTime;
    }
}
