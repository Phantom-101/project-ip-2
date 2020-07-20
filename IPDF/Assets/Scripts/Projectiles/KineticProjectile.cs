using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KineticProjectile : Projectile {
    [Header ("Stats")]
    public Ammunition ammunition;
    public Vector3 origin;
    public float speed;
    public float damage;
    [Header ("Components")]
    public GameObject mesh;

    public override void Initialize () {
        if (from == null || to == null) { Disable (); return; }
        base.Initialize ();
        turret = handler.turret;
        ammunition = handler.ammunition;
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
        mesh = Instantiate (ammunition.asset, transform) as GameObject;
        mesh.transform.localPosition = Vector3.zero;
        transform.parent = from.transform.parent;
        origin = from.transform.localPosition + from.transform.rotation * handler.position;
        transform.localPosition = origin;
        speed = (turret as KineticTurret).power / ammunition.mass;
        transform.localRotation = Quaternion.LookRotation (CalculateLeadPosition (
            transform.localPosition,
            to.transform.localPosition,
            to.GetComponent<Rigidbody> ().velocity,
            speed
        ));
        damage = ammunition.damage * (turret as KineticTurret).power;
        factionsManager.ChangeRelationsWithAcquiredModification (to.faction, from.faction, -damage);
    }

    public override void Process (float deltaTime) {
        if (!initialized || disabled) return;
        if ((origin - transform.localPosition).sqrMagnitude > ammunition.range * ammunition.range) { Disable(); return; }
        if (from == null || to == null) { Disable (); return; }
        Collider[] overlaps = Physics.OverlapSphere (transform.position, speed * deltaTime);
        foreach (Collider hit in overlaps) {
            StructureBehaviours hitStructure = hit.transform.parent.GetComponent<StructureBehaviours> ();
            if (hitStructure != null && hitStructure != from) {
                hitStructure.TakeDamage (damage, transform.localPosition);
                Disable ();
            }
        }
        transform.Translate (new Vector3 (0, 0, speed * deltaTime));
    }

    protected override void Disable () {
        base.Disable ();
        Destroy (gameObject, 1);
    }

    Vector3 CalculateLeadPosition (Vector3 currentPosition, Vector3 targetPosition, Vector3 targetVelocity, float projectileVelocity, bool lead = true) {
        if (!lead) return targetPosition - currentPosition;
        float distance = Vector3.Distance (currentPosition, targetPosition);
        float travelTime = distance / projectileVelocity;
        return targetPosition + targetVelocity * travelTime - currentPosition;
    }
}