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
    public Material projectileMat;
    public Color trailColor;
    [Header ("Turret Stats")]
    public float maxStoredEnergy;
    public float rechargeRate;
    [Header ("Activation Requirements")]
    public Ammunition[] acceptedAmmunitions;
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

    public void UseAmmunition (Ammunition ammunition) {
        usingAmmunition = ammunition;
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
            if (turret.projectile != null) {
                GameObject projectile = MonoBehaviour.Instantiate (turret.projectile,
                    equipper.transform.position + equipper.transform.rotation * offset,
                    (turret.projectileInitializeRotation ? Quaternion.LookRotation (
                        CalculateLeadPosition (
                            equipper.transform.position,
                            target.transform.position + target.transform.rotation * target.GetComponent<StructureBehaviours> ().profile.offset,
                            target.GetComponent<Rigidbody> ().velocity,
                            turret.projectileVelocity,
                            turret.leadProjectile
                        )
                    ) : equipper.transform.rotation) * RandomQuaternion (turret.projectileInaccuracy)
                ) as GameObject;
                projectile.GetComponent<Projectile> ().Initialize (turret, equipper.gameObject, target, storedEnergy / turret.maxStoredEnergy);
            }
            storedEnergy = 0.0f;
        }
    }

    public Vector3 CalculateLeadPosition (Vector3 currentPosition, Vector3 targetPosition, Vector3 targetVelocity, float projectileVelocity, bool lead) {
        if (!lead) return targetPosition - currentPosition;
        float distance = Vector3.Distance(currentPosition, targetPosition);
        float travelTime = distance / projectileVelocity;
        return targetPosition + targetVelocity * travelTime - currentPosition;
    }

    public Quaternion RandomQuaternion (float maxRandom) {
        return Quaternion.Euler (
            UnityEngine.Random.Range(-maxRandom, maxRandom),
            UnityEngine.Random.Range(-maxRandom, maxRandom),
            UnityEngine.Random.Range(-maxRandom, maxRandom)
        );
    }
}