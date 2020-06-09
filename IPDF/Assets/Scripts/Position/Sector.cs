using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour {
    public SectorData sectorData;
}

[Serializable]
public class SectorData {
    public int id;
    public float radius;
    public float alignment;
    public int skyboxID;
}