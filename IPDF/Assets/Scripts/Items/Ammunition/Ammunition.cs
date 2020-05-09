using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Ammunition", menuName = "Equipment/Ammunition")]
public class Ammunition : Item {
    public float projectileVelocity;
    public bool projectileSticky;
    public float projectileInaccuracy;
    public bool leadProjectile;
    public float damage;
    public float range;
    public float maxStoredEnergy;
    public float rechargeRate;
    public float activationThreshold;
}