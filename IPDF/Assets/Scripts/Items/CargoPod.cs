using System.Linq;
using UnityEngine;

public class CargoPod : MonoBehaviour {
    public StructureBehaviours structure;
    public StructuresManager structuresManager;

    void Awake () {
        structure = GetComponent<StructureBehaviours> ();
        structuresManager = FindObjectOfType<StructuresManager> ();
    }

    void Update () {
        if (structure == null || !structure.initialized || structuresManager == null) return;
        gameObject.name = structure.inventory.inventory.Keys.ToArray ()[0].name + " (" + structure.inventory.GetItemCount (structure.inventory.inventory.Keys.ToArray ()[0]) + ")";
        foreach (StructureBehaviours other in structuresManager.structures.ToArray ()) {
            if (other != null && other != structure) {
                if (other.tractorBeam != null && other.tractorBeam.activated) {
                    if (other.tractorBeam.target == gameObject) {
                        if ((other.transform.position - transform.position).sqrMagnitude <= (other.profile.apparentSize * 2) * (other.profile.apparentSize * 2)) {
                            other.tractorBeam.activated = false;
                            Item transferredItem = structure.inventory.inventory.Keys.ToArray ()[0];
                            int canTransferAmount = other.inventory.RoomFor (transferredItem);
                            int has = structure.inventory.inventory[transferredItem];
                            other.inventory.AddItem (transferredItem, Mathf.Min (canTransferAmount, has));
                            structure.inventory.inventory[transferredItem] -= Mathf.Min (canTransferAmount, has);
                            if (structure.inventory.inventory[transferredItem] <= 0) {
                                structuresManager.RemoveStructure (structure);
                                structure.sector.inSector.Remove (structure);
                                Destroy (gameObject);
                            }
                        }
                    }
                }
            }
        }
    }
}
