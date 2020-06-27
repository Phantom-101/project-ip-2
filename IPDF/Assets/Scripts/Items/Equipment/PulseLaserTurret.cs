using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Essentials;

[CreateAssetMenu (fileName = "New Pulse Laser Turret", menuName = "Equipment/Turrets/Pulse Laser Turret")]
public class PulseLaserTurret : Turret {
    [Header ("Appearance")]
    public VisualEffectAsset asset;
    public Gradient beamGradient;
    public float beamWidth;

    public override void AlterStats (TurretHandler caller) {
        if (!CanFire (caller, caller.target)) caller.Deactivate ();
        if (caller.activated) caller.storedEnergy = 0;
    }

    public override void InitializeProjectile (TurretHandler caller, GameObject projectile) {
        PulseLaserProjectile laserProjectile = projectile.AddComponent<PulseLaserProjectile> ();
        laserProjectile.handler = caller;
        laserProjectile.from = caller.equipper;
        laserProjectile.to = caller.equipper.targeted;
        laserProjectile.Initialize ();
    }

    public override bool CanFire (TurretHandler caller, GameObject target) {
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
        return CanFire (caller, target);
    }

    public override bool CanUseAmmunition (TurretHandler caller, Ammunition ammunition) {
        return false;
    }
}