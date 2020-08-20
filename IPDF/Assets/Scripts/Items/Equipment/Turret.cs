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
using UnityEngine;
using Essentials;

public class Turret : Equipment {
    [Header ("Audio")]
    public AudioAsset audio;
    [Header ("Turret Stats")]
    public string variant;
    public float maxStoredEnergy;
    public float rechargeRate;
    [Header ("Activation Requirements")]
    public float range;

    public virtual void InitializeProjectile (TurretHandler caller, GameObject projectile) {}

    public virtual bool CanActivate (TurretHandler caller, GameObject target) {
        return false;
    }

    public virtual void Activated (TurretHandler caller) {}

    public virtual bool CanSustain (TurretHandler caller, GameObject target) {
        return false;
    }

    public virtual void Sustained (TurretHandler caller, float deltaTime) {}

    public virtual bool CanRepeat (TurretHandler caller, GameObject target) {
        return false;
    }

    public virtual bool CanInteract (TurretHandler caller, GameObject target) {
        return false;
    }

    public virtual bool CanUseAmmunition (TurretHandler caller, Ammunition ammunition) {
        return false;
    }

    public virtual GameObject RetrieveFromPool (TurretHandler caller) {
        return null;
    }
}

[Serializable]
public class TurretHandler : EquipmentHandler {
    [Header ("Essential Information")]
    public Turret turret;
    public Ammunition ammunition;
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
    [Header ("Components")]
    public Pooler pooler;

    public TurretHandler (Turret turret = null) {
        if (turret == null) {
            this.turret = null;
            online = false;
            storedEnergy = 0.0f;
        } else {
            this.turret = turret;
            online = true;
            storedEnergy = 0.0f;
        }
        activated = false;
        EnforceEquipment ();
    }

    public TurretHandler (TurretHandler turretHandler) {
        turret = turretHandler.turret;
        ammunition = turretHandler.ammunition;
        online = turretHandler.online;
        activated = turretHandler.activated;
        storedEnergy = turretHandler.storedEnergy;
        EnforceEquipment ();
    }

    public override void SetOnline (bool target) {
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
            this.ammunition = ammunition;
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

    public override void Process (float deltaTime) {
        if (turret == null) return;
        if (pooler == null) pooler = Pooler.GetInstance ();
        if (ammunition == null) {
            if (turret is KineticTurret) {
                foreach (Ammunition possible in (turret as KineticTurret).ammunition)
                    if (equipper.inventory.GetItemCount (possible) > 0) {
                        ammunition = possible;
                        break;
                    }
            }
        } else {
            if (!turret.CanUseAmmunition (this, ammunition) || equipper.inventory.GetItemCount (ammunition) == 0) ammunition = null;
        }
        if (activated) {
            if (!turret.CanRepeat (this, target) && !turret.CanSustain (this, target)) Deactivate ();
            else {
                TryActivate ();
                turret.Sustained (this, deltaTime);
            }
        }
        if (turret.GetType () == typeof (KineticTurret)) UseAmmunition ((turret as KineticTurret).ammunition[0]);
    }

    public override void EnforceEquipment () {
        if (!EquipmentAllowed (turret)) turret = null;
    }

    public override bool EquipmentAllowed (Equipment equipment) {
        if (equipment == null) return true;
        if (!equipment.GetType ().IsSubclassOf (typeof (Turret))) return false;
        if (equipment.meta > equipper.profile.maxEquipmentMeta) return false;
        return true;
    }

    public override bool TrySetEquipment (Equipment target) {
        if (!EquipmentAllowed (target)) return false;
        turret = target as Turret;
        storedEnergy = 0;
        return true;
    }

    public void Interacted (GameObject target) {
        if (activated) Deactivate ();
        else if (turret.CanActivate (this, target)) {
            this.target = target;
            activated = true;
            HandleActivation ();
        }
    }

    public void TryActivate () {
        if (turret.CanActivate (this, target)) HandleActivation ();
    }

    public void Activate (GameObject target) {
        if (turret == null) return;
        if (turret.CanActivate (this, target)) {
            this.target = target;
            activated = true;
            HandleActivation ();
        }
    }

    public void HandleActivation () {
        turret.Activated (this);
        if (projectile == null) projectile = turret.RetrieveFromPool (this);
        turret.InitializeProjectile (this, projectile);
    }

    public void Deactivate () {
        activated = false;
    }

    public bool CanPress () {
        if (turret == null) return false;
        if (equipper == null || equipper.targeted == null) return false;
        if (!turret.CanInteract (this, equipper.targeted.gameObject)) return false;
        return true;
    }

    public override string GetSlotName () {
        return "Turret " + equipper.turrets.IndexOf (this);
    }

    public override Type GetEquipmentType () {
        return typeof (Turret);
    }

    public override string GetEquippedName () {
        return turret?.name ?? "None";
    }
}