using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIHandlerInitializer : MonoBehaviour {
    public bool active;

    void Awake () {
        if (active) GetComponent<GameUIHandler> ().Initialize ();
    }
}
