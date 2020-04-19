using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /*
    public float or;
    public float fr;
    public float dm;
    public GameObject from;
    public GameObject to;
    public Charge ammo;
    public Vector3 shotPosition;
    public float damageMult;
    public float projSpeedMult;
    public string fromFaction;
    
    void Start() {
        if(to != null) transform.LookAt(to.transform.position);
        shotPosition = transform.position;
        if(from == null) Destroy(gameObject);
        else {
            damageMult = from.GetComponent<StructureStatsManager>().GetStat("structure turret damage multiplier");
            projSpeedMult = from.GetComponent<StructureStatsManager>().GetStat("structure projectile speed multiplier");
            fromFaction = from.GetComponent<StructureStatsManager>().faction;
        }
    }

    void Update()
    {
        if(Vector3.Distance(shotPosition, transform.position) > fr) Destroy(gameObject);
        if(ammo.tracking && to == null) Destroy(gameObject);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, (ammo.speed * projSpeedMult) * Time.deltaTime * 2.0f))
        {
            if(hit.transform.gameObject != from && hit.transform.gameObject.GetComponent<StructureStatsManager>() && hit.transform.gameObject.GetComponent<StructureStatsManager>().faction != fromFaction)
            {
                float distance = Vector3.Distance(shotPosition, hit.transform.position);
                float maxAccu;
                if (distance > fr) maxAccu = 0.0f;
                else if (distance > or) maxAccu = (distance - or) / (fr - or);
                else maxAccu = 1.0f;
                hit.transform.gameObject.GetComponent<StructureStatsManager>().TakeDamage(ammo.damage * dm * damageMult, from);
                Destroy(gameObject);
            }
        }
        if(ammo.tracking && to != null) transform.LookAt(to.transform.position);
        transform.Translate(Vector3.forward * ammo.speed * projSpeedMult * Time.deltaTime);
    }
    */
}
