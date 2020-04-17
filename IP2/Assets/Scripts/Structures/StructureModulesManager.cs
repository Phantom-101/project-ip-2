using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureModulesManager : MonoBehaviour
{
    public List<Turret> turrets = new List<Turret>();
    public List<GameObject> turretGOs = new List<GameObject>();
    public List<Rig> rigs = new List<Rig>();
    public List<GameObject> rigGOs = new List<GameObject>();

    StructureStatsManager ssm;

    void Start() {
        InitializeModules();
    }

    void InitializeModules() {
        ssm = GetComponent<StructureStatsManager>();
        GameObject t = new GameObject("Turrets");
        t.transform.parent = transform;
        t.transform.localPosition = Vector3.zero;
        t.transform.localRotation = Quaternion.identity;
        int allowedTurrets = ssm.profile.maxTurrets;
        for(int i = 0; i < allowedTurrets; i++) {
            GameObject turret = new GameObject("Turret");
            turret.transform.parent = t.transform;
            turret.AddComponent<TurretAttachmentPoint>();
            turret.transform.localPosition = ssm.profile.turretLocations[i];
        }
        if (turrets.Count != allowedTurrets) turrets = new List<Turret>(allowedTurrets);
        turretGOs = new List<GameObject>(allowedTurrets);
        for (int i = 0; i < allowedTurrets; i++)
        {
            GameObject turretGO = t.transform.GetChild(i).gameObject;
            turretGOs.Add(turretGO);
            if (turrets[i] != null)
            {
                turretGO.GetComponent<TurretAttachmentPoint>().turret = turrets[i];
                turretGO.GetComponent<TurretAttachmentPoint>().LoadAmmo(turrets[i].accepted[0], 100);
                turretGO.GetComponent<TurretAttachmentPoint>().Initialize();
            }
        }
        GameObject r = new GameObject("Rigs");
        r.transform.parent = transform;
        r.transform.localPosition = Vector3.zero;
        r.transform.localRotation = Quaternion.identity;
        int allowedRigs = ssm.profile.maxRigs;
        for(int i = 0; i < allowedRigs; i++) {
            GameObject rig = new GameObject("Rig");
            rig.transform.parent = r.transform;
            rig.AddComponent<RigAttachmentPoint>();
            rig.transform.localPosition = Vector3.zero;
        }
        if (rigs.Count != allowedRigs) rigs = new List<Rig>(allowedRigs);
        rigGOs = new List<GameObject>(allowedRigs);
        for (int i = 0; i < allowedRigs; i++)
        {
            GameObject rigGO = r.transform.GetChild(i).gameObject;
            rigGOs.Add(rigGO);
            if (rigs[i] != null)
            {
                rigGO.GetComponent<RigAttachmentPoint>().rig = rigs[i];
                rigGO.GetComponent<RigAttachmentPoint>().Initialize();
            }
        }
    }

    public void TryActivateAllWeapons(GameObject to) {
        for(int i = 0; i < turretGOs.Count; i++) {
            turretGOs[i].GetComponent<TurretAttachmentPoint>().target = to;
            turretGOs[i].GetComponent<TurretAttachmentPoint>().SetModuleActive(true);
        }
    }

    public void TryActivateWeapon(int index, GameObject to) {
        turretGOs[index].GetComponent<TurretAttachmentPoint>().target = to;
        turretGOs[index].GetComponent<TurretAttachmentPoint>().SetModuleActive(true);
    }

    public void ToggleWeapon(int index, GameObject to, bool b) {
        turretGOs[index].GetComponent<TurretAttachmentPoint>().target = to;
        turretGOs[index].GetComponent<TurretAttachmentPoint>().SetModuleActive(b);
    }

    public void TryActivateAllRigs() {
        for(int i = 0; i < rigGOs.Count; i++) rigGOs[i].GetComponent<RigAttachmentPoint>().SetModuleActive(true);
    }

    public void TryActivateRig(int index) {
        rigGOs[index].GetComponent<RigAttachmentPoint>().SetModuleActive(true);
    }

    public void ToggleRig(int index, bool b) {
        rigGOs[index].GetComponent<RigAttachmentPoint>().SetModuleActive(b);
    }
}
