using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGate : MonoBehaviour {
    [Header ("Range Info")]
    public float triggerRange;
    public float forwardDistance;
    [Header ("Link")]
    public string otherId;
    public JumpGate other;
    [Header ("Components")]
    public AudioSource audioSource;
    public StructuresManager structuresManager;
    public CameraFollowPlayer cameraFollowPlayer;
    public NavigationManager navigationManager;
    public ResourcesManager resourcesManager;

    void Awake () {
        audioSource = gameObject.AddComponent<AudioSource> ();
        structuresManager = FindObjectOfType<StructuresManager> ();
        cameraFollowPlayer = FindObjectOfType<CameraFollowPlayer> ();
        navigationManager = FindObjectOfType<NavigationManager> ();
        resourcesManager = FindObjectOfType<ResourcesManager> ();
        audioSource.spatialBlend = 1;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
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
                structure.sector.inSector.Remove (structure);
                structure.transform.parent = other.transform.parent;
                Sector otherSector = other.transform.parent.GetComponent<Sector> ();
                structure.sector = otherSector;
                otherSector.inSector.Add (structure);
                if (cameraFollowPlayer.playerStructure == structure) cameraFollowPlayer.ResetPosition ();
                Jumped ();
                other.Jumped ();
            }
    }

    public void Jumped () {
        audioSource.PlayOneShot (resourcesManager.audioResources.jump[0], 1);
    }
}
