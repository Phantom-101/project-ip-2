using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealthChangeProfile", menuName = "ScriptableObjects/HealthChangeProfile")]
public class HealthChangeProfile : ScriptableObject {
    public float value;
    public float[] effectiveness;
    public bool[] bypasses;
}