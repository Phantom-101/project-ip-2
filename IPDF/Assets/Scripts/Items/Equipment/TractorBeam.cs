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
public class TractorBeam : Item {
    public float consumptionRate;
    public float activationThreshold;
    public float range;
    public float power;
    public float maxStoredEnergy;
    public float rechargeRate;
}

[Serializable]
public class TractorBeamHandler {
    public StructureBehaviours equipper;
    public TractorBeam tractorBeam;
    public bool online;
    public bool activated;
    public float storedEnergy;
    public GameObject target;
    
    public TractorBeamHandler (StructureBehaviours equipper, TractorBeam tractorBeam = null) {
        this.equipper = equipper;
        if (tractorBeam == null) {
            this.tractorBeam = null;
            this.online = false;
            this.activated = false;
            this.storedEnergy = 0.0f;
            this.target = null;
        } else {
            this.tractorBeam = tractorBeam;
            this.online = true;
            this.activated = false;
            this.storedEnergy = 0.0f;
            this.target = null;
        }
    }

    public TractorBeamHandler (TractorBeamHandler tractorBeamHandler, StructureBehaviours equipper) {
        this.equipper = equipper;
        this.tractorBeam = tractorBeamHandler.tractorBeam;
        this.online = tractorBeamHandler.online;
        this.activated = tractorBeamHandler.activated;
        this.storedEnergy = tractorBeamHandler.storedEnergy;
        this.target = tractorBeamHandler.target;
    }

    public float TransferEnergy (float available) {
        if (!online) return available;
        float transferred = MathUtils.Clamp (MathUtils.Clamp (tractorBeam.rechargeRate * Time.deltaTime, 0.0f, tractorBeam.maxStoredEnergy - storedEnergy), 0.0f, available);
        storedEnergy += transferred;
        return available - transferred;
    }

    public void SetOnline (bool target) {
        if (tractorBeam == null) {
            online = false;
            storedEnergy = 0.0f;
            return;
        }
        online = target;
        if (!online) storedEnergy = 0.0f;
    }

    public void Toggle (GameObject activator, GameObject target) {
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

    public void Process (GameObject processor) {
        if (!online) return;
        if (target == null) {
            Deactivate ();
            return;
        }
        if (activated) {
            storedEnergy = MathUtils.Clamp (storedEnergy - tractorBeam.consumptionRate * Time.deltaTime, 0.0f, tractorBeam.maxStoredEnergy);
            if (storedEnergy == 0.0f) Deactivate();
            else target.GetComponent<Rigidbody> ().AddForce ((processor.transform.position - target.transform.position).normalized * tractorBeam.power / target.GetComponent<Rigidbody> ().mass, ForceMode.Acceleration);
        }
    }
}