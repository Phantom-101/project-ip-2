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
public class Turret : Item {
    [Header ("Appearance")]
    public GameObject projectile;
    public Color trailColor;
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
    public bool online;
    public float storedEnergy;

    public TurretHandler (StructureBehaviours equipper, Turret turret = null) {
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
    }

    public TurretHandler (TurretHandler turretHandler, StructureBehaviours equipper) {
        this.equipper = equipper;
        this.turret = turretHandler.turret;
        this.usingAmmunition = turretHandler.usingAmmunition;
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
        float transferred = MathUtils.Clamp (MathUtils.Clamp (turret.rechargeRate * Time.deltaTime, 0.0f, turret.maxStoredEnergy - storedEnergy), 0.0f, available);
        storedEnergy += transferred;
        return available - transferred;
    }

    public void Activate (GameObject target) {
        if (!online || turret == null || equipper == null || target == null) return;
        if ((target.transform.position - equipper.transform.position).sqrMagnitude > turret.range * turret.range) return;
        if (storedEnergy >= turret.activationThreshold * turret.maxStoredEnergy) {
            Vector3 offset = Vector3.zero;
            for (int i = 0; i < equipper.turrets.Count; i++)
                if (equipper.turrets[i] == this)
                    offset = equipper.profile.turretPositions[i];
            equipper.InstantiateProjectiles (this, target, offset);
        }
    }
}