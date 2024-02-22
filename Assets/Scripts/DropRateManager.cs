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
    }
    public List<Drops> drops;
    private void OnDestroy() {
        if(!gameObject.scene.isLoaded){
            return;
        }
        float randomNumber = Random.Range(0f,100f);// random 1 số
        List<Drops> possibleDrops = new List<Drops>();// tạo 1 list mới
        foreach(Drops rate in drops){// check drops trong list
            if(randomNumber <= rate.dropRate){ 
                possibleDrops.Add(rate);// nếu số random < rate add vào list
            }
        }
        if(possibleDrops.Count > 0){
            Drops drops = possibleDrops[Random.Range(0,possibleDrops.Count)];// random các exp trong list để chọn
            Instantiate(drops.itemsPrefabs, transform.position, Quaternion.identity);// rớt ra exp
        }
    }
}
