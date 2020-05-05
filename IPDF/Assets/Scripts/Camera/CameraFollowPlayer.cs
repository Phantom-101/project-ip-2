using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {
    [Header ("Player & UI")]
    public UIHandler uIHandler;
    public StructureBehaviours playerStructure;
    [Header ("Physics")]
    public new Rigidbody rigidbody;
    public new ConstantForce constantForce;

    void Awake () {
        uIHandler = FindObjectOfType<UIHandler> ();
        playerStructure = uIHandler.source;
        transform.position = playerStructure.transform.rotation * new Vector3 (0.0f, 5.0f, -10.0f) + playerStructure.transform.position;
        rigidbody = GetComponent<Rigidbody> ();
        if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody> ();
        rigidbody.drag = 2.5f;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
        constantForce = GetComponent<ConstantForce> ();
        if (constantForce == null) constantForce = gameObject.AddComponent<ConstantForce> ();
    }

    void Update () {
        playerStructure = uIHandler.source;
        if (playerStructure != null) {
            Vector3 playerPosition = playerStructure.transform.position;
            transform.LookAt (playerPosition + playerStructure.transform.rotation * new Vector3 (0.0f, 0.0f, 3.0f));
            float distance = Vector3.Distance (transform.position, playerPosition);
            if (distance > 7.0f) {
                constantForce.relativeForce = new Vector3 (0.0f, 0.0f, distance * 1.0f);
            } else if (distance < 3.0f) {
                constantForce.relativeForce = new Vector3 (0.0f, 0.0f, -distance * 1.0f);
            } else {
                constantForce.relativeForce = new Vector3 (0.0f, 0.0f, 0.0f);
            }
        }
    }
}