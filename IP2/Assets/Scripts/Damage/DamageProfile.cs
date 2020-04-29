using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DamageProfile", menuName = "ScriptableObjects/DamageProfile")]
public class DamageProfile : ScriptableObject {
    [Header("General")]
    public float value;
    [Header("Effectiveness")]
    public float againstHull;
    public float againstArmor;
    public float againstShield;
    [Header("Bypasses")]
    public bool bypassHull;
    public bool bypassArmor;
    public bool bypassShield;
}

public struct DamageProfileStruct {
    public float value;
    public float againstHull;
    public float againstArmor;
    public float againstShield;
    public bool bypassHull;
    public bool bypassArmor;
    public bool bypassShield;

    public DamageProfileStruct(float value, float againstHull = 1.0f, float againstArmor = 1.0f, float againstShield = 1.0f, bool bypassHull = false, bool bypassArmor = false, bool bypassShield = false) {
        this.value = value;
        this.againstHull = againstHull;
        this.againstArmor = againstArmor;
        this.againstShield = againstShield;
        this.bypassHull = bypassHull;
        this.bypassArmor = bypassArmor;
        this.bypassShield = bypassShield;
    }

    public DamageProfileStruct(DamageProfile damageProfile) {
        this.value = damageProfile.value;
        this.againstHull = damageProfile.againstHull;
        this.againstArmor = damageProfile.againstArmor;
        this.againstShield = damageProfile.againstShield;
        this.bypassHull = damageProfile.bypassHull;
        this.bypassArmor = damageProfile.bypassArmor;
        this.bypassShield = damageProfile.bypassShield;
    }
    
    public DamageProfileStruct(DamageProfile damageProfile, float valueChange, bool multiply = false) {
        if(multiply) this.value = damageProfile.value * valueChange;
        else this.value = damageProfile.value + valueChange;
        this.againstHull = damageProfile.againstHull;
        this.againstArmor = damageProfile.againstArmor;
        this.againstShield = damageProfile.againstShield;
        this.bypassHull = damageProfile.bypassHull;
        this.bypassArmor = damageProfile.bypassArmor;
        this.bypassShield = damageProfile.bypassShield;
    }

    public DamageProfileStruct(DamageProfileStruct damageProfileStruct, float valueChange, bool multiply = false) {
        if(multiply) this.value = damageProfileStruct.value * valueChange;
        else this.value = damageProfileStruct.value + valueChange;
        this.againstHull = damageProfileStruct.againstHull;
        this.againstArmor = damageProfileStruct.againstArmor;
        this.againstShield = damageProfileStruct.againstShield;
        this.bypassHull = damageProfileStruct.bypassHull;
        this.bypassArmor = damageProfileStruct.bypassArmor;
        this.bypassShield = damageProfileStruct.bypassShield;
    }
}
