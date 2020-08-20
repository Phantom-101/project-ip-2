using UnityEngine;

public class CelestialObject : MonoBehaviour {
    public int celestialObjectID;
    public bool initialized = false;

    public ResourcesManager resourcesManager;

    void Awake () {
        resourcesManager = FindObjectOfType<ResourcesManager> ();
    }

    public void Initialize () {
        GameObject obj = Instantiate (resourcesManager.celestialObjects[celestialObjectID], transform);
        obj.transform.parent = transform.parent;
        initialized = true;
    }
}
