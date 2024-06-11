using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsychoAxeBehaviour : RangeBehaviour
{
    List<GameObject> markerEnemies;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }
    // protected override void OnTriggerEnter2D(Collider2D other) {
    //     if(other.CompareTag("Enemy") && !markerEnemies.Contains(other.gameObject)){
    //         EnemyStats enemyStats = other.GetComponent<EnemyStats>();
    //         enemyStats.TakeDamage(GetCurrrentDamage(),transform.position);
    //     }
    // }
}
