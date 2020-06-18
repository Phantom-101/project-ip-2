using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialObjectInitializer : MonoBehaviour {
    void Start () {
        CelestialObject celestialObject = GetComponent<CelestialObject> ();
        if (celestialObject != null) celestialObject.Initialize ();
    }
}
