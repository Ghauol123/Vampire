using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (objectPools.TryGetValue(prefab.name, out Queue<GameObject> objectPool))
        {
            while (objectPool.Count > 0)
            {
                GameObject obj = objectPool.Dequeue();
                if (obj != null) // Check if the object is not null
                {
                    obj.SetActive(true);
                    return obj;
                }
            }
        }

        return CreateNewObject(prefab);
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        if (!objectPools.ContainsKey(obj.name))
        {
            objectPools[obj.name] = new Queue<GameObject>();
        }
        objectPools[obj.name].Enqueue(obj);
    }

    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        obj.name = prefab.name;
        return obj;
    }

    public GameObject GetObjectEnemy(GameObject prefab)
    {
        if (objectPools.TryGetValue(prefab.name, out Queue<GameObject> objectPool))
        {
            while (objectPool.Count > 0)
            {
                GameObject obj = objectPool.Dequeue();
                if (obj != null) // Check if the object is not null
                {
                    obj.SetActive(true);
                    ResetEnemy(obj);
                    return obj;
                }
            }
        }
        return CreateNewObject(prefab);
    }

    private void ResetEnemy(GameObject obj)
    {
        EnemyStats enemyStats = obj.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            enemyStats.ResetEnemy();
        }
    }
}
