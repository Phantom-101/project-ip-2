using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionsManager : MonoBehaviour {
    Position[] positions;

    public void ShiftOrigin(Vector3 playerLocation) {
        positions = FindObjectsOfType<Position>();
        foreach(Position position in positions) position.ShiftOrigin(playerLocation);
    }
}
