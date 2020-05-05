using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public Turret turret;
    public GameObject activator;
    public GameObject target;
    public float storedEnergyRatio;

    bool initialized = false;

    public void Initialize (Turret turret, GameObject activator, GameObject target, float storedEnergyRatio) {
        this.turret = turret;
        this.activator = activator;
        this.target = target;
        this.storedEnergyRatio = storedEnergyRatio;
        initialized = true;
    }

    void Update () {
        if (!initialized) return;
        float step = turret.projectileVelocity * Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit, step)) {
            GameObject hitGameObject = hit.transform.gameObject;
            StructureBehaviours hitStructureBehaviours = hitGameObject.GetComponent<StructureBehaviours> ();
            if (hitStructureBehaviours != null) {
                hitStructureBehaviours.TakeDamage (turret.damage * storedEnergyRatio, transform.position);
                Destroy(gameObject);
            }
        }
    }
}