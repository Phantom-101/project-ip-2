using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth {
    void ApplyHealthChanges(float[] health, List<HealthChange> healthChanges);
}

public interface IMoveable {
    void Move(List<Vector3> movementVectors);
}