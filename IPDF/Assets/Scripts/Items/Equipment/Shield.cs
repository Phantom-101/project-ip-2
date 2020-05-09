/*

 $$$$$$\  $$\       $$\           $$\       $$\           
$$  __$$\ $$ |      \__|          $$ |      $$ |          
$$ /  \__|$$$$$$$\  $$\  $$$$$$\  $$ | $$$$$$$ | $$$$$$$\ 
\$$$$$$\  $$  __$$\ $$ |$$  __$$\ $$ |$$  __$$ |$$  _____|
 \____$$\ $$ |  $$ |$$ |$$$$$$$$ |$$ |$$ /  $$ |\$$$$$$\  
$$\   $$ |$$ |  $$ |$$ |$$   ____|$$ |$$ |  $$ | \____$$\ 
\$$$$$$  |$$ |  $$ |$$ |\$$$$$$$\ $$ |\$$$$$$$ |$$$$$$$  |
 \______/ \__|  \__|\__| \_______|\__| \_______|\_______/ 

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

[CreateAssetMenu (fileName = "New Shield", menuName = "Equipment/Shield")]
public class Shield : Item {
    public float strength;
    public float rechargeRate;
    public float shieldRechargeEfficiency;
}

public class ShieldHandler {
    public StructureBehaviours equipper;
    public Shield shield;
    public bool online;
    public float[] strengths;

    public ShieldHandler (StructureBehaviours equipper, Shield shield = null) {
        this.equipper = equipper;
        if (shield == null) {
            this.shield = null;
            this.online = false;
            this.strengths = new float[6];
        } else {
            this.shield = shield;
            this.online = true;
            this.strengths = new float[6];
            for (int i = 0; i < strengths.Length; i++) strengths[i] = this.shield.strength;
        }
    }

    public void SetOnline (bool target) {
        if (shield == null) {
            online = false;
            strengths = new float[6];
            return;
        }
        online = target;
        if (!online) strengths = new float[6];
    }

    public float TransferEnergy (float available) {
        if (!online || shield == null) return available;
        for (int i = 0; i < strengths.Length; i++) {
            float transferred = MathUtils.Clamp (MathUtils.Clamp (shield.rechargeRate * shield.shieldRechargeEfficiency * Time.deltaTime, 0.0f, shield.strength - strengths[i]), 0.0f, available);
            strengths[i] += transferred * shield.shieldRechargeEfficiency;
        }
        return available;
    }
}