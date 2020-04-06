using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    StructuresManager sm;
    StructureStatsManager ssm;
    StructureModulesManager smm;
    GameObject target;
    ConstantForce cf;
    float maneuverTimer;

    void Awake()
    {
        sm = GameObject.FindObjectOfType<StructuresManager>();
        ssm = GetComponent<StructureStatsManager>();
        smm = GetComponent<StructureModulesManager>();
        cf = GetComponent<ConstantForce>();
        maneuverTimer = 0.0f;
    }

    void Update()
    {
        List<StructureStatsManager> validTargets = sm.GetStructures();
        float closest = float.PositiveInfinity;
        foreach(StructureStatsManager structure in validTargets) {
            if(structure.faction != ssm.faction) {
                GameObject sGO = structure.gameObject;
                float distance = Vector3.Distance(transform.position, sGO.transform.position);
                if(distance < closest) {
                    closest = distance;
                    target = sGO;
                }
            }
        }
        if(target != null) {
            smm.TryActivateAllWeapons(target);
            Vector3 targetPos = target.transform.position;
            if(Vector3.Distance(transform.position, target.transform.position) > 1000.0f) {
                if(maneuverTimer > 5.0f) {
                    targetPos = target.transform.position + target.transform.TransformDirection(new Vector3(Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f)) * 750.0f);
                    maneuverTimer = 0.0f;
                }
                else maneuverTimer += Time.deltaTime;
            }
            Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * GetComponent<StructureStatsManager>().GetStat("structure current turnSpeed"));
            transform.Translate(new Vector3(0.0f, 0.0f, ssm.GetStat("structure current speed") * Time.deltaTime));
        }
    }

    public void RespondToHelpRequest(GameObject enemy) {
        if(target == null) target = enemy;
    }
}
