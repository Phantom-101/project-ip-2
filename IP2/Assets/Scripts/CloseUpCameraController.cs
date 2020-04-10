using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseUpCameraController : MonoBehaviour
{
    public float currentZoom = 1.0f;
    public float targetZoom = 1.0f;
    public float zoomInterpolation = 0.0f;

    float offset = -1.0f;
    Vector3 desiredPosition;
    public GameObject target;

    void Awake()
    {
        target = GameObject.Find("Player");
        if(target != null) {
            desiredPosition = target.transform.position + target.transform.forward * offset;
            transform.position = desiredPosition;
        }
    }

    void Update()
    {
        if(target != null) {
            desiredPosition = target.transform.position + target.transform.up * 0.15f + target.transform.forward * offset;
            transform.position = Vector3.Slerp(transform.position, desiredPosition, Time.deltaTime);
            transform.LookAt(target.transform.position, transform.up);
            ChangeZoom();
        }
    }

    void ChangeZoom()
    {
        float input = Input.GetAxis("Mouse ScrollWheel");
        if (input != 0.0f)
        {
            zoomInterpolation = 0.0f;
            float delta = 0.0f;
            if(input > 0.0f) delta = input * 0.2f;
            else delta = input * 0.3f;
            targetZoom = Mathf.Clamp(currentZoom - delta, 0.025f, 0.25f);
        }
        if (zoomInterpolation < 1.0f) zoomInterpolation += 0.5f * Time.deltaTime;
        else if (zoomInterpolation > 1.0f) zoomInterpolation = 1.0f;
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, zoomInterpolation);
        offset = -currentZoom;
    }
}
