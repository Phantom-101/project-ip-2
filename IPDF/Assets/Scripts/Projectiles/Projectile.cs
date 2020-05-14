using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public Turret turret;
    public Ammunition ammunition;
    public GameObject activator;
    public GameObject target;
    public float storedEnergyRatio;
    public Vector3 endPosition;

    bool initialized = false;
    bool disabled = false;
    float waitDestroy = 0.0f;
    float waitDestroyTimer = 0.0f;
    float fuelExpended = 0.0f;

    public void Initialize (Turret turret, Ammunition ammunition, GameObject activator, GameObject target, float storedEnergyRatio) {
        this.turret = turret;
        this.ammunition = ammunition;
        this.activator = activator;
        this.target = target;
        this.storedEnergyRatio = storedEnergyRatio;
        initialized = true;
        disabled = false;
        waitDestroy = 0.0f;
        waitDestroyTimer = 0.0f;
        if (ammunition == null) {
            GetComponent<TrailRenderer> ().startColor = turret.trailColor;
            GetComponent<TrailRenderer> ().endColor = turret.trailColor;
        } else {
            GetComponent<TrailRenderer> ().startColor = ammunition.trailColor;
            GetComponent<TrailRenderer> ().endColor = ammunition.trailColor;
        }
    }

    void Update () {
        if (!initialized) return;
        if (disabled) {
            waitDestroyTimer += Time.deltaTime;
            if (waitDestroyTimer >= waitDestroy) Destroy (gameObject);
            return;
        }
        if (ammunition == null) {
            if (turret.projectileSticky && target != null) endPosition = target.transform.position;
            float step = turret.projectileVelocity * Time.deltaTime;
            if (fuelExpended < turret.fuelRange) {
                Vector3 heading = endPosition - transform.position;
                Vector3 newDirection = Vector3.RotateTowards (transform.forward, heading, turret.projectileTracking * Mathf.Deg2Rad * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation (newDirection);
                transform.Translate (new Vector3 (0.0f, 0.0f, step));
                fuelExpended += step;
            } else Disable ();
            RaycastHit hit;
            if (Physics.Raycast (transform.position, transform.forward, out hit, step)) {
                Debug.DrawRay (transform.position, transform.rotation * new Vector3 (0.0f, 0.0f, step), Color.green);
                GameObject hitGameObject = hit.transform.gameObject;
                StructureBehaviours hitStructureBehaviours = hitGameObject.GetComponent<StructureBehaviours> ();
                if (hitStructureBehaviours != null) {
                    hitStructureBehaviours.TakeDamage (turret.damage * storedEnergyRatio, transform.position);
                    for (int i = 0; i < turret.explosionDetail; i++) {
                        float angle = 360.0f / turret.explosionDetail * i;
                        Vector3 dir = new Vector3 (Mathf.Sin (angle * Mathf.Deg2Rad), 0.0f, Mathf.Cos (angle * Mathf.Deg2Rad));
                        RaycastHit explosionHit;
                        if (Physics.Raycast (transform.position, transform.rotation * dir, out explosionHit, turret.explosionRange)) {
                            GameObject explosionHitGameObject = explosionHit.transform.gameObject;
                            StructureBehaviours explosionHitStructureBehaviours = explosionHitGameObject.GetComponent<StructureBehaviours> ();
                            if (explosionHitStructureBehaviours != null) {
                                Debug.DrawRay (transform.position, transform.rotation * (dir.normalized * turret.explosionRange), Color.green, 1.0f);
                                Debug.Log (turret.name + "'s explosion hit " + explosionHitGameObject.name + " and dealt " +
                                    (turret.explosiveDamage / turret.explosionDetail * storedEnergyRatio) + " points of damage.");
                                explosionHitStructureBehaviours.TakeDamage (turret.explosiveDamage / turret.explosionDetail * storedEnergyRatio, transform.position);
                            } else Debug.DrawRay (transform.position, transform.rotation * (dir.normalized * turret.explosionRange), Color.red, 1.0f);
                        } else Debug.DrawRay (transform.position, transform.rotation * (dir.normalized * turret.explosionRange), Color.red, 1.0f);
                    }
                    transform.position = hit.point;
                    Disable ();
                } else Debug.DrawRay (transform.position, transform.rotation * new Vector3 (0.0f, 0.0f, step), Color.red);
            } else Debug.DrawRay (transform.position, transform.rotation * new Vector3 (0.0f, 0.0f, step), Color.red);
        } else {
            Debug.Log ("This projectile is using an ammunition.");
            if (ammunition.projectileSticky && target != null) endPosition = target.transform.position;
            float step = ammunition.projectileVelocity * Time.deltaTime;
            if (fuelExpended < ammunition.fuelRange) {
                Vector3 heading = endPosition - transform.position;
                Vector3 newDirection = Vector3.RotateTowards (transform.forward, heading, ammunition.projectileTracking * Mathf.Deg2Rad * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation (newDirection);
                transform.Translate (new Vector3 (0.0f, 0.0f, step));
                fuelExpended += step;
            } else Disable ();
            RaycastHit hit;
            if (Physics.Raycast (transform.position, transform.forward, out hit, step)) {
                Debug.DrawRay (transform.position, transform.rotation * new Vector3 (0.0f, 0.0f, step), Color.green);
                GameObject hitGameObject = hit.transform.gameObject;
                StructureBehaviours hitStructureBehaviours = hitGameObject.GetComponent<StructureBehaviours> ();
                if (hitStructureBehaviours != null) {
                    hitStructureBehaviours.TakeDamage (ammunition.damage * storedEnergyRatio, transform.position);
                    for (int i = 0; i < ammunition.explosionDetail; i++) {
                        float angle = 360.0f / ammunition.explosionDetail * i;
                        Vector3 dir = new Vector3 (Mathf.Sin (angle * Mathf.Deg2Rad), 0.0f, Mathf.Cos (angle * Mathf.Deg2Rad));
                        RaycastHit explosionHit;
                        if (Physics.Raycast (transform.position, transform.rotation * dir, out explosionHit, ammunition.explosionRange)) {
                            GameObject explosionHitGameObject = explosionHit.transform.gameObject;
                            StructureBehaviours explosionHitStructureBehaviours = explosionHitGameObject.GetComponent<StructureBehaviours> ();
                            if (explosionHitStructureBehaviours != null) {
                                Debug.DrawRay (transform.position, transform.rotation * (dir.normalized * ammunition.explosionRange), Color.green, 1.0f);
                                Debug.Log (ammunition.name + "'s explosion hit " + explosionHitGameObject.name + " and dealt " +
                                    (ammunition.explosiveDamage / ammunition.explosionDetail * storedEnergyRatio) + " points of damage.");
                                explosionHitStructureBehaviours.TakeDamage (ammunition.explosiveDamage / ammunition.explosionDetail * storedEnergyRatio, transform.position);
                            } else Debug.DrawRay (transform.position, transform.rotation * (dir.normalized * ammunition.explosionRange), Color.red, 1.0f);
                        } else Debug.DrawRay (transform.position, transform.rotation * (dir.normalized * ammunition.explosionRange), Color.red, 1.0f);
                    }
                    transform.position = hit.point;
                    Disable ();
                } else Debug.DrawRay (transform.position, transform.rotation * new Vector3 (0.0f, 0.0f, step), Color.red);
            } else Debug.DrawRay (transform.position, transform.rotation * new Vector3 (0.0f, 0.0f, step), Color.red);
        }
    }

    void Disable () {
        disabled = true;
        TrailRenderer trail = GetComponent<TrailRenderer> ();
        if (trail != null) waitDestroy = trail.time;
    }
}