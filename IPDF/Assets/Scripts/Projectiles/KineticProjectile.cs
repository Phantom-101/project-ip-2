using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class KineticProjectile : Projectile {
    [Header ("Stats")]
    public float speed;
    public float damage;
    [Header ("Components")]
    public GameObject mesh;

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
        mesh = Instantiate (handler.ammunition.asset, transform) as GameObject;
        mesh.transform.localPosition = Vector3.zero;
        transform.parent = from.transform.parent;
        Vector3 origin = from.transform.localPosition + from.transform.rotation * handler.position;
        transform.localPosition = origin;
        speed = (turret as KineticTurret).power / handler.ammunition.mass;
        transform.localRotation = Quaternion.LookRotation (CalculateLeadPosition (transform.localPosition, to.transform.localPosition, to.GetComponent<Rigidbody> ().velocity / 60, speed));
        damage = speed * (turret as KineticTurret).damageMultiplier;
        factionsManager.ChangeRelationsWithAcquiredModification (to.factionID, from.factionID, -damage);
    }

    public override void Process (float deltaTime) {
        if (!initialized || disabled) return;
        if (from == null || to == null) { Disable (); return; }
        if (turret != handler.turret) { Disable (); return; }
        Collider[] overlaps = Physics.OverlapSphere (transform.position, speed);
        foreach (Collider hit in overlaps) {
            StructureBehaviours hitStructure = hit.transform.parent.GetComponent<StructureBehaviours> ();
            if (hitStructure != null && hitStructure != from) {
                hitStructure.TakeDamage (damage, transform.localPosition);
                Disable ();
            }
        }
        transform.Translate (new Vector3 (0, 0, speed));
    }

    protected override void Disable () {
        base.Disable ();
        Destroy (gameObject);
    }

    Vector3 CalculateLeadPosition (Vector3 currentPosition, Vector3 targetPosition, Vector3 targetVelocity, float projectileVelocity, bool lead = true) {
        if (!lead) return targetPosition - currentPosition;
        float distance = Vector3.Distance (currentPosition, targetPosition);
        float travelTime = distance / projectileVelocity;
        return targetPosition + targetVelocity * travelTime - currentPosition;
    }
}