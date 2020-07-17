using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Audio Asset", menuName = "Audio Asset")]
public class AudioAsset : ScriptableObject {
    public AudioClip clip;
    public float volume;
    public float spatialBlend;
    public float minDistance;
    public float maxDistance;
    public AudioRolloffMode rolloffMode;
}
