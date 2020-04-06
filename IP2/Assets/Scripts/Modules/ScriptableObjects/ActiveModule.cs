using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActivationAnchor {
    Start,
    End
}

public class ActiveModule : Module
{
    public float cycleTime;
    public float cycleTimeVariation;
    public bool cycleInterruptable;
    public ActivationAnchor activationAnchor;
    public float activationOffset;
    public int activationBursts;
    public float burstInterval;
    public bool repeating;
}
