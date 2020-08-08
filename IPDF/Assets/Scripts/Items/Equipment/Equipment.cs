using UnityEngine;
using System;

/*

$$$$$$$$\ $$$$$$$$\ $$$$$$$$\  $$$$$$\  
\__$$  __|$$  _____|$$  _____|$$  __$$\ 
   $$ |   $$ |      $$ |      $$ /  $$ |
   $$ |   $$$$$\    $$$$$\    $$ |  $$ |
   $$ |   $$  __|   $$  __|   $$ |  $$ |
   $$ |   $$ |      $$ |      $$ |  $$ |
   $$ |   $$ |      $$$$$$$$\  $$$$$$  |
   \__|   \__|      \________| \______/ 

1. GeneratorHandler.GenerateEnergy (CapacitorHandler capacitor);
2. CapacitorHandler.DistributeEnergy (List<TurretHandler> turrets, ShieldHandler shield, ElectronicsHandler electronics);
3. EngineHandler.ApplySettings (ConstantForce target);
4. ElectronicsHandler.Process (GameObject processor);
5. TractorBeamHandler.Process (GameObject processor);

*/

public class Equipment : Item {
   public int meta;
}

[Serializable]
public class EquipmentHandler {
    [Header ("Essential Information")]
    public StructureBehaviours equipper;
    public string mountedID;

    public virtual void SetOnline (bool target) { }

    public virtual void Process (float deltaTime) { }

    public virtual void EnforceEquipment () { }

    public virtual bool EquipmentAllowed (Equipment equipment) {
        return false;
    }

    public virtual bool TrySetEquipment (Equipment target) {
        return false;
    }

    public virtual string GetSlotName () {
        return "";
    }

    public virtual string GetEquippedName () {
        return "";
    }

    public virtual Type GetEquipmentType () {
        return null;
    }
}