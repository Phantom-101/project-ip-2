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

    void Start() {
        ssm = GetComponent<StructureStatsManager>();
    }

    void Update() {
        CalculateTargets();
        InterpolateCurrents();
        transform.Translate(currentTranslation * Time.deltaTime);
        transform.Rotate(currentRotation * Time.deltaTime);
    }

    void CalculateTargets() {
        if(translationPercentage.x > 1.0f) translationPercentage.x = 1.0f;
        targetTranslation.x = ssm.GetStat("structure speed") * translationPercentage.x;
        if(translationPercentage.y > 1.0f) translationPercentage.y = 1.0f;
        targetTranslation.y = ssm.GetStat("structure speed") * translationPercentage.y;
        if(translationPercentage.z > 1.0f) translationPercentage.z = 1.0f;
        targetTranslation.z = ssm.GetStat("structure speed") * translationPercentage.z;
        if(rotationPercentage.x > 1.0f) rotationPercentage.x = 1.0f;
        targetRotation.x = ssm.GetStat("structure turn speed") * rotationPercentage.x;
        if(rotationPercentage.y > 1.0f) rotationPercentage.y = 1.0f;
        targetRotation.y = ssm.GetStat("structure turn speed") * rotationPercentage.y;
        if(rotationPercentage.z > 1.0f) rotationPercentage.z = 1.0f;
        targetRotation.z = ssm.GetStat("structure turn speed") * rotationPercentage.z;
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
