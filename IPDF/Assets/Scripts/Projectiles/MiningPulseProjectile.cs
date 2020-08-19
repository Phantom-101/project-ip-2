using UnityEngine;

public class MiningPulseProjectile : PulseProjectile {
    public override void Enable () {
        base.Enable ();
        gameObject.SetActive (true);
        if (turret != handler.turret) {
            turret = handler.turret;
            if (beam != null) Destroy (beam);
            beam = Instantiate ((turret as MiningPulseTurret).asset, transform);
        }
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
        beam.transform.localScale = new Vector3 ((turret as MiningPulseTurret).beamWidth, (turret as MiningPulseTurret).beamWidth, Vector3.Distance (beamFrom, to.transform.position));
        transform.localPosition = beamFrom;
        transform.localRotation = Quaternion.LookRotation (to.transform.localPosition - beamFrom);
        if (to.profile.structureClass == StructureClass.Asteroid) {
            to.TakeDamage ((turret as MiningPulseTurret).damage * (turret as MiningPulseTurret).damageToAsteroidsMultiplier, beamFrom);
        } else {
            to.TakeDamage ((turret as MiningPulseTurret).damage, beamFrom);
            factionsManager.ChangeRelationsWithAcquiredModification (to.faction, from.faction, -(turret as MiningPulseTurret).damage / 10);
        }
        lifetime = 0;
    }
}