using System.Collections.Generic;
using UnityEngine;

public class StructuresManager : MonoBehaviour {
    public static StructuresManager current;

    public List<StructureBehaviours> structures;
    public GameObject cargoPod;

    void Awake () {
        current = this;
    }

    public static StructuresManager GetInstance () {
        return current;
    }

    public void TickStructures (float deltaTime) {
        foreach (StructureBehaviours structure in structures.ToArray ()) structure.Tick (deltaTime);
    }

    public void AddStructure (StructureBehaviours structure) {
        if (structure == null) return;
        structures.Add (structure);
        if (structure.id == null || structure.id == "")
            structure.id = System.Guid.NewGuid ().ToString ();
    }

    public void RemoveStructure (StructureBehaviours structure) {
        if (structure == null) return;
        structures.Remove (structure);
    }
}