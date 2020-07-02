using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PulseLaserProjectile : Projectile {
    [Header ("Stats")]
    public float lifetime = 0;
    [Header ("Components")]
    public GameObject beam;

    public override void Initialize () {
        base.Initialize ();
        turret = handler.turret;
        beam = Instantiate ((turret as PulseLaserTurret).asset, transform) as GameObject;
        beam.transform.localPosition = Vector3.zero;
        for (int i = 0; i < 8; i++) beam.transform.GetChild (i).GetComponent<MaterialColor> ().color = (turret as PulseLaserTurret).beamColor;
        transform.parent = from.transform.parent;
        Vector3 beamFrom = from.transform.position + from.transform.rotation * handler.position;
        transform.localPosition = beamFrom;
        RaycastHit hit;
        if (Physics.Raycast (beamFrom, to.transform.position - beamFrom, out hit, turret.range)) {
            transform.localRotation = Quaternion.LookRotation (to.transform.localPosition - beamFrom);
            beam.transform.localScale = new Vector3 ((turret as PulseLaserTurret).beamWidth, (turret as PulseLaserTurret).beamWidth, hit.distance);
            StructureBehaviours hitStructure = hit.transform.GetComponent<StructureBehaviours> ();
            if (hitStructure != null && hitStructure != from) {
                hitStructure.TakeDamage (turret.damage, beamFrom);
                Disable ();
            }
        }
        factionsManager.ChangeRelationsWithAcquiredModification (to.factionID, from.factionID, -turret.damage);
    }

    void Update () {
        lifetime += Time.deltaTime;
        for (int i = 0; i < 8; i++) beam.transform.GetChild (i).GetComponent<MaterialColor> ().color = new Color (
            (turret as PulseLaserTurret).beamColor.r,
            (turret as PulseLaserTurret).beamColor.g,
            (turret as PulseLaserTurret).beamColor.b,
            (turret as PulseLaserTurret).beamColor.a * ((turret as PulseLaserTurret).beamDuration - lifetime) / (turret as PulseLaserTurret).beamDuration
        );
    }

    protected override void Disable () {
        Destroy (gameObject, (turret as PulseLaserTurret).beamDuration);
        base.Disable ();
    }
}