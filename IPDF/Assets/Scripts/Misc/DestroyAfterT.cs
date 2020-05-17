using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterT : MonoBehaviour {
    public float T;

    void Awake () {
        Destroy (gameObject, T);
    }
}
