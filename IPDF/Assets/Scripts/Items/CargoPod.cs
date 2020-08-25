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
        foreach (StructureBehaviours structureBehaviours in structuresManager.structures.ToArray ()) {
            if (structureBehaviours != null && structureBehaviours != structure) {
                if (structureBehaviours.tractorBeam != null && structureBehaviours.tractorBeam.activated) {
                    if (structureBehaviours.tractorBeam.target == gameObject) {
                        if ((structureBehaviours.transform.position - transform.position).sqrMagnitude <= (structureBehaviours.profile.apparentSize * 2) * (structureBehaviours.profile.apparentSize * 2)) {
                            structureBehaviours.tractorBeam.activated = false;
                            Item transferredItem = structure.inventory.inventory.Keys.ToArray ()[0];
                            int canTransferAmount = structureBehaviours.inventory.RoomFor (transferredItem);
                            structureBehaviours.inventory.AddItem (transferredItem, Mathf.Min (canTransferAmount, structure.inventory.inventory[transferredItem]));
                            structure.inventory.inventory[transferredItem] -= Mathf.Min (canTransferAmount, structure.inventory.inventory[transferredItem]);
                            if (structure.inventory.inventory[transferredItem] <= 0) structuresManager.RemoveStructure (structure);
                        }
                    }
                }
            }
        }
    }
}
