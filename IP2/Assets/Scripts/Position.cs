using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Vector3Double {
    public double x;
    public double y;
    public double z;
}

public class Position : MonoBehaviour {
    public Vector3Double position;

    void Awake() {
        position.x = transform.position.x;
        position.y = transform.position.y;
        position.z = transform.position.z;
    }

    void Update() {
        transform.position = new Vector3((float)position.x, (float)position.y, (float)position.z);
    }

    public void Translate(Vector3 pos) {
        pos = transform.rotation * pos;
        position.x += pos.x;
        position.y += pos.y;
        position.z += pos.z;
    }

    public void Set(Vector3 pos) {
        position.x = pos.x;
        position.y = pos.y;
        position.z = pos.z;
    }
}
