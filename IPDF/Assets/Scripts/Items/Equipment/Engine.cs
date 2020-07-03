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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Engine", menuName = "Equipment/Engine")]
public class Engine : Equipment {
    public float forwardPower;
    public float turnPower;
}

[Serializable]
public class EngineHandler {
    public StructureBehaviours equipper;
    public Engine engine;
    public string engineName;
    public bool online;
    public float forwardSetting;
    public float turnSetting;

    public EngineHandler (Engine engine = null) {
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

    public EngineHandler (EngineHandler engineHandler) {
        this.engine = engineHandler.engine;
        this.online = engineHandler.online;
        this.forwardSetting = engineHandler.forwardSetting;
        this.turnSetting = engineHandler.turnSetting;
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

    public void ApplySettings (float deltaTime, Rigidbody target) {
        if (!online || engine == null) return;
        if (engine.meta > equipper.profile.maxEquipmentMeta) {
            engine = null;
            return;
        }
        target.AddRelativeForce (new Vector3 (0.0f, 0.0f, forwardSetting * engine.forwardPower * deltaTime / target.mass), ForceMode.Acceleration);
        target.AddTorque (new Vector3 (0.0f, turnSetting * engine.turnPower * deltaTime / target.mass, 0), ForceMode.Acceleration);
        float targetZRot = -target.GetComponent<Rigidbody> ().angularVelocity.y * 10;
        target.transform.localEulerAngles = new Vector3 (0.0f, target.transform.localEulerAngles.y, targetZRot);
    }
}