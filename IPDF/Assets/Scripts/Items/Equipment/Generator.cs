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
public class GeneratorHandler {
    public StructureBehaviours equipper;
    public Generator generator;
    public string generatorName;
    public bool online;

    public GeneratorHandler (Generator generator = null) {
        if (generator == null) {
            this.generator = null;
            this.online = false;
        } else {
            this.generator = generator;
            this.online = true;
        }
    }

    public GeneratorHandler (GeneratorHandler generatorHandler) {
        this.generator = generatorHandler.generator;
        this.online = generatorHandler.online;
    }

    public void SetOnline (bool target) {
        if (generator == null) {
            online = false;
            return;
        }
        online = target;
    }

    public void GenerateEnergy (CapacitorHandler capacitor) {
        if (!online || generator == null) return;
        if (generator.meta > equipper.profile.maxEquipmentMeta) {
            generator = null;
            return;
        }
        capacitor.Recharge (generator.generation * Time.deltaTime);
    }
}