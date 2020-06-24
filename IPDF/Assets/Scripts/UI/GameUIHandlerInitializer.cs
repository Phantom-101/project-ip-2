using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIHandlerInitializer : MonoBehaviour {
    void Awake () {
        GetComponent<GameUIHandler> ().Initialize ();
    }
}
