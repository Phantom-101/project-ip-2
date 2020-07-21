using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Essentials;

[CreateAssetMenu (fileName = "New Beam Turret", menuName = "Equipment/Turrets/Beam Turret")]
public class BeamTurret : Turret {
    [Header ("Appearance")]
    public GameObject asset;
    public Color beamColor;
    public float beamWidth;
    [Header ("Turret Stats")]
    public float depletionRate;
    public float damage;
    [Header ("Activation Requirements")]
    public float activationThreshold;

    public override void InitializeProjectile (TurretHandler caller, GameObject projectile) {
        BeamProjectile beamProjectile = projectile.AddComponent<BeamProjectile> ();
        beamProjectile.handler = caller;
        beamProjectile.from = caller.equipper;
        beamProjectile.to = caller.target.GetComponent<StructureBehaviours> ();
        beamProjectile.Initialize ();
    }

    public override bool CanActivate (TurretHandler caller, GameObject target) {
        if (caller.projectile != null) return false;
        if (caller.storedEnergy / maxStoredEnergy < activationThreshold) return false;
        if (!CanSustain (caller, target)) return false;
        return true;
    }

    public override bool CanSustain (TurretHandler caller, GameObject target) {
        if (caller == null || !caller.equipper.CanShoot ()) return false;
        if (target == null) return false;
        StructureBehaviours targetBehaviours = target.GetComponent<StructureBehaviours> ();
        if (targetBehaviours != null && !targetBehaviours.CanBeTargeted ()) return false;
        if (!caller.equipper.transform.parent.gameObject.GetComponent<Sector> ()) return false;
        float angle = target.transform.position - caller.equipper.transform.position == Vector3.zero ?
            0.0f :
            Quaternion.Angle (caller.equipper.transform.rotation * Quaternion.Euler (caller.rotation), Quaternion.LookRotation (target.transform.position - caller.equipper.transform.position)
        );
        if (angle > caller.angle) return false;
        if ((caller.equipper.transform.position - target.transform.position).sqrMagnitude > range * range) return false;
        return true;
    }

    public override void Sustained (TurretHandler caller) {
        if (caller.storedEnergy < depletionRate * Time.deltaTime) caller.Deactivate ();
        if (caller.activated) caller.storedEnergy -= depletionRate * Time.deltaTime;
    }

    public override bool CanInteract (TurretHandler caller, GameObject target) {
        if (caller.activated) return true;
        return CanActivate (caller, target);
    }

    public override bool CanUseAmmunition (TurretHandler caller, Ammunition ammunition) {
        return false;
    }
}