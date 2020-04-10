using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureMovementManager : MonoBehaviour
{
    [SerializeField] Vector3 targetTranslation;
    [SerializeField] Vector3 targetRotation;
    [SerializeField] Vector3 currentTranslation;
    [SerializeField] Vector3 currentRotation;
    [SerializeField] Vector3 translationPercentage;
    [SerializeField] Vector3 rotationPercentage;
    StructureStatsManager ssm;
    public string currentOrder = "";
    GameObject t;

    void Start() {
        ssm = GetComponent<StructureStatsManager>();
    }

    void Update() {
        if(currentOrder != "") {
            if(t == null) {
                currentOrder = "";
                return;
            }
            if(currentOrder == "Align") {
                translationPercentage = Vector3.forward;
                Vector3 heading = t.transform.position - transform.position;
                Vector3 LRPerp = Vector3.Cross(transform.forward, heading);
                float LRDif = Vector3.Dot(LRPerp, transform.up);
                float warpAccuracy = 1.0f / Mathf.Sqrt(ssm.GetStat("Warp Accuracy"));
                if(Mathf.Abs(LRDif) > warpAccuracy / 2.0f) rotationPercentage.y = LRDif > 1.0f ? 1.0f : -1.0f;
                Vector3 UDPerp = Vector3.Cross(transform.forward, heading);
                float UDDif = Vector3.Dot(UDPerp, transform.right);
                if(Mathf.Abs(UDDif) > warpAccuracy / 2.0f) rotationPercentage.x = UDDif > 1.0f ? 1.0f : -1.0f;
                if(Mathf.Abs(LRDif) <= warpAccuracy && Mathf.Abs(UDDif) <= warpAccuracy) {
                    currentOrder = "Warp";
                    return;
                }
            } else if (currentOrder == "Warp") {
                float dis = Vector3.Distance(transform.position, t.transform.position);
                if(dis > 1.0f) translationPercentage = Vector3.forward * ssm.GetStat("Warp Speed Factor") * ssm.GetStat("Warp Speed Factor");
                else {
                    translationPercentage = Vector3.zero;
                    currentOrder = "";
                    return;
                }
            }
        }
        CalculateTargets();
        InterpolateCurrents();
        transform.Translate(currentTranslation * Time.deltaTime);
        transform.Rotate(currentRotation * Time.deltaTime);
    }

    public void OverrideOrder() {
        currentOrder = "";
    }

    public void WarpTo(GameObject target) {
        t = target;
        currentOrder = "Align";
    }

    void CalculateTargets() {
        if(translationPercentage.x > 1.0f && currentOrder != "Warp") translationPercentage.x = 1.0f;
        targetTranslation.x = ssm.GetStat("Speed") * translationPercentage.x;
        if(translationPercentage.y > 1.0f && currentOrder != "Warp") translationPercentage.y = 1.0f;
        targetTranslation.y = ssm.GetStat("Speed") * translationPercentage.y;
        if(translationPercentage.z > 1.0f && currentOrder != "Warp") translationPercentage.z = 1.0f;
        targetTranslation.z = ssm.GetStat("Speed") * translationPercentage.z;
        if(rotationPercentage.x > 1.0f) rotationPercentage.x = 1.0f;
        targetRotation.x = ssm.GetStat("Turn Speed") * rotationPercentage.x;
        if(rotationPercentage.y > 1.0f) rotationPercentage.y = 1.0f;
        targetRotation.y = ssm.GetStat("Turn Speed") * rotationPercentage.y;
        if(rotationPercentage.z > 1.0f) rotationPercentage.z = 1.0f;
        targetRotation.z = ssm.GetStat("Turn Speed") * rotationPercentage.z;
    }

    void InterpolateCurrents() {
        currentTranslation.x = LinearInterpolate(currentTranslation.x, targetTranslation.x, 25.0f);
        currentTranslation.y = LinearInterpolate(currentTranslation.y, targetTranslation.y, 25.0f);
        currentTranslation.z = LinearInterpolate(currentTranslation.z, targetTranslation.z, 25.0f);
        currentRotation.x = LinearInterpolate(currentRotation.x, targetRotation.x, 10.0f);
        currentRotation.y = LinearInterpolate(currentRotation.y, targetRotation.y, 10.0f);
        currentRotation.z = LinearInterpolate(currentRotation.z, targetRotation.z, 10.0f);
    }

    public void SetAxisTranslation(Axis axis, float percent) {
        if(axis == Axis.X) {
            translationPercentage.x = percent;
        } else if (axis == Axis.Y) {
            translationPercentage.y = percent;
        } else {
            translationPercentage.z = percent;
        }
    }

    public void ChangeAxisTranslation(Axis axis, float percent) {
        if(axis == Axis.X) {
            translationPercentage.x += percent;
        } else if (axis == Axis.Y) {
            translationPercentage.y += percent;
        } else {
            translationPercentage.z += percent;
        }
    }

    public void SetPlaneRotation(Plane plane, float percent) {
        if(plane == Plane.XY) {
            rotationPercentage.z = percent;
        } else if (plane == Plane.YZ) {
            rotationPercentage.x = percent;
        } else {
            rotationPercentage.y = percent;
        }
    }

    public void ChangePlaneRotation(Plane plane, float percent) {
        if(plane == Plane.XY) {
            rotationPercentage.z += percent;
        } else if (plane == Plane.YZ) {
            rotationPercentage.x += percent;
        } else {
            rotationPercentage.y += percent;
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
}
