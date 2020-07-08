using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Essentials;

public class StructuresManager : MonoBehaviour {
    public List<StructureBehaviours> structures;
    public GameObject cargoPod;

    public void TickStructures (float deltaTime) {
        foreach (StructureBehaviours structure in structures.ToArray ()) structure.Tick (deltaTime);
    }

    public void AddStructure (StructureBehaviours structure) {
        if (structure == null) return;
        structures.Add (structure);
        if (structure.id == 0) {
            while (true) {
                int randomizedID = (int) Random.Range (int.MinValue, int.MaxValue);
                if (randomizedID != 0) {
                    bool idValid = true;
                    foreach (StructureBehaviours listedStructure in structures)
                        if (listedStructure.id == randomizedID)
                            idValid = false;
                    if (idValid) {
                        structure.id = randomizedID;
                        break;
                    }
                }
            }
        }
    }

    public void RemoveStructure (StructureBehaviours structure) {
        if (structure == null) return;
        structures.Remove (structure);
    }
}