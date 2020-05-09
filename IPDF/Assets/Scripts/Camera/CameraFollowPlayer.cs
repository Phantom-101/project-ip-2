using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {
    [Header ("Player")]
    public PlayerController playerController;
    public StructureBehaviours playerStructure;
    [Header ("Configurations")]
    public Vector3 startPositionOffset = new Vector3 (0.0f, 5.0f, -10.0f);
    public Vector3 positionOffset = new Vector3 (0.0f, 3.0f, -5.0f);
    [Range (2.5f, 10.0f)]
    public float positionInterpolationStrength = 5.0f;
    [Range (1.0f, 5.0f)]
    public float lookAtOffset = 3.0f;
    [Range (0.5f, 2.0f)]
    public float lookAtInterpolationStrength = 1.0f;
    [Header ("Physics")]
    public new Rigidbody rigidbody;
    public new ConstantForce constantForce;

    void Awake () {
        playerController = FindObjectOfType<PlayerController> ();
        playerStructure = playerController.structureBehaviours;
        ResetPosition ();
        rigidbody = GetComponent<Rigidbody> ();
        if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody> ();
        rigidbody.drag = 2.5f;
        rigidbody.angularDrag = 5.0f;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
        constantForce = GetComponent<ConstantForce> ();
        if (constantForce == null) constantForce = gameObject.AddComponent<ConstantForce> ();
    }

    public void ResetPosition () {
        playerStructure = playerController.structureBehaviours;
        transform.position = playerStructure.transform.rotation * startPositionOffset + playerStructure.transform.position;
    }

    void Update () {
        playerStructure = playerController.structureBehaviours;
        if (playerStructure != null) {
            Vector3 playerPosition = playerStructure.transform.position;
            transform.LookAt (playerPosition + new Vector3 (0.0f, lookAtOffset, 0.0f));
            //Vector3 targetDir = playerPosition - transform.position;
            //Vector3 forward = transform.forward;
            //Vector3 localTarget = transform.InverseTransformPoint (playerPosition);
            //float angle = Mathf.Atan2 (localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            //Vector3 eulerAngleVelocity = new Vector3 (0.0f, angle, 0.0f);
            //constantForce.torque = eulerAngleVelocity * lookAtInterpolationStrength;
            Vector3 targetPosition = playerPosition + playerStructure.transform.rotation * positionOffset;
            constantForce.force = (targetPosition - transform.position) * positionInterpolationStrength;
        }
    }
}