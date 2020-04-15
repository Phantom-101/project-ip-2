using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureMovementManager : MonoBehaviour
{
    [SerializeField] Vector3 targetTranslationPercentage;
    [SerializeField] Vector3 targetRotationPercentage;
    [SerializeField] Vector3 targetTranslation;
    [SerializeField] Vector3 targetRotation;
    [SerializeField] Vector3 currentTranslationPercentage;
    [SerializeField] Vector3 currentRotationPercentage;
    [SerializeField] Vector3 currentTranslation;
    [SerializeField] Vector3 currentRotation;
    StructureStatsManager ssm;
    float speed;
    float turnSpeed;
    public List<string> orders = new List<string>();
    GameObject t;

    void Start() {
        ssm = GetComponent<StructureStatsManager>();
    }

    void Update() {
        speed = ssm.GetStat("Speed");
        turnSpeed = ssm.GetStat("Turn Speed");
        ExecuteOrders();
        CalculateTargets();
        InterpolateCurrents();
        transform.Translate(currentTranslation * Time.deltaTime * ((orders.Count > 0 && orders[0] == "Warp") ? ssm.GetStat("Warp Speed") / speed : 1.0f));
        transform.Rotate(currentRotation * Time.deltaTime);
    }

    public void ClearOrders() {
        orders = new List<string>();
    }

    public void SetTarget(GameObject target) {
        t = target;
    }

    public void AddOrder(string order){
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
                Vector3 heading = t.transform.position - transform.position;
                Vector3 LRPerp = Vector3.Cross(transform.forward, heading);
                float LRDif = Vector3.Dot(LRPerp, transform.up);
                float absLRDif = Mathf.Abs(LRDif);
                float warpAccuracy = 1.0f / Mathf.Sqrt(ssm.GetStat("Warp Accuracy"));
                if(absLRDif > warpAccuracy / 2.0f) targetRotationPercentage.y = LRDif;
                Vector3 UDPerp = Vector3.Cross(transform.forward, heading);
                float UDDif = Vector3.Dot(UDPerp, transform.right);
                float absUDDif = Mathf.Abs(UDDif);
                if(absUDDif > warpAccuracy / 2.0f) targetRotationPercentage.x = UDDif;
                if(absLRDif <= warpAccuracy && absUDDif <= warpAccuracy) orders.RemoveAt(0);
                return;
            }
            if (currentOrder == "Warp") {
                targetRotationPercentage = Vector3.zero;
                if(ssm.GetStat("Warp Field Strength") <= 0.0f) orders.RemoveAt(0);
                float dis = Vector3.Distance(transform.position, t.transform.position);
                if(dis > 1.0f) targetTranslationPercentage = Vector3.forward;
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

    void InterpolateCurrents() {
        float speedInterpolation = ssm.GetStat("Speed Interpolation");
        float turnSpeedInterpolation = ssm.GetStat("Turn Speed Interpolation");
        currentTranslationPercentage.x = LinearInterpolate(currentTranslationPercentage.x, targetTranslationPercentage.x, speedInterpolation);
        currentTranslationPercentage.y = LinearInterpolate(currentTranslationPercentage.y, targetTranslationPercentage.y, speedInterpolation);
        currentTranslationPercentage.z = LinearInterpolate(currentTranslationPercentage.z, targetTranslationPercentage.z, speedInterpolation);
        currentRotationPercentage.x = LinearInterpolate(currentRotationPercentage.x, targetRotationPercentage.x, turnSpeedInterpolation);
        currentRotationPercentage.y = LinearInterpolate(currentRotationPercentage.y, targetRotationPercentage.y, turnSpeedInterpolation);
        currentRotationPercentage.z = LinearInterpolate(currentRotationPercentage.z, targetRotationPercentage.z, turnSpeedInterpolation);
        currentTranslation = currentTranslationPercentage * speed;
        currentRotation = currentRotationPercentage * turnSpeed;
    }

    public void SetAxisTranslation(Axis axis, float percent) {
        if(axis == Axis.X) {
            targetTranslationPercentage.x = percent;
        } else if (axis == Axis.Y) {
            targetTranslationPercentage.y = percent;
        } else {
            targetTranslationPercentage.z = percent;
        }
    }

    public void ChangeAxisTranslation(Axis axis, float percent) {
        if(axis == Axis.X) {
            targetTranslationPercentage.x += percent;
        } else if (axis == Axis.Y) {
            targetTranslationPercentage.y += percent;
        } else {
            targetTranslationPercentage.z += percent;
        }
    }

    public void SetPlaneRotation(Plane plane, float percent) {
        if(plane == Plane.XY) {
            targetRotationPercentage.z = percent;
        } else if (plane == Plane.YZ) {
            targetRotationPercentage.x = percent;
        } else {
            targetRotationPercentage.y = percent;
        }
    }

    public void ChangePlaneRotation(Plane plane, float percent) {
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
