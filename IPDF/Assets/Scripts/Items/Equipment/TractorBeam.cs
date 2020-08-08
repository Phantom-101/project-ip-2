/*

$$$$$$$$\                                 $$\                               $$$$$$$\                                              
\__$$  __|                                $$ |                              $$  __$$\                                             
   $$ |    $$$$$$\   $$$$$$\   $$$$$$$\ $$$$$$\    $$$$$$\   $$$$$$\        $$ |  $$ | $$$$$$\   $$$$$$\  $$$$$$\$$$$\   $$$$$$$\ 
   $$ |   $$  __$$\  \____$$\ $$  _____|\_$$  _|  $$  __$$\ $$  __$$\       $$$$$$$\ |$$  __$$\  \____$$\ $$  _$$  _$$\ $$  _____|
   $$ |   $$ |  \__| $$$$$$$ |$$ /        $$ |    $$ /  $$ |$$ |  \__|      $$  __$$\ $$$$$$$$ | $$$$$$$ |$$ / $$ / $$ |\$$$$$$\  
   $$ |   $$ |      $$  __$$ |$$ |        $$ |$$\ $$ |  $$ |$$ |            $$ |  $$ |$$   ____|$$  __$$ |$$ | $$ | $$ | \____$$\ 
   $$ |   $$ |      \$$$$$$$ |\$$$$$$$\   \$$$$  |\$$$$$$  |$$ |            $$$$$$$  |\$$$$$$$\ \$$$$$$$ |$$ | $$ | $$ |$$$$$$$  |
   \__|   \__|       \_______| \_______|   \____/  \______/ \__|            \_______/  \_______| \_______|\__| \__| \__|\_______/ 

*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

[CreateAssetMenu (fileName = "New Tractor Beam", menuName = "Equipment/Tractor Beam")]
public class TractorBeam : Equipment {
    public float consumptionRate;
    public float activationThreshold;
    public float range;
    public float power;
    public float maxStoredEnergy;
    public float rechargeRate;
}

[Serializable]
public class TractorBeamHandler : EquipmentHandler {
    public TractorBeam tractorBeam;
    public bool online;
    public bool activated;
    public float storedEnergy;
    public GameObject target;
    
    public TractorBeamHandler (TractorBeam tractorBeam = null) {
        if (tractorBeam == null) {
            this.tractorBeam = null;
            online = false;
            activated = false;
            storedEnergy = 0.0f;
            target = null;
        } else {
            this.tractorBeam = tractorBeam;
            online = true;
            activated = false;
            storedEnergy = 0.0f;
            target = null;
        }
        EnforceEquipment ();
    }

    public TractorBeamHandler (TractorBeamHandler tractorBeamHandler) {
        tractorBeam = tractorBeamHandler.tractorBeam;
        online = tractorBeamHandler.online;
        activated = tractorBeamHandler.activated;
        storedEnergy = tractorBeamHandler.storedEnergy;
        target = tractorBeamHandler.target;
        EnforceEquipment ();
    }

    public float TransferEnergy (float deltaTime, float available) {
        if (!online) return available;
        float transferred = MathUtils.Clamp (MathUtils.Clamp (tractorBeam.rechargeRate * deltaTime, 0.0f, tractorBeam.maxStoredEnergy - storedEnergy), 0.0f, available);
        storedEnergy += transferred;
        return available - transferred;
    }

    public override void SetOnline (bool target) {
        if (tractorBeam == null) {
            online = false;
            storedEnergy = 0.0f;
            return;
        }
        online = target;
        if (!online) storedEnergy = 0.0f;
    }

    public void Interacted (GameObject activator, GameObject target) {
        if (!activated) Activate (activator, target);
        else Deactivate ();
    }

    public void Activate (GameObject activator, GameObject target) {
        if (!online) return;
        if (tractorBeam == null || storedEnergy < tractorBeam.activationThreshold) return;
        activated = true;
        this.target = target;
    }

    public void Deactivate () {
        activated = false;
        target = null;
    }

    public override void Process (float deltaTime) {
        if (!online) return;
        if (target == null) {
            Deactivate ();
            return;
        }
        float minSqrMagnitude = equipper.profile.apparentSize + target.GetComponent<StructureBehaviours> ().profile.apparentSize;
        minSqrMagnitude *= minSqrMagnitude;
        if ((equipper.transform.position - target.transform.position).sqrMagnitude <= minSqrMagnitude) Deactivate ();
        if (activated) {
            storedEnergy = MathUtils.Clamp (storedEnergy - tractorBeam.consumptionRate * deltaTime, 0.0f, tractorBeam.maxStoredEnergy);
            if (storedEnergy == 0.0f) Deactivate();
            else target.GetComponent<Rigidbody> ().AddForce ((equipper.transform.position - target.transform.position).normalized * tractorBeam.power / target.GetComponent<Rigidbody> ().mass, ForceMode.Acceleration);
        }
    }

    public override string GetSlotName () {
        return "Tractor Beam";
    }

    public override Type GetEquipmentType () {
        return typeof (TractorBeam);
    }

    public override void EnforceEquipment () {
        if (!EquipmentAllowed (tractorBeam)) tractorBeam = null;
    }

    public override bool EquipmentAllowed (Equipment equipment) {
        if (equipment == null) return true;
        if (!equipment.GetType ().IsSubclassOf (typeof (TractorBeam))) return false;
        if (equipment.meta > equipper.profile.maxEquipmentMeta) return false;
        return true;
    }

    public override bool TrySetEquipment (Equipment target) {
        if (!EquipmentAllowed (target)) return false;
        tractorBeam = target as TractorBeam;
        storedEnergy = 0;
        return true;
    }

    public override string GetEquippedName () {
        return tractorBeam?.name ?? "None";
    }
}