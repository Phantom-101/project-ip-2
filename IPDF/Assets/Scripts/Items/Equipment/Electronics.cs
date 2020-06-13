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
public class ElectronicsHandler {
    public StructureBehaviours equipper;
    public Electronics electronics;
    public bool online;
    public bool activated;
    public float storedEnergy;
    public float timeSinceToggled;

    public ElectronicsHandler (StructureBehaviours equipper, Electronics electronics = null) {
        this.equipper = equipper;
        if (electronics == null) {
            this.electronics = null;
            this.online = false;
            this.activated = false;
            this.storedEnergy = 0.0f;
            this.timeSinceToggled = 0.0f;
        } else {
            this.electronics = electronics;
            this.online = true;
            this.activated = false;
            this.storedEnergy = 0.0f;
            this.timeSinceToggled = 0.0f;
        }
    }

    public ElectronicsHandler (ElectronicsHandler electronicsHandler, StructureBehaviours equipper) {
        this.equipper = equipper;
        this.electronics = electronicsHandler.electronics;
        this.online = electronicsHandler.online;
        this.activated = electronicsHandler.activated;
        this.storedEnergy = electronicsHandler.storedEnergy;
        this.timeSinceToggled = electronicsHandler.timeSinceToggled;
    }

    public void SetOnline (bool target) {
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

    public float TransferEnergy (float available) {
        if (!online || electronics == null) return available;
        float transferred = MathUtils.Clamp (MathUtils.Clamp (electronics.rechargeRate * Time.deltaTime, 0.0f, electronics.maxStoredEnergy - storedEnergy), 0.0f, available);
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

    public void Process (GameObject processor) {
        if (!online) return;
        if (electronics.meta > equipper.profile.maxEquipmentMeta) {
            electronics = null;
            return;
        }
        timeSinceToggled += Time.deltaTime;
        if (activated) {
            storedEnergy = MathUtils.Clamp (storedEnergy - electronics.consumptionRate * Time.deltaTime, 0.0f, electronics.maxStoredEnergy);
            if (storedEnergy == 0.0f) Deactivate();
            else if (timeSinceToggled >= electronics.cloakTime) processor.GetComponent<StructureBehaviours> ().cloaked = false;
        } else if (timeSinceToggled >= electronics.cloakTime) processor.GetComponent<StructureBehaviours> ().cloaked = true;
    }
}