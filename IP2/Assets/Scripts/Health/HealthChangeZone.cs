using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthChangeZone : MonoBehaviour {
    public HealthChangeZoneProfile healthChangeZoneProfile;

    StructuresManager structuresManager;

    void Awake() {
        structuresManager = FindObjectOfType<StructuresManager>();
    }

    public void Initialize() {}

    void Update() {
        if(healthChangeZoneProfile != null) {
            foreach(StructureStatsManager structure in structuresManager.GetStructures()) {
                if((transform.position - structure.gameObject.transform.position).sqrMagnitude <= healthChangeZoneProfile.radius * healthChangeZoneProfile.radius) {
                    HealthChangeProfile healthChangeProfile = healthChangeZoneProfile.damageProfile;
                    structure.AddHealthChange(new HealthChange(healthChangeProfile));
                }
            }
        }
        Destroy(gameObject);
    }
}
