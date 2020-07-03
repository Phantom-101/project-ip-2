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
    public string capacitorName;
    public float storedEnergy;

    public CapacitorHandler (Capacitor capacitor = null) {
        if (capacitor == null) {
            this.capacitor = null;
            this.storedEnergy = 0.0f;
        } else {
            this.capacitor = capacitor;
            this.storedEnergy = this.capacitor.capacitance;
        }
    }

    public CapacitorHandler (CapacitorHandler capacitorHandler) {
        this.capacitor = capacitorHandler.capacitor;
        this.storedEnergy = capacitorHandler.storedEnergy;
    }

    public void Recharge (float available) {
        if (capacitor == null) return;
        storedEnergy = MathUtils.Clamp (storedEnergy + available, 0.0f, capacitor.capacitance);
    }

    public void DistributeEnergy (float deltaTime, List<TurretHandler> turrets, ShieldHandler shield, ElectronicsHandler electronics, TractorBeamHandler tractorBeam) {
        if (capacitor == null) return;
        if (capacitor.meta > equipper.profile.maxEquipmentMeta) {
            capacitor = null;
            return;
        }
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
}