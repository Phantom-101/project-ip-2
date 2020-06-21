using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [Header ("Stats")]
    public TurretHandler handler;
    public Turret turret;
    public StructureBehaviours from;
    public StructureBehaviours to;
    public bool initialized;
    public bool disabled;
    [Header ("Components")]
    public FactionsManager factionsManager;

    public virtual void Initialize () {
        factionsManager = FindObjectOfType<FactionsManager> ();
        initialized = true;
    }

    void Update () {
        if (!initialized || disabled) return;
        Process ();
    }

    protected virtual void Process () {}

    protected virtual void Disable () {
        disabled = true;
    }
}