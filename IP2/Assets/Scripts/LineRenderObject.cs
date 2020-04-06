using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRenderObject : MonoBehaviour
{
    public Turret firedFrom;
    public Vector3 from;
    public Vector3 to;

    bool active;
    LineRenderer lr;
    float timePassed;
    Gradient gradient;
    GradientColorKey[] colorKeys;
    GradientAlphaKey[] alphaKeys;

    void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    void Start()
    {
        lr.colorGradient = firedFrom.LROGradient;
        if(firedFrom.useWidthMultiplier) {
            lr.startWidth = firedFrom.LROEndWidth * firedFrom.LROWidthMultiplierCurve.Evaluate(0.0f);
            lr.endWidth = firedFrom.LROStartWidth * firedFrom.LROWidthMultiplierCurve.Evaluate(0.0f);
        } else {
            lr.widthMultiplier = firedFrom.LROWidthMultiplier;
            lr.widthCurve = firedFrom.LROWidthCurve;
        }
    }

    public void Activate() {
        timePassed = 0.0f;
        active = true;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
    }

    void Update()
    {
        if(!active) {
            lr.enabled = false;
        } else {
            lr.enabled = true;
            if(timePassed < firedFrom.LROTime) {
                timePassed += Time.deltaTime;
                if(firedFrom.LROInterpolatePositions) {
                    Vector3 dif = to - from;
                    if(timePassed / firedFrom.LROTime > firedFrom.LROInterpolationRenderPercentage) {
                        lr.SetPosition(0, from + dif * ((timePassed / firedFrom.LROTime) - firedFrom.LROInterpolationRenderPercentage));
                    }
                    lr.SetPosition(1, from + dif * (timePassed / firedFrom.LROTime));
                }
                if(firedFrom.useWidthMultiplier) {
                    lr.startWidth = firedFrom.LROEndWidth * firedFrom.LROWidthMultiplierCurve.Evaluate(timePassed / firedFrom.LROTime);
                    lr.endWidth = firedFrom.LROStartWidth * firedFrom.LROWidthMultiplierCurve.Evaluate(timePassed / firedFrom.LROTime);
                }
                gradient = new Gradient();
                colorKeys = new GradientColorKey[2];
                colorKeys[0].color = firedFrom.LROGradient.colorKeys[0].color;
                colorKeys[0].time = 0.0f;
                colorKeys[1].color = firedFrom.LROGradient.colorKeys[1].color;
                colorKeys[1].time = 1.0f;
                alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0].alpha = firedFrom.LROEndAlpha * firedFrom.LROAlphaMultiplierCurve.Evaluate(timePassed / firedFrom.LROTime) / 255.0f;
                alphaKeys[0].time = 0.0f;
                alphaKeys[1].alpha = firedFrom.LROStartAlpha * firedFrom.LROAlphaMultiplierCurve.Evaluate(timePassed / firedFrom.LROTime) / 255.0f;
                alphaKeys[1].time = 1.0f;
                gradient.SetKeys(colorKeys, alphaKeys);
                lr.colorGradient = gradient;
            } else {
                active = false;
            }
        }
    }
}
