using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {
    [Header ("Player")]
    public PlayerController playerController;
    public StructureBehaviours playerStructure;
    [Header ("Configurations")]
    public Vector3 startPositionOffset = new Vector3 (0, 1, -2);
    public Vector3 positionOffset = new Vector3 (0, 1, -2);
    [Range (2.5f, 10)]
    public float positionInterpolationStrength = 5;
    [Range (0, 1)]
    public float lookAtOffset = 0.5f;
    [Range (0.5f, 2)]
    public float lookAtInterpolationStrength = 1;
    public bool lookAtTarget = false;
    [Header ("Physics")]
    public new Rigidbody rigidbody;
    public new ConstantForce constantForce;

    void Awake () {
        playerController = FindObjectOfType<PlayerController> ();
        playerStructure = playerController.structureBehaviours;
        ResetPosition ();
        rigidbody = GetComponent<Rigidbody> ();
        if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody> ();
        rigidbody.drag = 7.5f;
        rigidbody.angularDrag = 7.5f;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
        constantForce = GetComponent<ConstantForce> ();
        if (constantForce == null) constantForce = gameObject.AddComponent<ConstantForce> ();
    }

    public void ResetPosition () {
        playerStructure = playerController.structureBehaviours;
        transform.position = playerStructure.transform.rotation * (startPositionOffset * playerStructure.profile.apparentSize) + playerStructure.transform.position;
    }

    void Update () {
        playerStructure = playerController.structureBehaviours;
        if (playerStructure != null) {
            Vector3 playerPosition = playerStructure.transform.position;
            transform.LookAt (playerPosition + new Vector3 (0.0f, lookAtOffset * playerStructure.profile.apparentSize, 0.0f));
            /*
            Vector3 targetDir = playerPosition - transform.position;
            Vector3 forward = transform.forward;
            Vector3 localTarget = transform.InverseTransformPoint (playerPosition);
            float angle = Mathf.Atan2 (localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            Vector3 eulerAngleVelocity = new Vector3 (0.0f, angle, 0.0f);
            constantForce.torque = eulerAngleVelocity * lookAtInterpolationStrength;
            */
            float angle = 0.0f;
            if (lookAtTarget && playerStructure.targeted != null) {
                Vector3 heading = playerStructure.targeted.transform.position - playerStructure.transform.position;
                Vector3 perp = Vector3.Cross (playerStructure.transform.forward, heading);
                float leftRight = Vector3.Dot (perp, playerStructure.transform.up);
                angle = (playerStructure.targeted.transform.position + playerStructure.targeted.transform.rotation * playerStructure.targeted.profile.offset)
                    - (playerStructure.transform.position + playerStructure.transform.rotation * playerStructure.profile.offset) == Vector3.zero ?
                        0.0f :
                        Quaternion.Angle (playerStructure.transform.rotation,
                            Quaternion.LookRotation ((playerStructure.targeted.transform.position + playerStructure.targeted.transform.rotation * playerStructure.targeted.profile.offset)
                    - (playerStructure.transform.position + playerStructure.transform.rotation * playerStructure.profile.offset)));
                angle *= leftRight > 0.0f ? 1.0f : -1.0f;
            }
            Vector3 targetPositionOffset = new Vector3 (positionOffset.z * Mathf.Sin (angle * Mathf.Deg2Rad), positionOffset.y, positionOffset.z * Mathf.Cos (angle * Mathf.Deg2Rad));
            Vector3 targetPosition = playerPosition + playerStructure.transform.rotation * (targetPositionOffset * playerStructure.profile.apparentSize);
            constantForce.force = (targetPosition - transform.position) * positionInterpolationStrength * 5.0f;
        }
    }

    public void ToggleLookAtTarget () {
        lookAtTarget = !lookAtTarget;
    }
}