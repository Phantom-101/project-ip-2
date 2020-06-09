using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public Turret turret;
    public Ammunition ammunition;
    public GameObject activator;
    public GameObject target;
    public float storedEnergyRatio;
    public int fromFaction;
    public Vector3 endPosition;

    FactionsManager factionsManager;
    bool initialized = false;
    bool disabled = false;
    float waitDestroy = 0.0f;
    float waitDestroyTimer = 0.0f;
    float fuelExpended = 0.0f;
    float timeElapsed = 0.0f;

    public void Initialize (Turret turret, Ammunition ammunition, GameObject activator, GameObject target, float storedEnergyRatio, int fromFaction) {
        factionsManager = FindObjectOfType<FactionsManager> ();
        this.turret = turret;
        this.ammunition = ammunition;
        this.activator = activator;
        this.target = target;
        this.storedEnergyRatio = storedEnergyRatio;
        this.fromFaction = fromFaction;
        disabled = false;
        waitDestroy = 0.0f;
        waitDestroyTimer = 0.0f;
        if (ammunition == null) {
            GetComponent<TrailRenderer> ().colorGradient = turret.trailGradient;
            GetComponent<TrailRenderer> ().time = turret.trailTime;
        } else {
            GetComponent<TrailRenderer> ().colorGradient = ammunition.trailGradient;
            GetComponent<TrailRenderer> ().time = ammunition.trailTime;
        }
        initialized = true;
    }

    void Update () {
        if (!initialized) return;
        if (disabled) {
            waitDestroyTimer += Time.deltaTime;
            if (waitDestroyTimer >= waitDestroy) Destroy (gameObject);
            return;
        }
        timeElapsed += Time.deltaTime;
        if (ammunition == null) {
            if (target == null && turret.projectileSticky) Disable ();
            if (turret.projectileSticky && target != null && timeElapsed >= turret.trackingTime) endPosition = target.transform.position;
            else endPosition = transform.position + transform.forward;
            float step = turret.projectileVelocity * Time.deltaTime;
            RaycastHit hit;
            if (Physics.Raycast (transform.position, transform.forward, out hit, step)) {
                GameObject hitGameObject = hit.transform.gameObject;
                StructureBehaviours hitStructureBehaviours = hitGameObject.GetComponent<StructureBehaviours> ();
                if (hitStructureBehaviours != null) {
                    if (!(hitStructureBehaviours.factionID == fromFaction)) {
                        factionsManager.ChangeRelations (hitStructureBehaviours.factionID, fromFaction, -0.1f);
                        hitStructureBehaviours.TakeDamage (turret.damage * storedEnergyRatio, transform.position);
                        for (int i = 0; i < turret.explosionDetail; i++) {
                            float angle = 360.0f / turret.explosionDetail * i;
                            Vector3 dir = new Vector3 (Mathf.Sin (angle * Mathf.Deg2Rad), 0.0f, Mathf.Cos (angle * Mathf.Deg2Rad));
                            RaycastHit explosionHit;
                            if (Physics.Raycast (transform.position, transform.rotation * dir, out explosionHit, turret.explosionRange)) {
                                GameObject explosionHitGameObject = explosionHit.transform.gameObject;
                                StructureBehaviours explosionHitStructureBehaviours = explosionHitGameObject.GetComponent<StructureBehaviours> ();
                                if (explosionHitStructureBehaviours != null) {
                                    factionsManager.ChangeRelations (explosionHitStructureBehaviours.factionID, fromFaction, -0.05f);
                                    explosionHitStructureBehaviours.TakeDamage (turret.explosiveDamage / turret.explosionDetail * storedEnergyRatio, transform.position);
                                }
                            }
                        }
                        transform.position = hit.point;
                        Disable ();
                    }
                }
            }
            if (fuelExpended < turret.fuelRange && !disabled) {
                Vector3 heading = endPosition - transform.position;
                Vector3 newDirection = Vector3.RotateTowards (transform.forward, heading, turret.projectileTracking * Mathf.Deg2Rad * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation (newDirection);
                transform.Translate (new Vector3 (0.0f, 0.0f, step));
                fuelExpended += step;
            } else Disable ();
        } else {
            if (target == null && ammunition.projectileSticky) Disable ();
            if (ammunition.projectileSticky && target != null && timeElapsed >= turret.trackingTime) endPosition = target.transform.position;
            else endPosition = transform.position + transform.forward;
            float step = ammunition.projectileVelocity * Time.deltaTime;
            RaycastHit hit;
            if (Physics.Raycast (transform.position, transform.forward, out hit, step)) {
                GameObject hitGameObject = hit.transform.gameObject;
                StructureBehaviours hitStructureBehaviours = hitGameObject.GetComponent<StructureBehaviours> ();
                if (hitStructureBehaviours != null) {
                    if (!(hitStructureBehaviours.factionID == fromFaction)) {
                        factionsManager.ChangeRelations (hitStructureBehaviours.factionID, fromFaction, -0.1f);
                        hitStructureBehaviours.TakeDamage (ammunition.damage * storedEnergyRatio, transform.position);
                        for (int i = 0; i < ammunition.explosionDetail; i++) {
                            float angle = 360.0f / ammunition.explosionDetail * i;
                            Vector3 dir = new Vector3 (Mathf.Sin (angle * Mathf.Deg2Rad), 0.0f, Mathf.Cos (angle * Mathf.Deg2Rad));
                            RaycastHit explosionHit;
                            if (Physics.Raycast (transform.position, transform.rotation * dir, out explosionHit, ammunition.explosionRange)) {
                                GameObject explosionHitGameObject = explosionHit.transform.gameObject;
                                StructureBehaviours explosionHitStructureBehaviours = explosionHitGameObject.GetComponent<StructureBehaviours> ();
                                if (explosionHitStructureBehaviours != null) {
                                    factionsManager.ChangeRelations (explosionHitStructureBehaviours.factionID, fromFaction, -0.05f);
                                    explosionHitStructureBehaviours.TakeDamage (ammunition.explosiveDamage / ammunition.explosionDetail * storedEnergyRatio, transform.position);
                                }
                            }
                        }
                        transform.position = hit.point;
                        Disable ();
                    }
                }
            }
            if (fuelExpended < ammunition.fuelRange && !disabled) {
                Vector3 heading = endPosition - transform.position;
                Vector3 newDirection = Vector3.RotateTowards (transform.forward, heading, ammunition.projectileTracking * Mathf.Deg2Rad * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation (newDirection);
                transform.Translate (new Vector3 (0.0f, 0.0f, step));
                fuelExpended += step;
            } else Disable ();
        }
    }

    void Disable () {
        disabled = true;
        if (ammunition == null) {
            waitDestroy = turret.trailTime;
            if (turret.explosion != null) {
                GameObject explosion = Instantiate (turret.explosion, transform.position, Quaternion.identity) as GameObject;
                explosion.transform.localScale = Vector3.one * turret.explosionSize;
            }
        } else {
            waitDestroy = ammunition.trailTime;
            if (ammunition.explosion != null) {
                GameObject explosion = Instantiate (ammunition.explosion, transform.position, Quaternion.identity) as GameObject;
                explosion.transform.localScale = Vector3.one * ammunition.explosionSize;
            }
        }
    }
}