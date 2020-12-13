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

    bool initialized = false;

    void Update () {
        if (other == null) return;
        if (!initialized) {
            initialized = true;
            audioSource = gameObject.AddComponent<AudioSource> ();
            structuresManager = StructuresManager.GetInstance ();
            cameraFollowPlayer = CameraFollowPlayer.GetInstance ();
            navigationManager = NavigationManager.GetInstance ();
            resourcesManager = ResourcesManager.GetInstance ();
            navigationManager.AddAdjacency (transform.parent.GetComponent<Sector> (),
                other.transform.parent.GetComponent<Sector> (),
                transform.position
            );
        }
        foreach (StructureBehaviours structure in structuresManager.structures)
            if (structure != null && structure.transform != transform.parent && (transform.position - structure.transform.position).sqrMagnitude <= triggerRange * triggerRange &&
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
        AudioAsset audio = resourcesManager.audioResources.jumps[0];
        audioSource.spatialBlend = audio.spatialBlend;
        audioSource.rolloffMode = audio.rolloffMode;
        audioSource.minDistance = audio.minDistance;
        audioSource.maxDistance = audio.maxDistance;
        audioSource.PlayOneShot (audio.clip, audio.volume);
    }
}
