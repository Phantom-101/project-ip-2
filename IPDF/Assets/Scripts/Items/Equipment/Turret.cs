/*

$$$$$$$$\                                           $$\               
\__$$  __|                                          $$ |              
   $$ |   $$\   $$\  $$$$$$\   $$$$$$\   $$$$$$\  $$$$$$\    $$$$$$$\ 
   $$ |   $$ |  $$ |$$  __$$\ $$  __$$\ $$  __$$\ \_$$  _|  $$  _____|
   $$ |   $$ |  $$ |$$ |  \__|$$ |  \__|$$$$$$$$ |  $$ |    \$$$$$$\  
   $$ |   $$ |  $$ |$$ |      $$ |      $$   ____|  $$ |$$\  \____$$\ 
   $$ |   \$$$$$$  |$$ |      $$ |      \$$$$$$$\   \$$$$  |$$$$$$$  |
   \__|    \______/ \__|      \__|       \_______|   \____/ \_______/ 

*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

[CreateAssetMenu (fileName = "New Turret", menuName = "Equipment/Turret")]
public class Turret : Equipment {
    [Header ("Appearance")]
    public GameObject projectile;
    public Gradient trailGradient;
    public float trailTime;
    public GameObject explosion;
    public float explosionSize;
    [Header ("Turret Stats")]
    public float maxStoredEnergy;
    public float rechargeRate;
    [Header ("Activation Requirements")]
    public bool requireAmmunition;
    public Ammunition[] acceptedAmmunitions;
    public int activations;
    public float activationDelay;
    public float range;
    public float activationThreshold;
    [Header ("Projectile Movement")]
    public float trackingTime;
    public float projectileVelocity;
    public bool projectileSticky;
    public float projectileTracking;
    [Header ("Projectile Initial Rotation")]
    public bool projectileInitializeRotation;
    public float projectileInaccuracy;
    public bool leadProjectile;
    [Header ("Projectile Stats")]
    public float fuelRange;
    public float damage;
    public float explosiveDamage;
    public float explosionRange;
    public int explosionDetail;
}

[Serializable]
public class TurretHandler {
    public StructureBehaviours equipper;
    public Turret turret;
    public Ammunition usingAmmunition;
    public Vector3 position;
    public TurretAlignment turretAlignment;
    public bool online;
    public float storedEnergy;

    public TurretHandler (StructureBehaviours equipper, Vector3 position, TurretAlignment turretAlignment, Turret turret = null) {
        this.equipper = equipper;
        if (turret == null) {
            this.turret = null;
            this.online = false;
            this.storedEnergy = 0.0f;
        } else {
            this.turret = turret;
            this.online = true;
            this.storedEnergy = 0.0f;
        }
        this.position = position;
        this.turretAlignment = turretAlignment;
    }

    public TurretHandler (TurretHandler turretHandler, Vector3 position, TurretAlignment turretAlignment, StructureBehaviours equipper) {
        this.equipper = equipper;
        this.turret = turretHandler.turret;
        this.usingAmmunition = turretHandler.usingAmmunition;
        this.position = position;
        this.turretAlignment = turretAlignment;
        this.online = turretHandler.online;
        this.storedEnergy = turretHandler.storedEnergy;
    }

    public void SetOnline (bool target) {
        if (turret == null) {
            online = false;
            storedEnergy = 0.0f;
            return;
        }
        online = target;
        if (!online) storedEnergy = 0.0f;
    }

    public bool UseAmmunition (Ammunition ammunition) {
        bool isAccepted = false;
        foreach (Ammunition accepted in turret.acceptedAmmunitions)
            if (ammunition == accepted) {
                isAccepted = true;
                break;
            }
        if (isAccepted) usingAmmunition = ammunition;
        return isAccepted;
    }

    public float TransferEnergy (float available) {
        if (!online) return available;
        if (turret == null) return available;
        float transferred = MathUtils.Clamp (MathUtils.Clamp (turret.rechargeRate * Time.deltaTime, 0.0f, turret.maxStoredEnergy - storedEnergy), 0.0f, available);
        storedEnergy += transferred;
        return available - transferred;
    }

    public void Process () {
        if (turret == null) return;
        if (turret.meta > equipper.profile.maxEquipmentMeta) {
            turret = null;
            return;
        }
    }

    public void Activate (GameObject target) {
        if (CanActivate (target)) equipper.InstantiateProjectiles (this, target, position);
    }

    public bool CanActivate (GameObject target) {
        if (!online || turret == null || equipper == null || target == null) return false;
        if ((target.transform.position - equipper.transform.position).sqrMagnitude > turret.range * turret.range) return false;
        if (!(storedEnergy >= turret.activationThreshold * turret.maxStoredEnergy)) return false;
        if (equipper.electronics.activated) return false;
        if (!AlignmentIsValid (target)) return false;
        StructureBehaviours targetStructureBehaviours = target.GetComponent<StructureBehaviours> ();
        if (targetStructureBehaviours == null || targetStructureBehaviours.profile == null) return false;
        if (!targetStructureBehaviours.initialized) return false;
        if (!targetStructureBehaviours.profile.canFireAt) return false;
        return true;
    }

    public bool AlignmentIsValid (GameObject target) {
        Vector3 targetPos = target.transform.position;
        float angle = targetPos - equipper.transform.position == Vector3.zero ?
            0.0f :
            Quaternion.Angle (equipper.transform.rotation, Quaternion.LookRotation (targetPos - equipper.transform.position));
        Vector3 perp = Vector3.Cross(equipper.transform.forward, targetPos - equipper.transform.position);
        float leftRight = Vector3.Dot(perp, equipper.transform.up);
        angle *= leftRight >= 0.0f ? 1.0f : -1.0f;
        if (turretAlignment == TurretAlignment.All) return true;
        if (turretAlignment == TurretAlignment.ForwardQuadrant && angle >= -45.0f && angle <= 45.0f) return true;
        if (turretAlignment == TurretAlignment.LeftQuadrant && angle >= -135.0f && angle <= -45.0f) return true;
        if (turretAlignment == TurretAlignment.RightQuadrant && angle >= 45.0f && angle <= 135.0f) return true;
        if (turretAlignment == TurretAlignment.BackQuadrant && ((angle >= -180.0f && angle <= -135.0f) || (angle >= 135.0f && angle <= 180.0f))) return true;
        if (turretAlignment == TurretAlignment.ForwardHalf && angle >= -90.0f && angle <= 90.0f) return true;
        if (turretAlignment == TurretAlignment.LeftHalf && angle >= -180.0f && angle <= 0.0f) return true;
        if (turretAlignment == TurretAlignment.RightHalf && angle >= 0.0f && angle <= 180.0f) return true;
        if (turretAlignment == TurretAlignment.BackHalf && ((angle >= -180.0f && angle <= -90.0f) || (angle >= 90.0f && angle <= 180.0f))) return true;
        if (turretAlignment == TurretAlignment.ForwardLeft && angle >= -90.0f && angle <= 0.0f) return true;
        if (turretAlignment == TurretAlignment.BackLeft && angle >= -180.0f && angle <= -90.0f) return true;
        if (turretAlignment == TurretAlignment.ForwardRight && angle >= 0.0f && angle <= 90.0f) return true;
        if (turretAlignment == TurretAlignment.BackRight && angle >= 90.0f && angle <= 180.0f) return true;
        return false;
    }
}

public enum TurretAlignment {
    ForwardQuadrant,
    LeftQuadrant,
    RightQuadrant,
    BackQuadrant,
    ForwardHalf,
    LeftHalf,
    RightHalf,
    BackHalf,
    ForwardLeft,
    BackLeft,
    ForwardRight,
    BackRight,
    All
}