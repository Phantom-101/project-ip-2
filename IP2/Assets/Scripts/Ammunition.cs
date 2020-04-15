using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ammunition", menuName = "ScriptableObjects/Ammunition")]
public class Ammunition : Item
{
    public float damage;
    public float speed;
    public bool tracking;
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
