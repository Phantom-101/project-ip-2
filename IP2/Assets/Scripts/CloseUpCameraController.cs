using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseUpCameraController : MonoBehaviour {
    public float currentZoom = 1.0f;
    public float targetZoom = 1.0f;
    public float zoomInterpolation = 0.0f;

    float offset = -1.0f;
    Vector3 desiredPosition;
    public GameObject target;

    void Awake() {
        target = GameObject.Find("Player");
        if(target != null) {
            desiredPosition = target.transform.position + target.transform.forward * offset;
            transform.position = desiredPosition;
        }
    }

    void Update() {
        if(target != null) {
            desiredPosition = target.transform.position + target.transform.up * 0.25f + target.transform.forward * offset;
            transform.position = Vector3.Slerp(transform.position, desiredPosition, Time.deltaTime);
            transform.LookAt(target.transform.position, transform.up);
            ChangeZoom();
        }
    }

    void ChangeZoom() {
        float input = Input.GetAxis("Mouse ScrollWheel");
        if (input != 0.0f) {
            zoomInterpolation = 0.0f;
            float delta = input * 7.5f;
            targetZoom = Mathf.Clamp(currentZoom - delta, 0.25f, 2.5f);
        }
        if (zoomInterpolation < 1.0f) zoomInterpolation += 0.5f * Time.deltaTime;
        else if (zoomInterpolation > 1.0f) zoomInterpolation = 1.0f;
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, zoomInterpolation);
        offset = -currentZoom;
    }
}
