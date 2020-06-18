using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialObject : MonoBehaviour {
    public int celestialObjectID;
    public bool initialized = false;

    public GraphicsManager graphicsManager;

    void Awake () {
        graphicsManager = FindObjectOfType<GraphicsManager> ();
    }

    public void Initialize () {
        Instantiate (graphicsManager.celestialObjects[celestialObjectID], transform);
        initialized = true;
    }
}
