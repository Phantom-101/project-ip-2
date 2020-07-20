using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamProjectile : Projectile {
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
        beam = Instantiate ((turret as BeamTurret).asset, transform) as GameObject;
        for (int i = 0; i < 8; i++) beam.transform.GetChild (i).GetComponent<MaterialColor> ().color = (turret as BeamTurret).beamColor;
        transform.parent = from.transform.parent;
        factionsManager.ChangeRelationsWithAcquiredModification (to.faction, from.faction, -(turret as BeamTurret).damage);
    }

    public override void Process (float deltaTime) {
        if (!initialized || disabled) return;
        if (from == null || to == null) { Disable (); return; }
        if (turret != handler.turret) { Disable (); return; }
        if (!handler.activated) { Disable (); return; }
        if (!turret.CanSustain (handler, to.gameObject)) { Disable (); return; }
        Vector3 beamFrom = from.transform.localPosition + from.transform.rotation * handler.position;
        transform.localPosition = beamFrom;
        RaycastHit hit; 
        if (Physics.Raycast (beamFrom, to.transform.localPosition - beamFrom, out hit, turret.range)) {
            transform.localRotation = Quaternion.LookRotation (to.transform.localPosition - beamFrom);
            beam.transform.localScale = new Vector3 ((turret as BeamTurret).beamWidth, (turret as BeamTurret).beamWidth, hit.distance);
            StructureBehaviours hitStructure = hit.transform.GetComponent<StructureBehaviours> ();
            if (hitStructure != null && hitStructure != from) {
                hitStructure.TakeDamage ((turret as BeamTurret).damage * deltaTime, beamFrom);
            }
        }
    }

    protected override void Disable () {
        base.Disable ();
        Destroy (gameObject);
    }
}