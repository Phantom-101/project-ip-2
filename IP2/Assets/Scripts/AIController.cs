using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    StructuresManager structuresManager;
    StructureStatsManager structureStatsManager;
    StructureModulesManager structureModulesManager;
    StructureMovementManager structureMovementManager;
    GameObject target;
    float maneuverTimer;

    void Awake()
    {
        structuresManager = GameObject.FindObjectOfType<StructuresManager>();
        structureStatsManager = GetComponent<StructureStatsManager>();
        structureModulesManager = GetComponent<StructureModulesManager>();
        structureMovementManager = GetComponent<StructureMovementManager>();
        maneuverTimer = 0.0f;
    }

    void Update()
    {
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
            structureModulesManager.TryActivateAllWeapons(target);
            Vector3 targetPos = target.transform.position;
            if(Vector3.Distance(transform.position, target.transform.position) > 1000.0f) {
                if(maneuverTimer > 5.0f) {
                    targetPos = target.transform.position + target.transform.TransformDirection(new Vector3(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f)) * 750.0f);
                    maneuverTimer = 0.0f;
                }
                else maneuverTimer += Time.deltaTime;
            }
            Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
            structureMovementManager.SetTarget(target);
            structureMovementManager.ClearOrders();
            structureMovementManager.AddOrder("Align");
            structureMovementManager.SetAxisTranslation(Axis.Z, 1.0f);
        } else {
            structureMovementManager.SetAxisTranslation(Axis.Z, 0.0f);
        }
    }

    public void RespondToHelpRequest(GameObject enemy) {
        if(target == null) target = enemy;
    }
}
