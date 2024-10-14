using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawner")]
    [SerializeField] protected Transform holder;

    [SerializeField] protected int spawnedCount = 0;
    public int SpawnedCount => spawnedCount;

    [SerializeField] protected List<Transform> prefabs;
    [SerializeField] protected List<Transform> poolObjs;
    [SerializeField] protected int poolSize = 5;  // Kích thước pool mặc định

    private void Start() {
        this.LoadPrefabs();
        this.LoadHolder();
        this.PrepopulatePool();  // Tiền tạo pool với các bản sao không hoạt động
    }

    protected virtual void LoadHolder()
    {
        if (this.holder != null) return;
        this.holder = transform.Find("Holder");
        Debug.LogWarning(transform.name + ": LoadHolder", gameObject);
    }

    protected virtual void LoadPrefabs()
    {
        if (this.prefabs.Count > 0) return;

        Transform prefabObj = transform.Find("Prefabs");
        foreach (Transform prefab in prefabObj)
        {
            this.prefabs.Add(prefab);
        }

        Debug.LogWarning(transform.name + ": LoadPrefabs", gameObject);
    }

    protected virtual void PrepopulatePool()
    {
        // Tiền tạo pool với các bản sao không hoạt động của mỗi prefab
        foreach (Transform prefab in prefabs)
        {
            for (int i = 0; i < poolSize; i++)
            {
                Transform newPrefab = Instantiate(prefab);
                newPrefab.name = prefab.name;
                newPrefab.SetParent(this.holder);
                newPrefab.gameObject.SetActive(false); // Không hoạt động nhưng sẵn sàng được sử dụng lại
                this.poolObjs.Add(newPrefab);
            }
        }
    }

    public virtual Transform Spawn(string prefabName, Vector3 spawnPos, Quaternion rotation)
    {
        Transform prefab = this.GetPrefabByName(prefabName);
        if (prefab == null)
        {
            Debug.LogError("Prefab không tìm thấy: " + prefabName);
            return null;
        }

        return this.Spawn(prefab, spawnPos, rotation);
    }

    public virtual Transform Spawn(Transform prefab, Vector3 spawnPos, Quaternion rotation)
    {
        Transform newPrefab = this.GetObjectFromPool(prefab);
        if (newPrefab == null)
        {
            // Tạo mới nếu pool trống
            newPrefab = Instantiate(prefab);
            newPrefab.name = prefab.name;
        }

        newPrefab.SetPositionAndRotation(spawnPos, rotation);
        newPrefab.SetParent(this.holder);
        newPrefab.gameObject.SetActive(true); // Kích hoạt khi được sinh ra
        this.spawnedCount++;

        return newPrefab;
    }

    protected virtual Transform GetObjectFromPool(Transform prefab)
    {
        foreach (Transform poolObj in this.poolObjs)
        {
            if (poolObj == null) continue;

            if (poolObj.name == prefab.name && !poolObj.gameObject.activeInHierarchy)
            {
                this.poolObjs.Remove(poolObj);
                return poolObj;
            }
        }

        return null; // Trả về null nếu không có đối tượng nào trong pool
    }

    public virtual void Despawn(Transform obj)
    {
        if (!this.poolObjs.Contains(obj))
        {
            this.poolObjs.Add(obj);
        }
        obj.gameObject.SetActive(false); // Vô hiệu hóa đối tượng
        // this.spawnedCount--;
    }
    public virtual Transform GetPrefabByName(string prefabName)
    {
        foreach (Transform prefab in this.prefabs)
        {
            if (prefab.name == prefabName) return prefab;
        }

        return null;
    }

    public virtual Transform RandomPrefab()
    {
        int rand = Random.Range(0, this.prefabs.Count);
        return this.prefabs[rand];
    }

    public virtual void Hold(Transform obj)
    {
        obj.parent = this.holder;
    }
}
