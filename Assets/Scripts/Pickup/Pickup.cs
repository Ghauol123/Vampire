using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.VFX;

public class Pickup : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string id;
    [ContextMenu("Generate ID")]
    private void GenerateID()
    {
        id = Guid.NewGuid().ToString();
    }
    private SpriteRenderer spriteRenderer;
    public float lifeSpan = 0.5f;
    protected PlayerStats target; // if the pickup has a target, then fly toward the target
    protected float speed; // tốc độ ba
    [Header("Exp and Heal")]
    public int Exp;
    public int Heal;
    public bool Coolected = false;
    EnemyStats enemyStats;
    // Tạo ID khi đối tượng được khởi tạo
    protected virtual void Awake()
    {
        GenerateID();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected virtual void Update(){
        if(target){
            // tính toán khoảng cách để di chuyển đến người chơi
            Vector2 distance = target.transform.position - transform.position;
            if(distance.sqrMagnitude > speed*speed*Time.deltaTime){
                // cho biết khoảng cách và hướng của đường đi
                transform.position += (Vector3)distance.normalized*speed*Time.deltaTime;
            }
            else{
                Destroy(gameObject);
            }
        }
    }
    public virtual bool Collect(PlayerStats target, float speed, float lifeSpan = 0f){
        if(!this.target){
            this.target = target;
            this.speed = speed;
            if(lifeSpan > 0) this.lifeSpan = lifeSpan;
            Destroy(gameObject, Math.Max(0.01f,this.lifeSpan));
            return true;
        }
        return false;
    }
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         Debug.Log("collect");
    //         Destroy(gameObject);
    //     }
    // }
    protected virtual void OnDestroy() {
        if(!target) return;
        if(Exp != 0 ) target.IncreaseExperience(Exp);
        if(Heal != 0 ) target.RestoreHeal(Exp);
    }

public void LoadGameData(GameData gameData)
{
    if (gameData == null)
    {
        Debug.LogWarning("GameData is null.");
        return;
    }

    if (gameData.expCollected == null)
    {
        Debug.LogWarning("expCollected dictionary is null.");
        return;
    }

    if (gameData.expCollected.TryGetValue(id, out bool coolected))
    {
        if (coolected)
        {
            spriteRenderer.gameObject.SetActive(false);
        }
    }
    else
    {
        Debug.LogWarning("ID not found in expCollected dictionary.");
    }
}


public void SaveGameData(ref GameData gameData)
{
    // Kiểm tra nếu gameData là null
    if (gameData == null)
    {
        Debug.LogWarning("GameData is null.");
        return;
    }

    // Kiểm tra nếu expCollected là null và khởi tạo nếu cần
    if (gameData.expCollected == null)
    {
        Debug.LogWarning("expCollected dictionary is null. Initializing it.");
        gameData.expCollected = new SerializableDictionary<string, bool>();
    }

    // Xóa id nếu nó đã tồn tại trong từ điển
    if (gameData.expCollected.ContainsKey(id))
    {
        gameData.expCollected.Remove(id);
    }

    // Thêm hoặc cập nhật giá trị với id
    gameData.expCollected[id] = Coolected;
}

}
