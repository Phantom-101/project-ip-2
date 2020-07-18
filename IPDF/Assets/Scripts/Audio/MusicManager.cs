using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    [Header ("Triggers")]
    public MusicTrigger[] triggers;
    public List<MusicTrigger> queue;
    public MusicTrigger current;
    public bool transition;
    [Header ("Components")]
    public StructuresManager structuresManager;
    public PlayerController playerController;
    public AudioSource audioSource;

    void Awake () {
        structuresManager = FindObjectOfType<StructuresManager> ();
        playerController = FindObjectOfType<PlayerController> ();
        audioSource = FindObjectOfType<Camera> ().gameObject.GetComponent<AudioSource> ();
    }

    public void Tick (float deltaTime) {
        foreach (MusicTrigger trigger in triggers) {
            if (trigger.CanBeUsed (structuresManager, playerController.structureBehaviours)) {
                if (!queue.Contains (trigger)) {
                    queue.Add (trigger);
                    queue.Sort ();
                    if (queue[0] != current) {
                        current = queue[0];
                        transition = true;
                    }
                }
            } else {
                if (queue.Contains (trigger)) {
                    queue.Remove (trigger);
                }
            }
        }
        if (transition) {
            if (audioSource.volume > 0) audioSource.volume -= deltaTime;
            else {
                audioSource.Stop ();
                AudioAsset asset = current.GetRandomMusic ();
                audioSource.volume = asset.volume;
                audioSource.clip = asset.clip;
                audioSource.Play ();
                transition = false;
            }
        }
    }
}
