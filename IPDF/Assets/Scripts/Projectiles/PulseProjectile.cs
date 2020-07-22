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
        transform.parent = from.transform.parent;
        Vector3 beamFrom = from.transform.localPosition + from.transform.rotation * handler.position;
        beam.transform.localScale = new Vector3 ((turret as PulseTurret).beamWidth, (turret as PulseTurret).beamWidth, Vector3.Distance (beamFrom, to.transform.position));
        transform.localPosition = beamFrom;
        transform.localRotation = Quaternion.LookRotation (to.transform.localPosition - beamFrom);
        to.TakeDamage ((turret as PulseTurret).damage, beamFrom);
        factionsManager.ChangeRelationsWithAcquiredModification (to.faction, from.faction, -(turret as PulseTurret).damage / 10);
    }

    public override void Process (float deltaTime) {
        if (!initialized || disabled) return;
        lifetime += deltaTime;
        /*for (int i = 0; i < 4; i++) beam.transform.GetChild (i).GetComponent<MaterialColor> ().color = new Color (
            (turret as PulseTurret).beamColor.r,
            (turret as PulseTurret).beamColor.g,
            (turret as PulseTurret).beamColor.b,
            (turret as PulseTurret).beamColor.a * ((turret as PulseTurret).beamDuration - lifetime) / (turret as PulseTurret).beamDuration
        );*/
        beam.transform.localScale = new Vector3 (
            (turret as PulseTurret).beamWidth * ((turret as PulseTurret).beamDuration - lifetime) / (turret as PulseTurret).beamDuration,
            (turret as PulseTurret).beamWidth * ((turret as PulseTurret).beamDuration - lifetime) / (turret as PulseTurret).beamDuration,
            beam.transform.localScale.z
        );
        if (lifetime > (turret as PulseTurret).beamDuration) Disable ();
    }

    protected override void Disable () {
        base.Disable ();
        Destroy (gameObject);
    }
}