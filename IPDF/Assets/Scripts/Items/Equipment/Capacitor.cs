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
public class CapacitorHandler {
    public StructureBehaviours equipper;
    public Capacitor capacitor;
    public float storedEnergy;

    public CapacitorHandler (StructureBehaviours equipper, Capacitor capacitor = null) {
        this.equipper = equipper;
        if (capacitor == null) {
            this.capacitor = null;
            this.storedEnergy = 0.0f;
        } else {
            this.capacitor = capacitor;
            this.storedEnergy = this.capacitor.capacitance;
        }
    }

    public CapacitorHandler (CapacitorHandler capacitorHandler, StructureBehaviours equipper) {
        this.equipper = equipper;
        this.capacitor = capacitorHandler.capacitor;
        this.storedEnergy = capacitorHandler.storedEnergy;
    }

    public void Recharge (float available) {
        storedEnergy = MathUtils.Clamp (storedEnergy + available, 0.0f, capacitor.capacitance);
    }

    public void DistributeEnergy (List<TurretHandler> turrets, ShieldHandler shield, ElectronicsHandler electronics, TractorBeamHandler tractorBeam) {
        if (capacitor == null) return;
        if (capacitor.meta > equipper.profile.maxEquipmentMeta) {
            capacitor = null;
            return;
        }
        DistributeToTurrets (turrets);
        DistributeToShield (shield);
        DistributeToElectronics (electronics);
        DistributeToTractorBeam (tractorBeam);
    }

    public void DistributeToTurrets (List<TurretHandler> turrets) {
        foreach (TurretHandler turret in turrets) storedEnergy = turret.TransferEnergy (storedEnergy);
    }

    public void DistributeToShield (ShieldHandler shield) {
        storedEnergy = shield.TransferEnergy (storedEnergy);
    }

    public void DistributeToElectronics (ElectronicsHandler electronics) {
        storedEnergy = electronics.TransferEnergy (storedEnergy);
    }

    public void DistributeToTractorBeam (TractorBeamHandler tractorBeam) {
        storedEnergy = tractorBeam.TransferEnergy (storedEnergy);
    }
}