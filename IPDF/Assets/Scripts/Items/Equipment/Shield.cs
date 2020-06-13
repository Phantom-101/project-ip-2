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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

[CreateAssetMenu (fileName = "New Shield", menuName = "Equipment/Shield")]
public class Shield : Equipment {
    public float strength;
    public float rechargeRate;
    public float shieldRechargeEfficiency;
}

[Serializable]
public class ShieldHandler {
    public StructureBehaviours equipper;
    public Shield shield;
    public bool online;
    public float[] strengths;
    public float[] shieldTimesSinceLastDamaged;

    public ShieldHandler (StructureBehaviours equipper, Shield shield = null) {
        this.equipper = equipper;
        if (shield == null) {
            this.shield = null;
            this.online = false;
            this.strengths = new float[6];
            this.shieldTimesSinceLastDamaged = new float[6];
        } else {
            this.shield = shield;
            this.online = true;
            this.strengths = new float[6];
            for (int i = 0; i < strengths.Length; i++) strengths[i] = this.shield.strength;
            this.shieldTimesSinceLastDamaged = new float[6];
        }
    }

    public ShieldHandler (ShieldHandler shieldHandler, StructureBehaviours equipper) {
        this.equipper = equipper;
        this.shield = shieldHandler.shield;
        this.online = shieldHandler.online;
        this.strengths = shieldHandler.strengths;
        this.shieldTimesSinceLastDamaged = shieldHandler.shieldTimesSinceLastDamaged;
    }

    public void SetOnline (bool target) {
        if (shield == null) {
            online = false;
            strengths = new float[6];
            shieldTimesSinceLastDamaged = new float[6];
            return;
        }
        online = target;
        if (!online) {
            strengths = new float[6];
            shieldTimesSinceLastDamaged = new float[6];
        }
    }

    public float TransferEnergy (float available) {
        if (!online || shield == null) return available;
        for (int i = 0; i < strengths.Length; i++) {
            float transferred = MathUtils.Clamp (MathUtils.Clamp (shield.rechargeRate * shield.shieldRechargeEfficiency * Time.deltaTime, 0.0f, shield.strength - strengths[i]), 0.0f, available);
            strengths[i] += transferred * shield.shieldRechargeEfficiency;
        }
        return available;
    }

    public float TakeDamage (int sector, float amount) {
        if (strengths.Length != 6) strengths = new float[6];
        if (shieldTimesSinceLastDamaged.Length != 6) shieldTimesSinceLastDamaged = new float[6];
        if (strengths[sector] >= amount) {
            strengths[sector] -= amount;
            shieldTimesSinceLastDamaged[sector] = 0.3f;
            return 0.0f;
        } else {
            amount -= strengths[sector];
            strengths[sector] = 0.0f;
            shieldTimesSinceLastDamaged[sector] = 0.3f;
            return amount;
        }
    }

    public void Process () {
        if (!online || shield == null) return;
        if (shield.meta > equipper.profile.maxEquipmentMeta) {
            shield = null;
            return;
        }
        if (strengths.Length != 6) strengths = new float[6];
        if (shieldTimesSinceLastDamaged.Length != 6) shieldTimesSinceLastDamaged = new float[6];
        for (int i = 0; i < shieldTimesSinceLastDamaged.Length; i++) {
            if (shieldTimesSinceLastDamaged[i] > 1.5f) shieldTimesSinceLastDamaged[i] = 0.0f;
            else if (shieldTimesSinceLastDamaged[i] >= 0.3f) shieldTimesSinceLastDamaged[i] += Time.deltaTime;
        }
    }
}