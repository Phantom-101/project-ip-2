using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {
    public CameraFollowPlayer cameraFollowPlayer;
    public Camera camera;

    void Update () {
        cameraFollowPlayer = FindObjectOfType<CameraFollowPlayer> ();
        camera = cameraFollowPlayer.GetComponent<Camera> ();
        transform.LookAt (camera.transform.position);
    }
}
