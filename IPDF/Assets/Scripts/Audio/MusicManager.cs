using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    public static MusicManager current;

    [Header ("Triggers")]
    public MusicTrigger[] triggers;
    public List<MusicTrigger> queue;
    public MusicTrigger currentTrigger;
    public bool transition;
    [Header ("Components")]
    public StructuresManager structuresManager;
    public PlayerController playerController;
    public AudioSource audioSource;

    void Awake () {
        current = this;
    }

    public static MusicManager GetInstance () {
        return current;
    }

    void Start () {
        structuresManager = StructuresManager.GetInstance ();
        playerController = PlayerController.GetInstance ();
        audioSource = Camera.main.gameObject.GetComponent<AudioSource> ();
    }

    public void Tick (float deltaTime) {
        if (structuresManager == null || playerController == null || audioSource == null) return;
        foreach (MusicTrigger trigger in triggers) {
            if (trigger.CanBeUsed (structuresManager, playerController.structureBehaviours)) {
                if (!queue.Contains (trigger)) {
                    queue.Add (trigger);
                    queue.Sort ();
                    if (queue[0] != currentTrigger) {
                        currentTrigger = queue[0];
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
                AudioAsset asset = currentTrigger.GetRandomMusic ();
                audioSource.volume = asset.volume;
                audioSource.clip = asset.clip;
                audioSource.Play ();
                transition = false;
            }
        }
    }
}
