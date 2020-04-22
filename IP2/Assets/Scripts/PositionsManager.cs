using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionsManager : MonoBehaviour {
    Position[] positions;

    void Awake() {
        positions = FindObjectsOfType<Position>();
    }

    public void ShiftOrigion(Vector3 playerLocation) {
        foreach(Position position in positions) position.Translate(-playerLocation);
    }
}
