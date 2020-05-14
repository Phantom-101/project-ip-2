using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Ammunition", menuName = "Ammunition")]
public class Ammunition : Item {
    [Header ("Appearance")]
    public GameObject projectile;
    public Color trailColor;
    [Header ("Projectile Movement")]
    public float projectileVelocity;
    public bool projectileSticky;
    public float projectileTracking;
    [Header ("Projectile Initial Rotation")]
    public bool projectileInitializeRotation;
    public float projectileInaccuracy;
    public bool leadProjectile;
    [Header ("Projectile Stats")]
    public float fuelRange;
    public float damage;
    public float explosiveDamage;
    public float explosionRange;
    public int explosionDetail;
}