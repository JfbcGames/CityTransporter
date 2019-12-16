using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool {

    //private int poolSize;
    private List<GameObject> pool;
    private GameObject objectToPool;
    private bool shouldExpand = false;

    public Pool(GameObject objectToPool) {
        pool = new List<GameObject>();
        this.objectToPool = objectToPool;
        ///this.objectToPool.hideFlags = HideFlags.HideInHierarchy;
        AddToPool();
    }
    public Pool(GameObject objectToPool, int n) {
        pool = new List<GameObject>();
        this.objectToPool = objectToPool;
        //this.objectToPool.hideFlags = HideFlags.HideInHierarchy;
        IncrementPoolSize(n);
    }

    public GameObject GetElement(int i) {
        return pool[i];
    }

    public GameObject GetPooledObject() {
        for (int i = 0; i < pool.Count; i++) {
            if (!pool[i].activeInHierarchy) {
                return pool[i];
            }
        }
        if (shouldExpand) {
            return AddToPool();
        } else {
            return null;
        }
    }

    public List<GameObject> GetActiveObjects() {
        List<GameObject> activeObjects = new List<GameObject>();
        for (int i = 0; i < pool.Count; i++) {
            if (pool[i].activeSelf) {
                activeObjects.Add(pool[i]);
            }
        }
        return activeObjects;
    }

    public GameObject AddToPool() {
        GameObject go = GameManager.instance.InstantiateObject(objectToPool);
        return AddToPool(go);
    }

    public GameObject AddToPool(GameObject go) {
        go.SetActive(false);
        //go.hideFlags = HideFlags.HideInHierarchy;
        pool.Add(go);
        return go;
    }

    public void IncrementPoolSize(int n) {
        for (int i = 0; i < n; i++) {
            AddToPool();
        }
    }

    public void SetShouldExpand(bool b) {
        this.shouldExpand = b;
    }
    
    public bool GetShouldExpand() {
        return shouldExpand;
    }

    public int GetPoolSize() {
        return pool.Count;
    }

    public bool AvaiablePooledObjects() {
        if (shouldExpand)
            return true;
        else {
            for (int i = 0; i < pool.Count; i++) {
                if (pool[i].activeInHierarchy) {
                    return true;
                }
            }
            return false;
        }
    }

}
