using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {
    [Header ("Range Info")]
    public float triggerRange;
    public float forwardDistance;
    [Header ("Link")]
    public Teleporter other;

    public StructuresManager structuresManager;
    public CameraFollowPlayer cameraFollowPlayer;
    public NavigationManager navigationManager;

    void Awake () {
        structuresManager = FindObjectOfType<StructuresManager> ();
        cameraFollowPlayer = FindObjectOfType<CameraFollowPlayer> ();
        navigationManager = FindObjectOfType<NavigationManager> ();
        if (other.navigationManager == null) navigationManager.sectorLinks.Add (new SectorLink (transform.parent.GetComponent<Sector> (),
            transform.position,
            other.transform.parent.GetComponent<Sector> (),
            other.transform.position
        ));
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
