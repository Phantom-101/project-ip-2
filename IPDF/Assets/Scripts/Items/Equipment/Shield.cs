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
using UnityEngine;
using Essentials;

[CreateAssetMenu (fileName = "New Shield", menuName = "Equipment/Shield")]
public class Shield : Equipment {
    public float strength;
    public float rechargeRate;
    public float shieldRechargeEfficiency;
}

[Serializable]
public class ShieldHandler : EquipmentHandler {
    public Shield shield;
    public bool online;
    public float[] strengths;
    public float[] shieldTimesSinceLastDamaged;

    public ShieldHandler (Shield shield = null) {
        if (shield == null) {
            this.shield = null;
            online = false;
            strengths = new float[6];
            shieldTimesSinceLastDamaged = new float[6];
        } else {
            this.shield = shield;
            online = true;
            strengths = new float[6];
            for (int i = 0; i < strengths.Length; i++) strengths[i] = this.shield.strength;
            shieldTimesSinceLastDamaged = new float[6];
        }
        EnforceEquipment ();
    }

    public ShieldHandler (ShieldHandler shieldHandler) {
        shield = shieldHandler.shield;
        online = shieldHandler.online;
        strengths = shieldHandler.strengths;
        shieldTimesSinceLastDamaged = shieldHandler.shieldTimesSinceLastDamaged;
        EnforceEquipment ();
    }

    public override void SetOnline (bool target) {
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

    public float TransferEnergy (float deltaTime, float available) {
        if (!online || shield == null) return available;
        for (int i = 0; i < strengths.Length; i++) {
            strengths[i] = MathUtils.Clamp (strengths[i], 0, shield.strength);
            float transferred = MathUtils.Clamp (MathUtils.Clamp (shield.rechargeRate * shield.shieldRechargeEfficiency * deltaTime, 0.0f, shield.strength - strengths[i]), 0.0f, available);
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

    public override void Process (float deltaTime) {
        if (!online || shield == null) {
            strengths = new float[6];
            shieldTimesSinceLastDamaged = new float[6];
            return;
        }
        if (strengths.Length != 6) strengths = new float[6];
        if (shieldTimesSinceLastDamaged.Length != 6) shieldTimesSinceLastDamaged = new float[6];
        for (int i = 0; i < strengths.Length; i++)
            strengths[i] = MathUtils.Clamp (strengths[i], 0, shield.strength);
        for (int i = 0; i < shieldTimesSinceLastDamaged.Length; i++) {
            if (shieldTimesSinceLastDamaged[i] > 1.5f) shieldTimesSinceLastDamaged[i] = 0.0f;
            else if (shieldTimesSinceLastDamaged[i] >= 0.3f) shieldTimesSinceLastDamaged[i] += deltaTime;
        }
    }

    public override string GetSlotName () {
        return "Shield";
    }

    public override Type GetEquipmentType () {
        return typeof (Shield);
    }

    public override void EnforceEquipment () {
        if (!EquipmentAllowed (shield)) shield = null;
    }

    public override bool EquipmentAllowed (Equipment equipment) {
        if (equipment == null) return true;
        if (!equipment.GetType ().IsSubclassOf (typeof (Shield))) return false;
        if (equipment.meta > equipper.profile.maxEquipmentMeta) return false;
        return true;
    }

    public override bool TrySetEquipment (Equipment target) {
        if (!EquipmentAllowed (target)) return false;
        shield = target as Shield;
        for (int i = 0; i < strengths.Length; i++) strengths[i] = this.shield.strength;
        shieldTimesSinceLastDamaged = new float[6];
        return true;
    }

    public override string GetEquippedName () {
        return shield?.name ?? "None";
    }
}