using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ammunition", menuName = "ScriptableObjects/Ammunition")]
public class Ammunition : Item
{
    public float damage;
    public float speed;
    public bool tracking;
}
