using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {
    [Header ("Range Info")]
    public float triggerRange;
    public float forwardDistance;
    [Header ("Link")]
    public Transform other;

    StructuresManager structuresManager;
    CameraFollowPlayer cam;

    void Awake () {
        structuresManager = FindObjectOfType<StructuresManager> ();
        cam = FindObjectOfType<CameraFollowPlayer> ();
    }

    void Update () {
        foreach (StructureBehaviours structure in structuresManager.structures)
            if (structure != null &&
                (transform.position - structure.transform.position).sqrMagnitude <= triggerRange * triggerRange &&
                structure.profile.structureClass != StructureClass.Station) {
                structure.transform.position = other.position + other.forward * forwardDistance * 2.0f;
                structure.transform.rotation = other.rotation;
                structure.targeted = null;
                structure.transform.parent = other.parent;
                if (cam.playerStructure == structure) cam.ResetPosition ();
            }
    }
}
