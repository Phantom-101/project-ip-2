using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Turret", menuName = "ScriptableObjects/Turret")]
public class Turret : ActiveModule
{
    public float damageMultiplier;
    public float optimalRange;
    public float falloffRange;
    public float tracking;
    public Ammunition[] accepted;
    public bool showLoadedAmmoIcon;
    public GameObject LRO;
    public Gradient LROGradient;
    public AnimationCurve LROWidthCurve;
    public float LROWidthMultiplier;
    public bool useWidthMultiplier;
    public float LROStartWidth;
    public float LROEndWidth;
    public AnimationCurve LROWidthMultiplierCurve;
    public float LROTime;
    public float LROStartAlpha;
    public float LROEndAlpha;
    public AnimationCurve LROAlphaMultiplierCurve;
    public bool LROInterpolatePositions;
    public float LROInterpolationRenderPercentage;
}