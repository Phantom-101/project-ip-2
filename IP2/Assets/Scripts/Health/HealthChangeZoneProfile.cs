using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealthChangeZoneProfile", menuName = "ScriptableObjects/HealthChangeZoneProfile")]
public class HealthChangeZoneProfile : ScriptableObject {
    [Header("Size")]
    public float radius;
    [Header("Application")]
    public HealthChangeProfile damageProfile;
}
