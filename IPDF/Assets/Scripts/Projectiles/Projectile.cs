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
    public Ticker ticker;

    public virtual void Initialize () {
        factionsManager = FactionsManager.GetInstance ();
        ticker = Ticker.GetInstance ();
        ticker.projectiles.Add (this);
        initialized = true;
    }

    public virtual void Process (float deltaTime) {}

    public virtual void Enable () {
        disabled = false;
    }

    protected virtual void Disable () {
        disabled = true;
    }
}