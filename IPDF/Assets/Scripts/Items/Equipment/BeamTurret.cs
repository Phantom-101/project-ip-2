using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

[CreateAssetMenu (fileName = "New Beam Turret", menuName = "Equipment/Turrets/Beam Turret")]
public class BeamTurret : Turret {
    [Header ("Appearance")]
    public Material beamMaterial;
    public Gradient beamGradient;
    public AnimationCurve beamWidth;
    [Header ("Turret Stats")]
    public float depletionRate;

    public override void AlterStats (TurretHandler caller) {
        if (!CanFire (caller, caller.target)) caller.Deactivate ();
        if (caller.storedEnergy < depletionRate * Time.deltaTime) caller.Deactivate ();
        if (caller.activated) {
            caller.storedEnergy -= depletionRate * Time.deltaTime;
        }
    }

    public override void InitializeProjectile (TurretHandler caller, GameObject projectile) {
        BeamProjectile beamProjectile = projectile.AddComponent<BeamProjectile> ();
        beamProjectile.handler = caller;
        beamProjectile.from = caller.equipper;
        beamProjectile.to = caller.equipper.targeted;
        beamProjectile.Initialize ();
    }

    public override bool CanFire (TurretHandler caller, GameObject target) {
        if (target == null) return false;
        if (!caller.equipper.transform.parent.gameObject.GetComponent<Sector> ()) return false;
        float angle = target.transform.position - caller.equipper.transform.position == Vector3.zero ?
            0.0f :
            Quaternion.Angle (caller.equipper.transform.rotation * Quaternion.Euler (caller.rotation), Quaternion.LookRotation (target.transform.position - caller.equipper.transform.position)
        );
        if (angle > caller.angle) return false;
        return true;
    }

    public override bool CanInteract (TurretHandler caller, GameObject target) {
        if (caller.activated) return true;
        return CanFire (caller, target);
    }

    public override bool CanUseAmmunition (TurretHandler caller, Ammunition ammunition) {
        return false;
    }
}