using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Wave {
    public GameObject[] wave;
}

public class WaveSpawner : MonoBehaviour {
    public Wave[] waves;
    public GameObject spawnEffect;
    public List<GameObject> instantiated = new List<GameObject>();
    public float spawnRadius;

    int wavesSpawned;
    float timeElapsed;

    void Update() {
        foreach (GameObject go in instantiated.ToArray ()) if (go == null) instantiated.Remove (go);
        if (instantiated.Count == 0) {
            if (wavesSpawned >= waves.Length) Destroy (gameObject);
            GameObject[] wave = waves[wavesSpawned].wave;
            foreach (GameObject waveObject in wave) {
                Vector3 randomPos = new Vector3 (
                    UnityEngine.Random.Range(-spawnRadius, spawnRadius),
                    0.0f,
                    UnityEngine.Random.Range(-spawnRadius, spawnRadius)
                );
                GameObject go = Instantiate (waveObject, transform.position + randomPos, Quaternion.identity) as GameObject;
                instantiated.Add (go);
                GameObject spawnedEffect = Instantiate (spawnEffect, transform.position + randomPos, Quaternion.identity) as GameObject;
                spawnedEffect.transform.localScale = Vector3.one * (go.GetComponent<StructureBehaviours> () ? go.GetComponent<StructureBehaviours> ().profile.apparentSize : 1.0f) * 10.0f;
            }
            wavesSpawned ++;
        }
    }
}
