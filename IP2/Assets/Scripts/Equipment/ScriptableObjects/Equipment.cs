using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "ScriptableObjects/Equipment")]
public class Equipment : Item {
    public float signature;
    public float hitpoints;
    public int meta;
    public bool activatable;
    public float cycleTime;
    public bool cycleInterruptable;
    public float[] activations;
    public AudioClip[] activationClips;
    public bool repeating;
    public bool mustBeTargeted;
    public float range;
    public string[] rangeStats;
    public string[] requirements;
    public string[] requirementStats;
    public float[] minValues;
    public float[] maxValues;
    public bool requireCharge;
    public Charge[] accepted;
    public bool showLoadedChargeIcon;
    public GameObject vfx;
    public EquipmentStatsModificationProfile passiveEffects;
    public EquipmentStatsModificationProfile activeEffects;
    public DamageZoneProfile damageZoneProfile;
}
