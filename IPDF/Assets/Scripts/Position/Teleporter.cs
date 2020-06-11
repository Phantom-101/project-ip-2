using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {
    [Header ("Range Info")]
    public float triggerRange;
    public float forwardDistance;
    [Header ("Link")]
    public Transform other;

    public StructuresManager structuresManager;
    public CameraFollowPlayer cameraFollowPlayer;
    public NavigationManager navigationManager;

    void Awake () {
        structuresManager = FindObjectOfType<StructuresManager> ();
        cameraFollowPlayer = FindObjectOfType<CameraFollowPlayer> ();
        navigationManager = FindObjectOfType<NavigationManager> ();
        navigationManager.AddAdjacency (transform.parent.GetComponent<Sector> (),
            other.parent.GetComponent <Sector> (),
            transform.position
        );
    }

    void Update () {
        foreach (StructureBehaviours structure in structuresManager.structures)
            if (structure != null &&
                (transform.position - structure.transform.position).sqrMagnitude <= triggerRange * triggerRange &&
                structure.profile.structureClass != StructureClass.Station) {
                structure.transform.position = other.transform.position + other.transform.forward * forwardDistance * 2.0f;
                structure.transform.rotation = other.transform.rotation;
                structure.targeted = null;
                structure.transform.parent = other.transform.parent;
                if (cameraFollowPlayer.playerStructure == structure) cameraFollowPlayer.ResetPosition ();
            }
    }
}
