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
        foreach (Item item in structure.inventory.inventory.Keys.ToArray ()) {
            if (structure.inventory.GetItemCount (item) > 0) {
                GameObject pod = Instantiate (cargoPod, structure.transform.position, Quaternion.identity) as GameObject;
                pod.transform.parent = structure.transform.parent;
                pod.GetComponent<StructureBehaviours> ().inventory.AddItem (item, structure.inventory.inventory[item]);
                pod.AddComponent<CargoPod> ();
            }
        }
        if (structure.profile.debris != null) {
            GameObject debris = Instantiate (structure.profile.debris,
                structure.transform.position,
                Quaternion.Euler (new Vector3 (Random.Range (-180, 180), Random.Range (-180, 180), Random.Range (-180, 180)))
            ) as GameObject;
            debris.transform.localScale = Vector3.one * structure.profile.apparentSize;
        }
        structures.Remove (structure);
        Destroy (structure.gameObject);
    }
}