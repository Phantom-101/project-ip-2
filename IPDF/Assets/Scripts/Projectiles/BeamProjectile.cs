using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BeamProjectile : Projectile {
    [Header ("Components")]
    public VisualEffect beam;

    public override void Initialize () {
        base.Initialize ();
        turret = handler.turret;
        beam = gameObject.AddComponent<VisualEffect> ();
        beam.visualEffectAsset = (turret as BeamTurret).asset;
        beam.SetGradient ("gradient", (turret as BeamTurret).beamGradient);
        beam.SetFloat ("size", (turret as BeamTurret).beamWidth);
        beam.SetFloat ("lifetime", 0.1f);
        transform.parent = from.transform.parent;
    }

    protected override void Process () {
        if (turret != handler.turret) Disable ();
        if (!handler.activated) Disable ();
        if (!turret.CanFire (handler, to.gameObject)) Disable ();
        if (from == null || to == null) Disable ();
        Vector3 beamFrom = from.transform.position + from.transform.rotation * handler.position;
        transform.localPosition = beamFrom;
        RaycastHit hit;
        if (Physics.Raycast (beamFrom, to.transform.position - beamFrom, out hit, turret.range)) {
            transform.localRotation = Quaternion.LookRotation (to.transform.position - beamFrom);
            beam.SetFloat ("forward", hit.distance);
            beam.SetFloat ("count", hit.distance * 1000 / (turret as BeamTurret).beamWidth);
            StructureBehaviours hitStructure = hit.transform.GetComponent<StructureBehaviours> ();
            if (hitStructure != null && hitStructure != from) hitStructure.TakeDamage (turret.damage * Time.deltaTime, beamFrom);
        }
    }

    protected override void Disable () {
        Destroy (gameObject);
        base.Disable ();
    }
}