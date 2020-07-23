using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseProjectile : Projectile {
    [Header ("Stats")]
    public float lifetime = 0;
    [Header ("Components")]
    public GameObject beam;

    public override void Initialize () {
        if (initialized) return;
        base.Initialize ();
        turret = handler.turret;
        beam = Instantiate ((turret as PulseTurret).asset, transform) as GameObject;
    }

    public override void Process (float deltaTime) {
        if (!initialized || disabled) return;
        lifetime += deltaTime;
        beam.transform.localScale = new Vector3 (
            (turret as PulseTurret).beamWidth * ((turret as PulseTurret).beamDuration - lifetime) / (turret as PulseTurret).beamDuration,
            (turret as PulseTurret).beamWidth * ((turret as PulseTurret).beamDuration - lifetime) / (turret as PulseTurret).beamDuration,
            beam.transform.localScale.z
        );
        if (lifetime > (turret as PulseTurret).beamDuration) Disable ();
    }

    public override void Enable () {
        base.Enable ();
        beam.SetActive (true);
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
        beam.transform.localPosition = Vector3.zero;
        transform.parent = from.transform.parent;
        Vector3 beamFrom = from.transform.localPosition + from.transform.rotation * handler.position;
        beam.transform.localScale = new Vector3 ((turret as PulseTurret).beamWidth, (turret as PulseTurret).beamWidth, Vector3.Distance (beamFrom, to.transform.position));
        transform.localPosition = beamFrom;
        transform.localRotation = Quaternion.LookRotation (to.transform.localPosition - beamFrom);
        to.TakeDamage ((turret as PulseTurret).damage, beamFrom);
        factionsManager.ChangeRelationsWithAcquiredModification (to.faction, from.faction, -(turret as PulseTurret).damage / 10);
        lifetime = 0;
    }

    protected override void Disable () {
        base.Disable ();
        beam.SetActive (false);
    }
}