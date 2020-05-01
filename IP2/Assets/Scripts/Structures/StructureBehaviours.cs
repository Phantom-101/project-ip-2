using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureBehaviours : MonoBehaviour, IHealth, IMoveable {
    [SerializeField] float[] hitpoints;
    List<HealthChange> healthStack = new List<HealthChange>();
    List<MovementVector> movementStack = new List<MovementVector>();

    new ConstantForce constantForce;

    void Awake() {
        hitpoints = new float[3];
        constantForce = GetComponent<ConstantForce>();
    }

    public float GetTotalHealth() { float res = 0.0f; foreach(float hitpoint in hitpoints) res += hitpoint; return res; }
    public float GetLayerHealth(int i) { return hitpoints[i]; }
    public List<HealthChange> GetHealthChanges() { return healthStack; }
    public float GetHealthChangesSum() { float res = 0.0f; foreach(HealthChange healthChange in healthStack) res += healthChange.value; return res; }
    public void AddHealthChange(HealthChange healthChange) { healthStack.Add(healthChange); }
    public void ApplyHealthChanges() {
        foreach(HealthChange healthChange in healthStack.ToArray()) {
            float v = healthChange.value;
            for(int i = hitpoints.Length - 1; i >= 0 && v != 0.0f; i--) {
                if(!healthChange.bypasses[i]) {
                    float curHp = hitpoints[i]; float resist = GetComponent<StructureStatsManager>().GetStat("Resistance " + i);
                    float delta = v * (1 - resist) * healthChange.effectiveness[i];
                    if(curHp + delta >= 0.0f) {
                        hitpoints[i] += delta;
                        v = 0.0f;
                    } else {
                        hitpoints[i] = 0.0f;
                        HealthChange temp = healthChange;
                        v -= curHp;
                    }
                }
            }
        }
    }
    public void RegenerateHealth() { AddHealthChange(new HealthChange(1.0f, new float[] {0.0f, 0.0f, 1.0f}, new bool[] {true, true, false})); }
    public List<MovementVector> GetMovementVectors() { return movementStack; }
    public List<Vector3> GetTranslations() { List<Vector3> res = new List<Vector3>(); foreach(MovementVector movementVector in movementStack) res.Add(movementVector.translation); return res;}
    public Vector3 GetTranslationsMagnitude() { Vector3 res = Vector3.zero; foreach(MovementVector movementVector in movementStack) res += movementVector.translation; return res;}
    public List<Vector3> GetRotations() { List<Vector3> res = new List<Vector3>(); foreach(MovementVector movementVector in movementStack) res.Add(movementVector.rotation); return res;}
    public Vector3 GetRotationsMagnitude() { Vector3 res = Vector3.zero; foreach(MovementVector movementVector in movementStack) res += movementVector.rotation; return res;}
    public void AddMovementVector(MovementVector movementVector) { movementStack.Add(movementVector); }
    public void ApplyMovementVectors() { constantForce.relativeForce = GetTranslationsMagnitude(); constantForce.relativeTorque = GetRotationsMagnitude(); }
}
