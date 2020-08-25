using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
    public static MusicManager current;

    [Header ("Triggers")]
    public MusicTrigger[] triggers;
    public List<MusicTrigger> queue;
    public MusicTrigger currentTrigger;
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
        audioSource.loop = false;
    }

    public void Tick (float deltaTime) {
        if (triggers.Length == 0) return;
        if (structuresManager == null || playerController == null || audioSource == null) {
            structuresManager = StructuresManager.GetInstance ();
            playerController = PlayerController.GetInstance ();
            audioSource = Camera.main.gameObject.GetComponent<AudioSource> ();
            return;
        }
        foreach (MusicTrigger trigger in triggers) {
            if (trigger.CanBeUsed (structuresManager, playerController.structureBehaviours)) {
                if (!queue.Contains (trigger)) {
                    queue.Add (trigger);
                }
            } else {
                if (queue.Contains (trigger)) {
                    queue.Remove (trigger);
                }
            }
        }
        queue.Sort ();
        currentTrigger = queue[0];
        if (!audioSource.isPlaying) {
            AudioAsset asset = currentTrigger.GetRandomMusic ();
            audioSource.volume = asset.volume;
            audioSource.clip = asset.clip;
            audioSource.Play ();
        }
    }
}
