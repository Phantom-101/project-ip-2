using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Vector3Double {
    public double x;
    public double y;
    public double z;
}

public class Position : MonoBehaviour {
    public void ShiftOrigin(Vector3 pos) {
        transform.Translate(-pos, Space.World);
    }

    public void Set(Vector3 pos) {
        transform.position = pos;
    }
}
