using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureMovementManager : MonoBehaviour {
    // Movement
    [SerializeField] Vector3 targetTranslationPercentage;
    [SerializeField] Vector3 targetRotationPercentage;
    [SerializeField] Vector3 targetTranslation;
    [SerializeField] Vector3 targetRotation;
    // Cache managers
    StructureInitializer structureInitializer;
    StructureStatsManager structureStatsManager;
    // Cache stats
    float speed;
    float turnSpeed;
    // Movement orders
    public List<string> orders = new List<string>();
    // Selected
    Vector3 t;
    // Rigidbody and ConstantForce of structure
    Rigidbody rigidbody;
    ConstantForce constantForce;

    // Has this component been initialized?
    bool initialized = false;

    public void Initialize(StructureInitializer initializer) {
        structureInitializer = initializer;
        structureStatsManager = initializer.structureStatsManager;
        // Initialize physics
        rigidbody = gameObject.AddComponent<Rigidbody>();
        constantForce = gameObject.AddComponent<ConstantForce>();
        rigidbody.mass = structureStatsManager.profile.mass;
        rigidbody.drag = structureStatsManager.profile.drag;
        rigidbody.angularDrag = structureStatsManager.profile.angularDrag;
        initialized = true;
    }

    void Update() {
        if(!initialized) return;
        speed = structureStatsManager.GetStat("Speed");
        turnSpeed = structureStatsManager.GetStat("Turn Speed");
        ExecuteOrders();
        CalculateTargets();
        Vector3 translation = targetTranslation * ((orders.Count > 0 && orders[0] == "Warp") ? structureStatsManager.GetStat("Warp Speed") : 1.0f);
        constantForce.relativeForce = translation;
        constantForce.relativeTorque = targetRotation;
    }

    public void ClearOrders() {
        if(!initialized) return;
        orders = new List<string>();
    }

    public void SetTarget(Vector3 target) {
        if(!initialized) return;
        t = target;
    }

    public void AddOrder(string order){
        if(!initialized) return;
        orders.Add(order);
    }

    void ExecuteOrders() {
        if(orders.Count > 0) {
            if(t == null) {
                orders = new List<string>();
                return;
            }
            string currentOrder = orders[0];
            if(currentOrder == "Align") {
                Vector3 heading = t - transform.position;
                Vector3 LRPerp = Vector3.Cross(transform.forward, heading);
                float LRDif = Vector3.Dot(LRPerp, transform.up);
                float absLRDif = Mathf.Abs(LRDif);
                float warpAccuracy = 1.0f / Mathf.Sqrt(structureStatsManager.GetStat("Warp Accuracy"));
                if(absLRDif > warpAccuracy / 2.0f) targetRotationPercentage.y = LRDif / 25.0f;
                Vector3 UDPerp = Vector3.Cross(transform.forward, heading);
                float UDDif = Vector3.Dot(UDPerp, transform.right);
                float absUDDif = Mathf.Abs(UDDif);
                if(absUDDif > warpAccuracy / 2.0f) targetRotationPercentage.x = UDDif / 25.0f;
                if(absLRDif <= warpAccuracy && absUDDif <= warpAccuracy) orders.RemoveAt(0);
                return;
            }
            if (currentOrder == "Warp") {
                targetRotationPercentage = Vector3.zero;
                if(structureStatsManager.GetStat("Warp Field Strength") <= 0.0f) orders.RemoveAt(0);
                float dis = Vector3.Distance(transform.position, t);
                if(dis > 5.0f) targetTranslationPercentage = Vector3.forward;
                else {
                    targetTranslationPercentage = Vector3.zero;
                    orders.RemoveAt(0);
                }
                return;
            }
        }
    }

    void CalculateTargets() {
        targetTranslationPercentage.x = Clamp(targetTranslationPercentage.x, -1.0f, 1.0f);
        targetTranslation.x = speed * targetTranslationPercentage.x;
        targetTranslationPercentage.y = Clamp(targetTranslationPercentage.y, -1.0f, 1.0f);
        targetTranslation.y = speed * targetTranslationPercentage.y;
        targetTranslationPercentage.z = Clamp(targetTranslationPercentage.z, -1.0f, 1.0f);
        targetTranslation.z = speed * targetTranslationPercentage.z;
        targetRotationPercentage.x = Clamp(targetRotationPercentage.x, -1.0f, 1.0f);
        targetRotation.x = turnSpeed * targetRotationPercentage.x;
        targetRotationPercentage.y = Clamp(targetRotationPercentage.y, -1.0f, 1.0f);
        targetRotation.y = turnSpeed * targetRotationPercentage.y;
        targetRotationPercentage.z = Clamp(targetRotationPercentage.z, -1.0f, 1.0f);
        targetRotation.z = turnSpeed * targetRotationPercentage.z;
    }

    public void SetAxisTranslation(Axis axis, float percent) {
        if(!initialized) return;
        if(axis == Axis.X) {
            targetTranslationPercentage.x = percent;
        } else if (axis == Axis.Y) {
            targetTranslationPercentage.y = percent;
        } else {
            targetTranslationPercentage.z = percent;
        }
    }

    public void ChangeAxisTranslation(Axis axis, float percent) {
        if(!initialized) return;
        if(axis == Axis.X) {
            targetTranslationPercentage.x += percent;
        } else if (axis == Axis.Y) {
            targetTranslationPercentage.y += percent;
        } else {
            targetTranslationPercentage.z += percent;
        }
    }

    public void SetPlaneRotation(Plane plane, float percent) {
        if(!initialized) return;
        if(plane == Plane.XY) {
            targetRotationPercentage.z = percent;
        } else if (plane == Plane.YZ) {
            targetRotationPercentage.x = percent;
        } else {
            targetRotationPercentage.y = percent;
        }
    }

    public void ChangePlaneRotation(Plane plane, float percent) {
        if(!initialized) return;
        if(plane == Plane.XY) {
            targetRotationPercentage.z += percent;
        } else if (plane == Plane.YZ) {
            targetRotationPercentage.x += percent;
        } else {
            targetRotationPercentage.y += percent;
        }
    }

    float LinearInterpolate(float current, float target, float interpolationValue) {
        if(current == target) return target;
        float dt = Time.deltaTime;
        if(Mathf.Abs(target - current) <= interpolationValue * dt) return target;
        if (current > target) {
            return current - interpolationValue * dt;
        } else {
            return current + interpolationValue * dt;
        }
    }
    
    float Clamp(float current, float lower, float upper) {
        if(current < lower) current = lower;
        else if (current > upper) current = upper;
        return current;
    }
}
