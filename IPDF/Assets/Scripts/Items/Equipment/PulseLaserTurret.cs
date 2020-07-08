using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Essentials;

[CreateAssetMenu (fileName = "New Pulse Laser Turret", menuName = "Equipment/Turrets/Pulse Laser Turret")]
public class PulseLaserTurret : Turret {
    [Header ("Appearance")]
    public GameObject asset;
    public Color beamColor;
    public float beamWidth;
    public float beamDuration;

    public override void AlterStats (TurretHandler caller) {
        if (!CanSustain (caller, caller.target)) caller.Deactivate ();
        if (caller.activated) caller.storedEnergy = 0;
    }

    public override void InitializeProjectile (TurretHandler caller, GameObject projectile) {
        PulseLaserProjectile laserProjectile = projectile.AddComponent<PulseLaserProjectile> ();
        laserProjectile.handler = caller;
        laserProjectile.from = caller.equipper;
        laserProjectile.to = caller.equipper.targeted;
        laserProjectile.Initialize ();
    }

    public override bool CanActivate (TurretHandler caller, GameObject target) {
        if (!CanSustain (caller, target)) return false;
        return true;
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