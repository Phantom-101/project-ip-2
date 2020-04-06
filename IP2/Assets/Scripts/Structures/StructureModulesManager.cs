using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureModulesManager : MonoBehaviour
{
    public List<Turret> turrets = new List<Turret>();
    public List<GameObject> turretGOs = new List<GameObject>();

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
            GameObject turretGO = transform.Find("Turrets").GetChild(i).gameObject;
            turretGOs.Add(turretGO);
            if (turrets[i] != null)
            {
                turretGO.GetComponent<TurretAttachmentPoint>().turret = turrets[i];
                turretGO.GetComponent<TurretAttachmentPoint>().LoadAmmo(turrets[i].accepted[0], 100);
                turretGO.GetComponent<TurretAttachmentPoint>().Initialize();
            }
        }
    }

    public void TryActivateAllWeapons(GameObject to) {
        if(to == null) return;
        for(int i = 0; i < turretGOs.Count; i++) {
            turretGOs[i].GetComponent<TurretAttachmentPoint>().target = to;
            turretGOs[i].GetComponent<TurretAttachmentPoint>().SetModuleActive(true);
        }
    }

    public void TryActivateWeapon(int index, GameObject to) {
        if(to == null) return;
        turretGOs[index].GetComponent<TurretAttachmentPoint>().target = to;
        turretGOs[index].GetComponent<TurretAttachmentPoint>().SetModuleActive(true);
    }

    public void ToggleWeapon(int index, GameObject to, bool b) {
        if(b && to == null) return;
        turretGOs[index].GetComponent<TurretAttachmentPoint>().target = to;
        turretGOs[index].GetComponent<TurretAttachmentPoint>().SetModuleActive(b);
    }
}
