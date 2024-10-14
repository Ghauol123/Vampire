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
            if (objectPool.Count > 0)
            {
                GameObject obj = objectPool.Dequeue();
                obj.SetActive(true);
                return obj;
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
}