using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooler : MonoBehaviour {
    public static Pooler current;
    [Header ("Pools")]
    public Pool beamPool = new Pool ();
    public Pool pulsePool = new Pool ();
    public Pool kineticPool = new Pool ();
    public Pool explosionPool = new Pool ();

    void Awake () {
        current = this;
    }

    public static Pooler GetInstance () {
        return current;
    }

    public GameObject Retrieve (Pool pool) {
        foreach (GameObject pooled in pool.pool)
            if (!pooled.activeSelf)
                return pooled;
        GameObject instantiated = Instantiate (pool.prefab) as GameObject;
        pool.pool.Add (instantiated);
        return instantiated;
    }
}

[System.Serializable]
public class Pool {
    public GameObject prefab;
    public List<GameObject> pool = new List<GameObject> ();
}