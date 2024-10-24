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
        public int Amount; // Thêm thuộc tính để xác định số lượng EXP rơi
        public int Exp;
    }

    public List<Drops> drops;

    public void DropItems(Vector3 position) {
        float randomNumber = Random.Range(0f, 100f);
        List<Drops> possibleDrops = new List<Drops>();
        
        foreach (Drops rate in drops) {
            if (randomNumber <= rate.dropRate) {
                possibleDrops.Add(rate);
            }
            rate.itemsPrefabs.GetComponent<Pickup>().Exp = rate.Exp;
        }

        if (possibleDrops.Count > 0) {
            foreach (Drops drop in possibleDrops) {
                for (int i = 0; i < drop.Amount; i++) {
                    Vector3 randomDropPosition = position + Random.insideUnitSphere * 2f;
                    GameObject pooledObject = ObjectPool.Instance.GetObject(drop.itemsPrefabs);
                    if (pooledObject != null) {
                        pooledObject.transform.position = randomDropPosition;
                        pooledObject.SetActive(true);
                    }
                }
            }
        }
    }
}
