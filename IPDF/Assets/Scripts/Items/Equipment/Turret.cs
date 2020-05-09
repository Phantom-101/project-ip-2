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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

[CreateAssetMenu (fileName = "New Turret", menuName = "Equipment/Turret")]
public class Turret : Item {
    public GameObject projectile;
    public Ammunition[] acceptedAmmunitions;
    public float projectileVelocity;
    public bool projectileSticky;
    public float projectileTracking;
    public float projectileInaccuracy;
    public bool leadProjectile;
    public float damage;
    public float range;
    public float maxStoredEnergy;
    public float rechargeRate;
    public float activationThreshold;
}

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

    public void Activate (GameObject activator, GameObject target) {
        if (!online || turret == null || activator == null || target == null) return;
        if (storedEnergy >= turret.activationThreshold * turret.maxStoredEnergy) {
            if (turret.projectile != null) {
                GameObject projectile = MonoBehaviour.Instantiate (turret.projectile,
                    activator.transform.position,
                    Quaternion.LookRotation (
                        CalculateLeadPosition (
                            activator.transform.position,
                            target.transform.position,
                            target.GetComponent<Rigidbody> ().velocity,
                            turret.projectileVelocity,
                            turret.leadProjectile
                        )
                    ) * RandomQuaternion (turret.projectileInaccuracy)
                ) as GameObject;
                projectile.GetComponent<Projectile> ().Initialize (turret, activator, target, storedEnergy / turret.maxStoredEnergy);
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
            Random.Range(-maxRandom, maxRandom),
            Random.Range(-maxRandom, maxRandom),
            Random.Range(-maxRandom, maxRandom)
        );
    }
}