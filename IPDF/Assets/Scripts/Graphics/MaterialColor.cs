using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColor : MonoBehaviour {
    public Color color;
    public Renderer renderer;
    
    void Awake () {
        renderer = GetComponent<Renderer> ();
    }

    void Update () {
        renderer.material.SetColor ("_BaseColor", color);
        renderer.material.SetColor ("_EmissionColor", color);
    }
}
