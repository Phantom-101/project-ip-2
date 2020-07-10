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
        GameObject soundEffect = new GameObject ("Beam Laser Sound Effect");
        soundEffect.transform.parent = from.transform.parent;
        soundEffect.transform.localPosition = from.transform.localPosition;
        AudioSource audioSource = soundEffect.AddComponent<AudioSource> ();
        audioSource.spatialBlend = 1;
        audioSource.minDistance = turret.audioDistance;
        audioSource.PlayOneShot (turret.clip, 1);
        Destroy (soundEffect, 5);
        beam = Instantiate ((turret as PulseLaserTurret).asset, transform) as GameObject;
        beam.transform.localPosition = Vector3.zero;
        for (int i = 0; i < 8; i++) beam.transform.GetChild (i).GetComponent<MaterialColor> ().color = (turret as PulseLaserTurret).beamColor;
        transform.parent = from.transform.parent;
        Vector3 beamFrom = from.transform.localPosition + from.transform.rotation * handler.position;
        transform.localPosition = beamFrom;
        RaycastHit hit;
        if (Physics.Raycast (beamFrom, to.transform.localPosition - beamFrom, out hit, turret.range)) {
            transform.localRotation = Quaternion.LookRotation (to.transform.localPosition - beamFrom);
            beam.transform.localScale = new Vector3 ((turret as PulseLaserTurret).beamWidth, (turret as PulseLaserTurret).beamWidth, hit.distance);
            StructureBehaviours hitStructure = hit.transform.GetComponent<StructureBehaviours> ();
            if (hitStructure != null && hitStructure != from) {
                hitStructure.TakeDamage ((turret as PulseLaserTurret).damage, beamFrom);
            }
        }
        factionsManager.ChangeRelationsWithAcquiredModification (to.factionID, from.factionID, -(turret as PulseLaserTurret).damage);
    }

    public override void Process (float deltaTime) {
        if (!initialized || disabled) return;
        lifetime += deltaTime;
        for (int i = 0; i < 8; i++) beam.transform.GetChild (i).GetComponent<MaterialColor> ().color = new Color (
            (turret as PulseLaserTurret).beamColor.r,
            (turret as PulseLaserTurret).beamColor.g,
            (turret as PulseLaserTurret).beamColor.b,
            (turret as PulseLaserTurret).beamColor.a * ((turret as PulseLaserTurret).beamDuration - lifetime) / (turret as PulseLaserTurret).beamDuration
        );
        if (lifetime > (turret as PulseLaserTurret).beamDuration) Disable ();
    }

    protected override void Disable () {
        base.Disable ();
        Destroy (gameObject);
    }
}