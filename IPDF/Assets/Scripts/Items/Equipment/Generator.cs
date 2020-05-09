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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Generator", menuName = "Equipment/Generator")]
public class Generator : Item {
    public float generation;
}

public class GeneratorHandler {
    public StructureBehaviours equipper;
    public Generator generator;
    public bool online;

    public GeneratorHandler (StructureBehaviours equipper, Generator generator = null) {
        this.equipper = equipper;
        if (generator == null) {
            this.generator = null;
            this.online = false;
        } else {
            this.generator = generator;
            this.online = true;
        }
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
        capacitor.Recharge (generator.generation * Time.deltaTime);
    }
}