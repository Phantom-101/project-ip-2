using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisMovement : MonoBehaviour {
    public float force;

    void Start () {
        Rigidbody rigidbody = GetComponent<Rigidbody> ();
        if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody> ();
        rigidbody.drag = 0.25f;
        rigidbody.angularDrag = 0.5f;
        rigidbody.AddForce (transform.localPosition * force * transform.parent.localScale.x, ForceMode.Impulse);
        rigidbody.AddTorque (new Vector3 (Random.Range (-25, 25), Random.Range (-25, 25), Random.Range (-25, 25)), ForceMode.Impulse);
    }
}
