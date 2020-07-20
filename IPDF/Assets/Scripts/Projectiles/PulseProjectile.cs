using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseProjectile : Projectile {
    [Header ("Stats")]
    public float lifetime = 0;
    [Header ("Components")]
    public GameObject beam;

    public override void Initialize () {
        base.Initialize ();
        turret = handler.turret;
        if (turret.audio != null) {
            GameObject soundEffect = new GameObject ("Sound Effect");
            soundEffect.transform.parent = from.transform.parent;
            soundEffect.transform.localPosition = from.transform.localPosition;
            AudioSource audioSource = soundEffect.AddComponent<AudioSource> ();
            audioSource.spatialBlend = turret.audio.spatialBlend;
            audioSource.minDistance = turret.audio.minDistance;
            audioSource.maxDistance = turret.audio.maxDistance;
            audioSource.rolloffMode = turret.audio.rolloffMode;
            audioSource.PlayOneShot (turret.audio.clip, turret.audio.volume);
            Destroy (soundEffect, 5);
        }
        beam = Instantiate ((turret as PulseTurret).asset, transform) as GameObject;
        beam.transform.localPosition = Vector3.zero;
        for (int i = 0; i < 8; i++) beam.transform.GetChild (i).GetComponent<MaterialColor> ().color = (turret as PulseTurret).beamColor;
        transform.parent = from.transform.parent;
        Vector3 beamFrom = from.transform.localPosition + from.transform.rotation * handler.position;
        transform.localPosition = beamFrom;
        RaycastHit hit;
        if (Physics.Raycast (beamFrom, to.transform.localPosition - beamFrom, out hit, turret.range)) {
            transform.localRotation = Quaternion.LookRotation (to.transform.localPosition - beamFrom);
            beam.transform.localScale = new Vector3 ((turret as PulseTurret).beamWidth, (turret as PulseTurret).beamWidth, hit.distance);
            StructureBehaviours hitStructure = hit.transform.GetComponent<StructureBehaviours> ();
            if (hitStructure != null && hitStructure != from) {
                hitStructure.TakeDamage ((turret as PulseTurret).damage, beamFrom);
            }
        }
        factionsManager.ChangeRelationsWithAcquiredModification (to.faction, from.faction, -(turret as PulseTurret).damage);
    }

    public override void Process (float deltaTime) {
        if (!initialized || disabled) return;
        lifetime += deltaTime;
        for (int i = 0; i < 8; i++) beam.transform.GetChild (i).GetComponent<MaterialColor> ().color = new Color (
            (turret as PulseTurret).beamColor.r,
            (turret as PulseTurret).beamColor.g,
            (turret as PulseTurret).beamColor.b,
            (turret as PulseTurret).beamColor.a * ((turret as PulseTurret).beamDuration - lifetime) / (turret as PulseTurret).beamDuration
        );
        if (lifetime > (turret as PulseTurret).beamDuration) Disable ();
    }

    protected override void Disable () {
        base.Disable ();
        Destroy (gameObject);
    }
}