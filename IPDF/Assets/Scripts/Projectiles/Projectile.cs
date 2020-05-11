using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public Turret turret;
    public GameObject activator;
    public GameObject target;
    public float storedEnergyRatio;
    public Vector3 endPosition;

    bool initialized = false;
    bool disabled = false;
    float waitDestroy = 0.0f;
    float waitDestroyTimer = 0.0f;
    float fuelExpended = 0.0f;

    public void Initialize (Turret turret, GameObject activator, GameObject target, float storedEnergyRatio) {
        this.turret = turret;
        this.activator = activator;
        this.target = target;
        this.storedEnergyRatio = storedEnergyRatio;
        initialized = true;
        disabled = false;
        waitDestroy = 0.0f;
        waitDestroyTimer = 0.0f;
        GetComponent<Renderer> ().material = turret.projectileMat;
        GetComponent<TrailRenderer> ().startColor = turret.trailColor;
        GetComponent<TrailRenderer> ().endColor = turret.trailColor;
    }

    void Update () {
        if (disabled) {
            waitDestroyTimer += Time.deltaTime;
            if (waitDestroyTimer >= waitDestroy) Destroy (gameObject);
            return;
        }
        if (!initialized) return;
        if (turret.projectileSticky && target != null) endPosition = target.transform.position;
        float step = turret.projectileVelocity * Time.deltaTime;
        if (fuelExpended < turret.fuelRange) {
            Vector3 heading = endPosition - transform.position;
            Vector3 newDirection = Vector3.RotateTowards (transform.forward, heading, turret.projectileTracking * Mathf.Deg2Rad * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation (newDirection);
            transform.Translate (new Vector3 (0.0f, 0.0f, step));
            fuelExpended += step;
        } else Disable ();
        transform.position = new Vector3 (transform.position.x, 0.0f, transform.position.z);
        RaycastHit hit;
        if (Physics.Raycast (transform.position, transform.forward, out hit, step)) {
            GameObject hitGameObject = hit.transform.gameObject;
            StructureBehaviours hitStructureBehaviours = hitGameObject.GetComponent<StructureBehaviours> ();
            if (hitStructureBehaviours != null) {
                hitStructureBehaviours.TakeDamage (turret.damage * storedEnergyRatio, transform.position);
                Disable ();
            }
        }
    }

    void Disable () {
        disabled = true;
        GetComponent<MeshRenderer> ().enabled = false;
        TrailRenderer trail = GetComponent<TrailRenderer> ();
        if (trail != null) waitDestroy = trail.time;
    }
}