using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderCookingBehaviour : MeleeBehaviour
{
    List<GameObject> markerEnemies;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        markerEnemies = new List<GameObject>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected override void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Enemy") && !markerEnemies.Contains(other.gameObject)){
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            enemyStats.TakeDamage(GetCurrrentDamage());
            markerEnemies.Add(other.gameObject); // cooldown cho việc một đối tượng sẽ không bị dính dame liên tục từ một vũ khí cận chiến
        }
    }
    public void UpSize(){
        spriteRenderer.transform.localScale *= 1.2f; 
    }
}
