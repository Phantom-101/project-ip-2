using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealthChangeProfile", menuName = "ScriptableObjects/HealthChangeProfile")]
public class HealthChangeProfile : ScriptableObject {
    public float value;
    public float[] effectiveness;
    public bool[] bypasses;
}

public struct HealthChange {
    public float value;
    public float[] effectiveness;
    public bool[] bypasses;

    public HealthChange(float value, float[] effectiveness, bool[] bypasses) {
        this.value = value;
        this.effectiveness = effectiveness;
        this.bypasses = bypasses;
    }

    public HealthChange(HealthChangeProfile healthChangeProfile) {
        this.value = healthChangeProfile.value;
        this.effectiveness = healthChangeProfile.effectiveness;
        this.bypasses = healthChangeProfile.bypasses;
    }
}
