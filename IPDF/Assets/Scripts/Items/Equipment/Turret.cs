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

public class Turret : Equipment {
    [Header ("Appearance")]
    public GameObject impactEffect;
    public float impactEffectSize;
    [Header ("Turret Stats")]
    public float maxStoredEnergy;
    public float rechargeRate;
    [Header ("Activation Requirements")]
    public float range;
    [Header ("Projectile Stats")]
    public float damage;

    public virtual void AlterStats (TurretHandler caller) {}

    public virtual void InitializeProjectile (TurretHandler caller, GameObject projectile) {}

    public virtual bool CanActivate (TurretHandler caller, GameObject target) {
        return false;
    }

    public virtual bool CanSustain (TurretHandler caller, GameObject target) {
        return false;
    }

    public virtual bool CanInteract (TurretHandler caller, GameObject target) {
        return false;
    }

    public virtual bool CanUseAmmunition (TurretHandler caller, Ammunition ammunition) {
        return false;
    }
}

[Serializable]
public class TurretHandler {
    [Header ("Essential Information")]
    public StructureBehaviours equipper;
    public Turret turret;
    public string turretName;
    public Ammunition usingAmmunition;
    [Header ("Transforms")]
    public Vector3 position;
    public Vector3 rotation;
    public float angle;
    [Header ("Stats")]
    public GameObject target;
    public bool online;
    public bool activated;
    public GameObject projectile;
    public float storedEnergy;

    public TurretHandler (Turret turret = null) {
        if (turret == null) {
            this.turret = null;
            this.online = false;
            this.storedEnergy = 0.0f;
        } else {
            this.turret = turret;
            this.online = true;
            this.storedEnergy = 0.0f;
        }
        this.activated = false;
    }

    public TurretHandler (TurretHandler turretHandler) {
        this.turret = turretHandler.turret;
        this.usingAmmunition = turretHandler.usingAmmunition;
        this.online = turretHandler.online;
        this.activated = turretHandler.activated;
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
        if (turret.CanUseAmmunition (this, ammunition)) {
            usingAmmunition = ammunition;
            return true;
        }
        return false;
    }

    public float TransferEnergy (float deltaTime, float available) {
        if (!online) return available;
        if (turret == null) return available;
        float transferred = MathUtils.Clamp (MathUtils.Clamp (turret.rechargeRate * deltaTime, 0.0f, turret.maxStoredEnergy - storedEnergy), 0.0f, available);
        storedEnergy += transferred;
        return available - transferred;
    }

    public void Process (float deltaTime) {
        if (turret == null) return;
        if (turret.meta > equipper.profile.maxEquipmentMeta) {
            turret = null;
            return;
        }
        if (turret != null) turret.AlterStats (this);
        if (activated && !turret.CanSustain (this, target)) Deactivate ();
    }

    public void Interacted (GameObject target) {
        if (activated) Deactivate ();
        else Activate (target);
    }

    public void Activate (GameObject target) {
        if (turret != null && turret.CanActivate (this, target)) {
            this.target = target;
            activated = true;
            projectile = new GameObject (turret.name);
            turret.InitializeProjectile (this, projectile);
        }
    }

    public void Deactivate () {
        activated = false;
    }

    public bool CanPress () {
        if (turret == null) return false;
        if (equipper == null) return false;
        if (!turret.CanInteract (this, equipper.targeted == null ? null : equipper.targeted.gameObject)) return false;
        return true;
    }
}