using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialObject : MonoBehaviour {
    public int celestialObjectID;
    public bool initialized = false;

    public ResourcesManager resourcesManager;

    void Awake () {
        resourcesManager = FindObjectOfType<ResourcesManager> ();
    }

    public void Initialize () {
        Instantiate (resourcesManager.celestialObjects[celestialObjectID], transform);
        initialized = true;
    }
}
