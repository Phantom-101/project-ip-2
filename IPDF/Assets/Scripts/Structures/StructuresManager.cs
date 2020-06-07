using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Essentials;

public class StructuresManager : MonoBehaviour {
    public List<StructureBehaviours> structures;
    public GameObject cargoPod;

    public void AddStructure (StructureBehaviours structure) {
        if (structure == null) return;
        structures.Add (structure);
    }

    public void RemoveStructure (StructureBehaviours structure) {
        if (structure == null) return;
        foreach (Item item in structure.inventory.inventory.Keys.ToArray ()) {
            GameObject pod = Instantiate (cargoPod, structure.transform.position, Quaternion.identity) as GameObject;
            pod.transform.parent = structure.transform.parent;
            pod.GetComponent<StructureBehaviours> ().inventory.AddItem (item, structure.inventory.inventory[item]);
        }
        structures.Remove (structure);
        Destroy (structure.gameObject);
    }
}