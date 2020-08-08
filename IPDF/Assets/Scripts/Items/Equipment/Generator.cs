/*

 $$$$$$\                                                      $$\                                   
$$  __$$\                                                     $$ |                                  
$$ /  \__| $$$$$$\  $$$$$$$\   $$$$$$\   $$$$$$\   $$$$$$\  $$$$$$\    $$$$$$\   $$$$$$\   $$$$$$$\ 
$$ |$$$$\ $$  __$$\ $$  __$$\ $$  __$$\ $$  __$$\  \____$$\ \_$$  _|  $$  __$$\ $$  __$$\ $$  _____|
$$ |\_$$ |$$$$$$$$ |$$ |  $$ |$$$$$$$$ |$$ |  \__| $$$$$$$ |  $$ |    $$ /  $$ |$$ |  \__|\$$$$$$\  
$$ |  $$ |$$   ____|$$ |  $$ |$$   ____|$$ |      $$  __$$ |  $$ |$$\ $$ |  $$ |$$ |       \____$$\ 
\$$$$$$  |\$$$$$$$\ $$ |  $$ |\$$$$$$$\ $$ |      \$$$$$$$ |  \$$$$  |\$$$$$$  |$$ |      $$$$$$$  |
 \______/  \_______|\__|  \__| \_______|\__|       \_______|   \____/  \______/ \__|      \_______/ 

*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Generator", menuName = "Equipment/Generator")]
public class Generator : Equipment {
    public float generation;
}

[Serializable]
public class GeneratorHandler : EquipmentHandler {
    public Generator generator;
    public bool online;

    public GeneratorHandler (Generator generator = null) {
        if (generator == null) {
            this.generator = null;
            online = false;
        } else {
            this.generator = generator;
            online = true;
        }
        EnforceEquipment ();
    }

    public GeneratorHandler (GeneratorHandler generatorHandler) {
        generator = generatorHandler.generator;
        online = generatorHandler.online;
        EnforceEquipment ();
    }

    public override void SetOnline (bool target) {
        if (generator == null) {
            online = false;
            return;
        }
        online = target;
    }

    public override void Process (float deltaTime) {
        if (!online || generator == null) return;
        equipper.capacitor.Recharge (generator.generation * deltaTime);
    }

    public override string GetSlotName () {
        return "Generator";
    }

    public override Type GetEquipmentType () {
        return typeof (Generator);
    }

    public override void EnforceEquipment () {
        if (!EquipmentAllowed (generator)) generator = null;
    }

    public override bool EquipmentAllowed (Equipment equipment) {
        if (equipment == null) return true;
        if (!equipment.GetType ().IsSubclassOf (typeof (Generator))) return false;
        if (equipment.meta > equipper.profile.maxEquipmentMeta) return false;
        return true;
    }

    public override bool TrySetEquipment (Equipment target) {
        if (!EquipmentAllowed (target)) return false;
        generator = target as Generator;
        return true;
    }

    public override string GetEquippedName () {
        return generator?.name ?? "None";
    }
}