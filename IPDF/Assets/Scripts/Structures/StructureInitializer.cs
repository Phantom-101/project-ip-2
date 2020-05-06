using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureInitializer : MonoBehaviour {
    void Awake () {
        StructureBehaviours structureBehaviours = GetComponent<StructureBehaviours> ();
        if (structureBehaviours != null) structureBehaviours.Initialize ();
    }
}
