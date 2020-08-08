/*

$$$$$$$$\ $$\                       $$\                                   $$\                     
$$  _____|$$ |                      $$ |                                  \__|                    
$$ |      $$ | $$$$$$\   $$$$$$$\ $$$$$$\    $$$$$$\   $$$$$$\  $$$$$$$\  $$\  $$$$$$$\  $$$$$$$\ 
$$$$$\    $$ |$$  __$$\ $$  _____|\_$$  _|  $$  __$$\ $$  __$$\ $$  __$$\ $$ |$$  _____|$$  _____|
$$  __|   $$ |$$$$$$$$ |$$ /        $$ |    $$ |  \__|$$ /  $$ |$$ |  $$ |$$ |$$ /      \$$$$$$\  
$$ |      $$ |$$   ____|$$ |        $$ |$$\ $$ |      $$ |  $$ |$$ |  $$ |$$ |$$ |       \____$$\ 
$$$$$$$$\ $$ |\$$$$$$$\ \$$$$$$$\   \$$$$  |$$ |      \$$$$$$  |$$ |  $$ |$$ |\$$$$$$$\ $$$$$$$  |
\________|\__| \_______| \_______|   \____/ \__|       \______/ \__|  \__|\__| \_______|\_______/ 

*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

[CreateAssetMenu (fileName = "New Electronics", menuName = "Equipment/Electronics")]
public class Electronics : Equipment {
    public float cloakTime;
    public float maxStoredEnergy;
    public float rechargeRate;
    public float consumptionRate;
    public float activationThreshold;
}

[Serializable]
public class ElectronicsHandler : EquipmentHandler {
    public Electronics electronics;
    public bool online;
    public bool activated;
    public float storedEnergy;
    public float timeSinceToggled;

    public ElectronicsHandler (Electronics electronics = null) {
        if (electronics == null) {
            this.electronics = null;
            online = false;
            activated = false;
            storedEnergy = 0.0f;
            timeSinceToggled = 0.0f;
        } else {
            this.electronics = electronics;
            online = true;
            activated = false;
            storedEnergy = 0.0f;
            timeSinceToggled = 0.0f;
        }
        EnforceEquipment ();
    }

    public ElectronicsHandler (ElectronicsHandler electronicsHandler) {
        electronics = electronicsHandler.electronics;
        online = electronicsHandler.online;
        activated = electronicsHandler.activated;
        storedEnergy = electronicsHandler.storedEnergy;
        timeSinceToggled = electronicsHandler.timeSinceToggled;
        EnforceEquipment ();
    }

    public override void SetOnline (bool target) {
        if (electronics == null) {
            online = false;
            activated = false;
            storedEnergy = 0.0f;
            timeSinceToggled = 0.0f;
            return;
        }
        online = target;
        if (!online) Deactivate();
    }

    public float TransferEnergy (float deltaTime, float available) {
        if (!online || electronics == null) return available;
        float transferred = MathUtils.Clamp (MathUtils.Clamp (electronics.rechargeRate * deltaTime, 0.0f, electronics.maxStoredEnergy - storedEnergy), 0.0f, available);
        storedEnergy += transferred;
        return available - transferred;
    }

    public void Activate () {
        if (!online) return;
        if (electronics == null || storedEnergy < electronics.activationThreshold) return;
        activated = true;
        timeSinceToggled = 0.0f;
    }

    public void Deactivate () {
        activated = false;
        timeSinceToggled = 0.0f;
    }

    public override void Process (float deltaTime) {
        if (!online) return;
        timeSinceToggled += deltaTime;
        if (activated) {
            storedEnergy = MathUtils.Clamp (storedEnergy - electronics.consumptionRate * deltaTime, 0.0f, electronics.maxStoredEnergy);
            if (storedEnergy == 0.0f) Deactivate();
            else if (timeSinceToggled >= electronics.cloakTime) equipper.cloaked = false;
        } else if (timeSinceToggled >= electronics.cloakTime) equipper.cloaked = true;
    }

    public override string GetSlotName () {
        return "Electronics";
    }

    public override Type GetEquipmentType () {
        return typeof (Electronics);
    }

    public override void EnforceEquipment () {
        if (!EquipmentAllowed (electronics)) electronics = null;
    }

    public override bool EquipmentAllowed (Equipment equipment) {
        if (equipment == null) return true;
        if (!equipment.GetType ().IsSubclassOf (typeof (Electronics))) return false;
        if (equipment.meta > equipper.profile.maxEquipmentMeta) return false;
        return true;
    }

    public override bool TrySetEquipment (Equipment target) {
        if (!EquipmentAllowed (target)) return false;
        electronics = target as Electronics;
        storedEnergy = 0;
        return true;
    }

    public override string GetEquippedName () {
        return electronics?.name ?? "None";
    }
}