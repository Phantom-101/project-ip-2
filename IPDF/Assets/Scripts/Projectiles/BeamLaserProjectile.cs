using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BeamLaserProjectile : Projectile {
    [Header ("Components")]
    public VisualEffect beam;

    public override void Initialize () {
        base.Initialize ();
        turret = handler.turret;
        beam = gameObject.AddComponent<VisualEffect> ();
        beam.visualEffectAsset = (turret as BeamLaserTurret).asset;
        beam.SetGradient ("gradient", (turret as BeamLaserTurret).beamGradient);
        beam.SetFloat ("size", (turret as BeamLaserTurret).beamWidth);
        beam.SetFloat ("lifetime", 0.1f);
        beam.SetFloat ("count", 40);
        transform.parent = from.transform.parent;
    }

    protected override void Process () {
        if (turret != handler.turret) { Disable (); return; }
        if (!handler.activated) { Disable (); return; }
        if (from == null || to == null) { Disable (); return; }
        if (!turret.CanSustain (handler, to.gameObject)) { Disable (); return; }
        Vector3 beamFrom = from.transform.position + from.transform.rotation * handler.position;
        transform.localPosition = beamFrom;
        RaycastHit hit;
        if (Physics.Raycast (beamFrom, to.transform.position - beamFrom, out hit, turret.range)) {
            transform.localRotation = Quaternion.LookRotation (to.transform.position - beamFrom);
            beam.SetFloat ("forward", hit.distance);
            StructureBehaviours hitStructure = hit.transform.GetComponent<StructureBehaviours> ();
            if (hitStructure != null && hitStructure != from) {
                hitStructure.TakeDamage (turret.damage * Time.deltaTime, beamFrom);
                factionsManager.ChangeRelations (hitStructure.factionID, from.factionID, -turret.damage * Time.deltaTime);
            }
        }
    }

    protected override void Disable () {
        Destroy (gameObject);
        base.Disable ();
    }
}