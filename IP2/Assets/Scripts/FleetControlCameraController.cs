using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetControlCameraController : MonoBehaviour
{
    public float currentZoom = 1.0f;
    public float targetZoom = 1.0f;
    public float zoomInterpolation = 0.0f;

    float offset = -1.0f;
    Vector3 desiredPosition;
    public GameObject target;

    void Awake ()
    {
        target = GameObject.Find("Player");
    }

    void Update ()
    {
        if(target != null) {
            desiredPosition = target.transform.position;
            Vector3 dir = target.transform.position - transform.position;
            desiredPosition += dir.normalized * offset;
            transform.position = desiredPosition;
            float input = -Input.GetAxis("ArrowVertical");
            if (transform.position.y < target.transform.position.y + 0.95f * -offset && input > 0.0f) {
                transform.RotateAround(target.transform.position, transform.TransformDirection(Vector3.right), input * 100.0f * Time.deltaTime);
            }
            if (transform.position.y > target.transform.position.y - 0.95f * -offset && input < 0.0f) {
                transform.RotateAround(target.transform.position, transform.TransformDirection(Vector3.right), input * 100.0f * Time.deltaTime);
            }
            transform.RotateAround(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z), Vector3.up, Input.GetAxis("ArrowHorizontal") * 100.0f * Time.deltaTime);
            transform.LookAt(target.transform.position);
            ChangeZoom();
        }
    }

    void ChangeZoom()
    {
        float input = Input.GetAxis("Mouse ScrollWheel");
        if(input != 0.0f) {
            zoomInterpolation = 0.0f;
            targetZoom = Mathf.Clamp(currentZoom - input * 20.0f, 0.25f, 50.0f);
        }
        if (zoomInterpolation < 1.0f) zoomInterpolation += 0.5f * Time.deltaTime;
        else if (zoomInterpolation > 1.0f) zoomInterpolation = 1.0f;
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, zoomInterpolation);
        offset = -currentZoom;
    }
}
