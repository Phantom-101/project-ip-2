﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Item {
    public float signature;
    public bool activatable;
    public float cycleTime;
    public bool cycleInterruptable;
    public float[] activations;
    public AudioClip[] activationClips;
    public bool repeating;
    public bool mustBeTargeted;
    public float range;
    public string[] rangeStats;
    public bool requireCharge;
    public Charge[] accepted;
    public bool showLoadedChargeIcon;
    public GameObject vfx;
}
