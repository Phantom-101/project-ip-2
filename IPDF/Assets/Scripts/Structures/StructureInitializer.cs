using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureInitializer : MonoBehaviour {
    void Start () {
        StructureBehaviours structureBehaviours = GetComponent<StructureBehaviours> ();
        if (structureBehaviours != null) structureBehaviours.Initialize ();
    }
}
