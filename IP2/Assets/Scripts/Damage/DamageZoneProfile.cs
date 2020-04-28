using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DamageZoneProfile", menuName = "ScriptableObjects/DamageZoneProfile")]
public class DamageZoneProfile : ScriptableObject {
    public float radius;
    public DamageProfile damageProfile;
    public bool applyPerFrame;
    public float duration;
}
