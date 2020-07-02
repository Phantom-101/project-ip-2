using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Essentials;

[CreateAssetMenu (fileName = "New Beam Laser Turret", menuName = "Equipment/Turrets/Beam Laser Turret")]
public class BeamLaserTurret : Turret {
    [Header ("Appearance")]
    public GameObject asset;
    public Color beamColor;
    public float beamWidth;
    [Header ("Turret Stats")]
    public float depletionRate;
    [Header ("Activation Requirements")]
    public float activationThreshold;

    public override void AlterStats (TurretHandler caller) {
        if (!CanSustain (caller, caller.target)) caller.Deactivate ();
        if (caller.storedEnergy < depletionRate * Time.deltaTime) caller.Deactivate ();
        if (caller.activated) caller.storedEnergy -= depletionRate * Time.deltaTime;
    }

    public override void InitializeProjectile (TurretHandler caller, GameObject projectile) {
        BeamLaserProjectile laserProjectile = projectile.AddComponent<BeamLaserProjectile> ();
        laserProjectile.handler = caller;
        laserProjectile.from = caller.equipper;
        laserProjectile.to = caller.equipper.targeted;
        laserProjectile.Initialize ();
    }

    public override bool CanActivate (TurretHandler caller, GameObject target) {
        if (caller.activated || caller.projectile != null) return false;
        if (caller.storedEnergy / maxStoredEnergy < activationThreshold) return false;
        if (!CanSustain (caller, target)) return false;
        return true;
    }

    public override bool CanSustain (TurretHandler caller, GameObject target) {
        if (target == null) return false;
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