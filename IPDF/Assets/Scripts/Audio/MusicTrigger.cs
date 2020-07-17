using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Music Trigger", menuName = "Music Triggers/Default")]
public class MusicTrigger : ScriptableObject, IComparable<MusicTrigger> {
    public AudioAsset[] music;
    public int priority;

    public bool CanBeUsed (StructuresManager structures, StructureBehaviours player) {
        return true;
    }

    public int CompareTo (MusicTrigger other) {
        if (priority > other.priority) return -1;
        if (priority < other.priority) return 1;
        return 0;
    }

    public AudioAsset GetRandomMusic () {
        return music[UnityEngine.Random.Range (0, music.Length)];
    }
}
