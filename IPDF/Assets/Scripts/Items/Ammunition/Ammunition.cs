using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Ammunition", menuName = "Ammunition")]
public class Ammunition : Item {
    [Header ("Appearance")]
    public GameObject asset;
    [Header ("Projectile Stats")]
    public float range;
    public float mass;
    public float damage;
}