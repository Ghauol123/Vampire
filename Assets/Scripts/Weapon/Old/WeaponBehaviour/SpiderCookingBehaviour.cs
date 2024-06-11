using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderCookingBehaviour : MeleeBehaviour
{
    List<GameObject> markerEnemies;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        markerEnemies = new List<GameObject>();
    }
    // protected override void OnTriggerEnter2D(Collider2D other) {
    //     if(other.CompareTag("Enemy") && !markerEnemies.Contains(other.gameObject)){
    //         EnemyStats enemyStats = other.GetComponent<EnemyStats>();
    //         enemyStats.TakeDamage(GetCurrrentDamage(),transform.position);
    //         markerEnemies.Add(other.gameObject); // cooldown cho việc một đối tượng sẽ không bị dính dame liên tục từ một vũ khí cận chiến
    //     }
    // }
}
