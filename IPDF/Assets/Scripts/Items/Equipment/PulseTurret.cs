using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Essentials;

[CreateAssetMenu (fileName = "New Pulse Turret", menuName = "Equipment/Turrets/Pulse Turret")]
public class PulseTurret : Turret {
    [Header ("Appearance")]
    public GameObject asset;
    public Color beamColor;
    public float beamWidth;
    public float beamDuration;
    [Header ("Turret Stats")]
    public float damage;

    public override void InitializeProjectile (TurretHandler caller, GameObject projectile) {
        PulseProjectile pulseProjectile = projectile.AddComponent<PulseProjectile> ();
        pulseProjectile.handler = caller;
        pulseProjectile.from = caller.equipper;
        pulseProjectile.to = caller.equipper.targeted;
        pulseProjectile.Initialize ();
    }

    public override bool CanActivate (TurretHandler caller, GameObject target) {
        if (!CanSustain (caller, target)) return false;
        Vector3 pos = caller.equipper.transform.localPosition + caller.equipper.transform.rotation * caller.position;
        RaycastHit hit; 
        if (Physics.Raycast (pos, target.transform.localPosition - pos, out hit, range)) {
            StructureBehaviours hitStructure = hit.transform.GetComponent<StructureBehaviours> ();
            if (hitStructure != target.GetComponent<StructureBehaviours> ()) return false;
        } else return false;
        return true;
    }

    public override void Activated (TurretHandler caller) {
        caller.storedEnergy = 0;
    }

    public override bool CanSustain (TurretHandler caller, GameObject target) {
        if (caller == null || !caller.equipper.CanShoot ()) return false;
        if (target == null) return false;
        StructureBehaviours targetBehaviours = target.GetComponent<StructureBehaviours> ();
        if (targetBehaviours != null && !targetBehaviours.CanBeTargeted ()) return false;
        if (caller.storedEnergy < maxStoredEnergy) return false;
        if (!caller.equipper.transform.parent.gameObject.GetComponent<Sector> ()) return false;
        if ((target.transform.localPosition - caller.equipper.transform.localPosition).sqrMagnitude > range * range) return false;
        float angle = target.transform.position - caller.equipper.transform.position == Vector3.zero ?
            0.0f :
            Quaternion.Angle (caller.equipper.transform.rotation * Quaternion.Euler (caller.rotation), Quaternion.LookRotation (target.transform.position - caller.equipper.transform.position)
        );
        if (angle > caller.angle) return false;
        return true;
    }

    public override bool CanInteract (TurretHandler caller, GameObject target) {
        if (caller.activated) return true;
        return CanActivate (caller, target);
    }

    public override bool CanUseAmmunition (TurretHandler caller, Ammunition ammunition) {
        return false;
    }
}