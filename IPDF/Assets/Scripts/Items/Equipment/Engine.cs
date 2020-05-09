/*

$$$$$$$$\                     $$\                               
$$  _____|                    \__|                              
$$ |      $$$$$$$\   $$$$$$\  $$\ $$$$$$$\   $$$$$$\   $$$$$$$\ 
$$$$$\    $$  __$$\ $$  __$$\ $$ |$$  __$$\ $$  __$$\ $$  _____|
$$  __|   $$ |  $$ |$$ /  $$ |$$ |$$ |  $$ |$$$$$$$$ |\$$$$$$\  
$$ |      $$ |  $$ |$$ |  $$ |$$ |$$ |  $$ |$$   ____| \____$$\ 
$$$$$$$$\ $$ |  $$ |\$$$$$$$ |$$ |$$ |  $$ |\$$$$$$$\ $$$$$$$  |
\________|\__|  \__| \____$$ |\__|\__|  \__| \_______|\_______/ 
                    $$\   $$ |                                  
                    \$$$$$$  |                                  
                     \______/                                   

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Engine", menuName = "Equipment/Engine")]
public class Engine : Item {
    public float forwardPower;
    public float turnPower;
}

public class EngineHandler {
    public StructureBehaviours equipper;
    public Engine engine;
    public bool online;
    public float forwardSetting;
    public float turnSetting;

    public EngineHandler (StructureBehaviours equipper, Engine engine = null) {
        this.equipper = equipper;
        if (engine == null) {
            this.engine = null;
            this.online = false;
            this.forwardSetting = 0.0f;
            this.turnSetting = 0.0f;
        } else {
            this.engine = engine;
            this.online = true;
            this.forwardSetting = 0.0f;
            this.turnSetting = 0.0f;
        }
    }

    public void SetOnline (bool target) {
        if (engine == null) {
            online = false;
            forwardSetting = 0.0f;
            turnSetting = 0.0f;
            return;
        }
        online = target;
    }

    public void SetForwardSetting (float target) {
        if (!online || engine == null) return;
        forwardSetting = target;
    }

    public void SetTurnSetting (float target) {
        if (!online || engine == null) return;
        turnSetting = target;
    }

    public void ApplySettings (ConstantForce target) {
        if (!online || engine == null) return;
        target.relativeForce = new Vector3 (0.0f, 0.0f, forwardSetting * engine.forwardPower);
        target.torque = new Vector3 (0.0f, turnSetting * engine.turnPower, 0.0f);
        float targetZRot = -target.GetComponent<Rigidbody> ().angularVelocity.y * 10.0f;
        target.transform.localEulerAngles = new Vector3 (0.0f, target.transform.localEulerAngles.y, targetZRot);
    }
}