using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour {
    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public void SetLoadedScene(string name) {
        SceneManager.LoadScene(name);
    }

    public void Exit() {
        Application.Quit();
    }
}
