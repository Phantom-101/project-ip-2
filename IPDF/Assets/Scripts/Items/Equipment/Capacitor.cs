/*

 $$$$$$\                                          $$\   $$\                                   
$$  __$$\                                         \__|  $$ |                                  
$$ /  \__| $$$$$$\   $$$$$$\   $$$$$$\   $$$$$$$\ $$\ $$$$$$\    $$$$$$\   $$$$$$\   $$$$$$$\ 
$$ |       \____$$\ $$  __$$\  \____$$\ $$  _____|$$ |\_$$  _|  $$  __$$\ $$  __$$\ $$  _____|
$$ |       $$$$$$$ |$$ /  $$ | $$$$$$$ |$$ /      $$ |  $$ |    $$ /  $$ |$$ |  \__|\$$$$$$\  
$$ |  $$\ $$  __$$ |$$ |  $$ |$$  __$$ |$$ |      $$ |  $$ |$$\ $$ |  $$ |$$ |       \____$$\ 
\$$$$$$  |\$$$$$$$ |$$$$$$$  |\$$$$$$$ |\$$$$$$$\ $$ |  \$$$$  |\$$$$$$  |$$ |      $$$$$$$  |
 \______/  \_______|$$  ____/  \_______| \_______|\__|   \____/  \______/ \__|      \_______/ 
                    $$ |                                                                      
                    $$ |                                                                      
                    \__|                                                                      

*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

[CreateAssetMenu (fileName = "New Capacitor", menuName = "Equipment/Capacitor")]
public class Capacitor : Equipment {
    public float capacitance;
}

[Serializable]
public class CapacitorHandler : EquipmentHandler {
    public Capacitor capacitor;
    public float storedEnergy;

    public CapacitorHandler (Capacitor capacitor = null) {
        if (capacitor == null) {
            this.capacitor = null;
            storedEnergy = 0.0f;
        } else {
            this.capacitor = capacitor;
            storedEnergy = this.capacitor.capacitance;
        }
        EnforceEquipment ();
    }

    public CapacitorHandler (CapacitorHandler capacitorHandler) {
        capacitor = capacitorHandler.capacitor;
        storedEnergy = capacitorHandler.storedEnergy;
        EnforceEquipment ();
    }

    public override void Process (float deltaTime) {
        if (capacitor == null) return;
        DistributeEnergy (deltaTime, equipper.turrets, equipper.shield, equipper.electronics, equipper.tractorBeam);
    }

    public void Recharge (float available) {
        if (capacitor == null) return;
        storedEnergy = MathUtils.Clamp (storedEnergy + available, 0.0f, capacitor.capacitance);
    }

    public void DistributeEnergy (float deltaTime, List<TurretHandler> turrets, ShieldHandler shield, ElectronicsHandler electronics, TractorBeamHandler tractorBeam) {
        DistributeToTurrets (deltaTime, turrets);
        DistributeToShield (deltaTime, shield);
        DistributeToElectronics (deltaTime, electronics);
        DistributeToTractorBeam (deltaTime, tractorBeam);
    }

    public void DistributeToTurrets (float deltaTime, List<TurretHandler> turrets) {
        foreach (TurretHandler turret in turrets) storedEnergy = turret.TransferEnergy (deltaTime, storedEnergy);
    }

    public void DistributeToShield (float deltaTime, ShieldHandler shield) {
        storedEnergy = shield.TransferEnergy (deltaTime, storedEnergy);
    }

    public void DistributeToElectronics (float deltaTime, ElectronicsHandler electronics) {
        storedEnergy = electronics.TransferEnergy (deltaTime, storedEnergy);
    }

    public void DistributeToTractorBeam (float deltaTime, TractorBeamHandler tractorBeam) {
        storedEnergy = tractorBeam.TransferEnergy (deltaTime, storedEnergy);
    }

    public override string GetSlotName () {
        return "Capacitor";
    }

    public override Type GetEquipmentType () {
        return typeof (Capacitor);
    }

    public override void EnforceEquipment () {
        if (!EquipmentAllowed (capacitor)) capacitor = null;
    }

    public override bool EquipmentAllowed (Equipment equipment) {
        if (equipment == null) return true;
        if (!equipment.GetType ().IsSubclassOf (typeof (Capacitor))) return false;
        if (equipment.meta > equipper.profile.maxEquipmentMeta) return false;
        return true;
    }

    public override bool TrySetEquipment (Equipment target) {
        if (!EquipmentAllowed (target)) return false;
        capacitor = target as Capacitor;
        storedEnergy = 0;
        return true;
    }

    public override string GetEquippedName () {
        return capacitor?.name ?? "None";
    }
}