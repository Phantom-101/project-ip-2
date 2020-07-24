using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour {
    public static ResourcesManager current;

    public Material[] skyboxes;
    public GameObject[] celestialObjects;
    public AudioResources audioResources;

    void Awake () {
        current = this;
    }

    public static ResourcesManager GetInstance () {
        return current;
    }
}
