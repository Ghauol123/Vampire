using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops{
        public string name;
        public GameObject itemsPrefabs;
        public float dropRate;
        public int expAmount; // Thêm thuộc tính để xác định số lượng EXP rơi
    }

    public List<Drops> drops;

    private void OnDestroy() {
        if(!gameObject.scene.isLoaded){
            return;
        }

        float randomNumber = Random.Range(0f,100f);
        List<Drops> possibleDrops = new List<Drops>();
        
        foreach(Drops rate in drops){
            if(randomNumber <= rate.dropRate){
                possibleDrops.Add(rate);
            }
        }

        if(possibleDrops.Count > 0){
            foreach (Drops drop in possibleDrops) {
                // Instantiate item và set vị trí rơi
                for (int i = 0; i < drop.expAmount; i++) {
                    Vector3 randomDropPosition = transform.position + Random.insideUnitSphere * 2f; // Điều chỉnh bán kính rơi tại đây
                    Instantiate(drop.itemsPrefabs, randomDropPosition, Quaternion.identity);
                }
            }
        }

    }
}
