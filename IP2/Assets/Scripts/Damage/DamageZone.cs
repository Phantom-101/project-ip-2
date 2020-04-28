using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour {
    public DamageZoneProfile damageZoneProfile;

    StructuresManager structuresManager;

    void Awake() {
        structuresManager = FindObjectOfType<StructuresManager>();
    }

    public void Initialize() {
        if(damageZoneProfile.duration != 0.0f) Destroy(gameObject, damageZoneProfile.duration);
    }

    void Update() {
        if(damageZoneProfile != null) {
            foreach(StructureStatsManager structure in structuresManager.GetStructures()) {
                if((transform.position - structure.gameObject.transform.position).sqrMagnitude <= damageZoneProfile.radius * damageZoneProfile.radius) {
                    DamageProfile damageProfile = damageZoneProfile.damageProfile;
                    structure.AddDamage(new DamageProfileStruct(damageProfile, Time.deltaTime, true));
                }
            }
            if(damageZoneProfile.duration == 0.0f) Destroy(gameObject);
        }
    }
}
