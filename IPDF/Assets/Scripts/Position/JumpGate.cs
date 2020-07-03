using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGate : MonoBehaviour {
    [Header ("Range Info")]
    public float triggerRange;
    public float forwardDistance;
    [Header ("Link")]
    public JumpGate other;

    public StructuresManager structuresManager;
    public CameraFollowPlayer cameraFollowPlayer;
    public NavigationManager navigationManager;

    void Awake () {
        structuresManager = FindObjectOfType<StructuresManager> ();
        cameraFollowPlayer = FindObjectOfType<CameraFollowPlayer> ();
        navigationManager = FindObjectOfType<NavigationManager> ();
        if (other == null) return;
        navigationManager.AddAdjacency (transform.parent.GetComponent<Sector> (),
            other.transform.parent.GetComponent <Sector> (),
            transform.position
        );
    }

    void Update () {
        if (other == null) return;
        foreach (StructureBehaviours structure in structuresManager.structures)
            if (structure.transform != transform.parent && structure != null &&
                (transform.position - structure.transform.position).sqrMagnitude <= triggerRange * triggerRange &&
                structure.profile.structureClass != StructureClass.Station) {
                structure.transform.position = other.transform.position + other.transform.forward * forwardDistance;
                structure.transform.rotation = other.transform.rotation;
                structure.targeted = null;
                structure.transform.parent.GetComponent<Sector> ().inSector.Remove (structure);
                foreach (Transform child in structure.transform)
                    if (child.GetComponent<StructureBehaviours> ())
                        structure.transform.parent.GetComponent<Sector> ().inSector.Remove (child.GetComponent<StructureBehaviours> ());
                structure.transform.parent = other.transform.parent;
                if (cameraFollowPlayer.playerStructure == structure) cameraFollowPlayer.ResetPosition ();
            }
    }
}
