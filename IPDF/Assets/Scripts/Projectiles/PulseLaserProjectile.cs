using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PulseLaserProjectile : Projectile {
    [Header ("Components")]
    public VisualEffect beam;

    public override void Initialize () {
        base.Initialize ();
        turret = handler.turret;
        beam = gameObject.AddComponent<VisualEffect> ();
        beam.visualEffectAsset = (turret as PulseLaserTurret).asset;
        beam.SetGradient ("gradient", (turret as PulseLaserTurret).beamGradient);
        beam.SetFloat ("size", (turret as PulseLaserTurret).beamWidth);
        beam.SetFloat ("lifetime", (turret as PulseLaserTurret).beamDuration);
        transform.parent = from.transform.parent;
        Vector3 beamFrom = from.transform.position + from.transform.rotation * handler.position;
        transform.localPosition = beamFrom;
        RaycastHit hit;
        if (Physics.Raycast (beamFrom, to.transform.position - beamFrom, out hit, turret.range)) {
            transform.localRotation = Quaternion.LookRotation (to.transform.position - beamFrom);
            beam.SetFloat ("forward", hit.distance);
            beam.SetFloat ("count", hit.distance * 2000 / (turret as PulseLaserTurret).beamWidth);
            StructureBehaviours hitStructure = hit.transform.GetComponent<StructureBehaviours> ();
            if (hitStructure != null && hitStructure != from) {
                hitStructure.TakeDamage (turret.damage, beamFrom);
                Disable ();
            }
        }
        factionsManager.ChangeRelationsWithAcquiredModification (to.factionID, from.factionID, -turret.damage);
    }

    protected override void Disable () {
        Destroy (gameObject, (turret as PulseLaserTurret).beamDuration);
        base.Disable ();
    }
}