using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesManager : MonoBehaviour {
    public void SetLoadedScene (string name) {
        Canvas[] canvases = FindObjectsOfType<Canvas> ();
        foreach (Canvas canvas in canvases) Destroy (canvas.gameObject);
        SceneManager.LoadScene (name);
    }

    public void Exit () {
        Application.Quit ();
    }
}
