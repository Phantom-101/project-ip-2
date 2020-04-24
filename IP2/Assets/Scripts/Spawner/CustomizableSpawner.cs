using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizableSpawner : MonoBehaviour {
    public GameObject[] prefabs;
    public List<GameObject> instantiated = new List<GameObject>();
    public float max;
    public float spawnInterval;
    public float spawnRadius;

    float timeElapsed;

    void Update() {
        timeElapsed += Time.deltaTime;
        instantiated.RemoveAll(item => item == null);
        if((max == -1 || instantiated.Count < max) && timeElapsed >= spawnInterval) {
            GameObject go = Instantiate(prefabs[Random.Range(0, prefabs.Length - 1)], transform.position + new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    Random.Range(-spawnRadius, spawnRadius),
                    Random.Range(-spawnRadius, spawnRadius)),
                Quaternion.identity) as GameObject;
            instantiated.Add(go);
            timeElapsed = 0.0f;
        }
    }
}
