using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour {
    public SectorData sectorData;
    public GameObject killSpawnIn;
    public List<GameObject> tagged = new List<GameObject> ();

    void Update () {
        foreach (Transform child in transform)
            if (child.GetComponent<StructureBehaviours> () && child.GetComponent<StructureBehaviours> ().factionID != 1 &&
                child.localPosition.sqrMagnitude > sectorData.radius * sectorData.radius && !tagged.Contains (child.gameObject)) {
                Instantiate (killSpawnIn, transform);
                killSpawnIn.transform.localPosition = child.localPosition;
                tagged.Add (child.gameObject);
            }
    }
}

[Serializable]
public class SectorData {
    public int id;
    public float radius;
    public float alignment;
    public int skyboxID;
}