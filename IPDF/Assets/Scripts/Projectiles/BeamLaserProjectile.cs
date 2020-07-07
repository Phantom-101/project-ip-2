using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BeamLaserProjectile : Projectile {
    [Header ("Components")]
    public GameObject beam;

    public override void Initialize () {
        base.Initialize ();
        turret = handler.turret;
        beam = Instantiate ((turret as BeamLaserTurret).asset, transform) as GameObject;
        for (int i = 0; i < 4; i++) beam.transform.GetChild (i).GetComponent<MaterialColor> ().color = (turret as BeamLaserTurret).beamColor;
        transform.parent = from.transform.parent;
        factionsManager.ChangeRelationsWithAcquiredModification (to.factionID, from.factionID, -turret.damage);
    }

    public override void Process (float deltaTime) {
        if (!initialized || disabled) return;
        if (turret != handler.turret) { Disable (); return; }
        if (!handler.activated) { Disable (); return; }
        if (from == null || to == null) { Disable (); return; }
        if (!turret.CanSustain (handler, to.gameObject)) { Disable (); return; }
        Vector3 beamFrom = from.transform.localPosition + from.transform.rotation * handler.position;
        transform.localPosition = beamFrom;
        RaycastHit hit;
        if (Physics.Raycast (beamFrom, to.transform.localPosition - beamFrom, out hit, turret.range)) {
            transform.localRotation = Quaternion.LookRotation (to.transform.localPosition - beamFrom);
            beam.transform.localScale = new Vector3 ((turret as BeamLaserTurret).beamWidth, (turret as BeamLaserTurret).beamWidth, hit.distance);
            StructureBehaviours hitStructure = hit.transform.GetComponent<StructureBehaviours> ();
            if (hitStructure != null && hitStructure != from) {
                hitStructure.TakeDamage (turret.damage * deltaTime, beamFrom);
            }
        }
    }

    protected override void Disable () {
        Destroy (gameObject);
        base.Disable ();
    }
}