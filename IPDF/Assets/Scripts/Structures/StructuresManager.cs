using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

public class StructuresManager : MonoBehaviour {
    public List<StructureBehaviours> structures;

    public void AddStructure (StructureBehaviours structure) {
        structures.Add (structure);
    }

    public void RemoveStructure (StructureBehaviours structure) {
        structures.Remove (structure);
    }
}