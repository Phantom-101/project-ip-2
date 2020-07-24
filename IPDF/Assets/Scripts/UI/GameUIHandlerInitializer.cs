using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIHandlerInitializer : MonoBehaviour {
    public bool active;

    void Start () {
        if (active) GameUIHandler.GetInstance ().Initialize ();
    }
}
