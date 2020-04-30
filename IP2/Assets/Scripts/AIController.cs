﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    StructuresManager structuresManager;
    StructureStatsManager structureStatsManager;
    StructureEquipmentManager structureEquipmentManager;
    StructureMovementManager structureMovementManager;
    GameObject target;
    float maneuverTimer;


    void Awake() {
        structuresManager = FindObjectOfType<StructuresManager>();
        structureStatsManager = GetComponent<StructureStatsManager>();
        structureEquipmentManager = GetComponent<StructureEquipmentManager>();
        structureMovementManager = GetComponent<StructureMovementManager>();
        maneuverTimer = 0.0f;
    }

    void Update() {
        List<StructureStatsManager> validTargets = structuresManager.GetStructures();
        float closest = float.PositiveInfinity;
        foreach(StructureStatsManager structure in validTargets) {
            if(structure.faction != structureStatsManager.faction) {
                GameObject sGO = structure.gameObject;
                float distance = Vector3.Distance(transform.position, sGO.transform.position);
                if(distance < closest) {
                    closest = distance;
                    target = sGO;
                }
            }
        }
        if(target != null) {
            structureEquipmentManager.TryActivateAllEquipment(target);
            Vector3 targetPos = target.transform.position;
            if(Vector3.Distance(transform.position, target.transform.position) < 25.0f) {
                if(maneuverTimer > 30.0f) {
                    targetPos += new Vector3(Random.Range(25.0f, 50.0f), Random.Range(25.0f, 50.0f), Random.Range(25.0f, 50.0f));
                    maneuverTimer = 0.0f;
                }
                else maneuverTimer += Time.deltaTime;
            }
            Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
            structureMovementManager.SetTarget(targetPos);
            structureMovementManager.ClearOrders();
            structureMovementManager.AddOrder("Align");
            structureMovementManager.SetAxisTranslation(Axis.Z, 1.0f);
        } else {
            structureMovementManager.SetAxisTranslation(Axis.Z, 0.0f);
        }
    }
}
