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
    public float beamWidth;
    public float beamDuration;
    [Header ("Turret Stats")]
    public float damage;

    public override void InitializeProjectile (TurretHandler caller, GameObject projectile) {
        PulseProjectile pulseProjectile = projectile.GetComponent<PulseProjectile> ();
        if (pulseProjectile == null) pulseProjectile = projectile.AddComponent<PulseProjectile> ();
        pulseProjectile.handler = caller;
        pulseProjectile.from = caller.equipper;
        pulseProjectile.to = caller.target.GetComponent<StructureBehaviours> ();
        pulseProjectile.Initialize ();
        pulseProjectile.Enable ();
    }

    public override bool CanActivate (TurretHandler caller, GameObject target) {
        if (caller == null || !caller.equipper.CanShoot ()) return false;
        if (target == null) return false;
        if (caller.storedEnergy < maxStoredEnergy) return false;
        StructureBehaviours targetBehaviours = target.GetComponent<StructureBehaviours> ();
        if (targetBehaviours != null && !targetBehaviours.CanBeTargeted ()) return false;
        if (caller.equipper.sector == null) return false;
        if (Vector3.Distance (target.transform.localPosition, caller.equipper.transform.localPosition) > range) return false;
        float angle = target.transform.position - caller.equipper.transform.position == Vector3.zero ?
            0.0f :
            Quaternion.Angle (caller.equipper.transform.rotation * Quaternion.Euler (caller.rotation), Quaternion.LookRotation (target.transform.position - caller.equipper.transform.position)
        );
        if (angle > caller.angle) return false;
        return true;
    }

    public override void Activated (TurretHandler caller) {
        caller.storedEnergy = 0;
    }

    public override bool CanInteract (TurretHandler caller, GameObject target) {
        if (caller.activated) return true;
        return CanActivate (caller, target);
    }
}